using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using VNNB2B.Models;
using VNNB2B.Models.Hata;
using static VNNB2B.Controllers.Api.UrunApiController;

namespace VNNB2B.Controllers.Api
{
    public class SiparisApiController : Controller
    {
        private readonly Context c;
        public SiparisApiController(Context context)
        {
            c = context;
        }

        //Bayi Panel İşlemleri
        [HttpPost]
        public IActionResult SepetList(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.Durum == true && v.BayiID == id && v.BayiOnay == false);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            string para = "";
            if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
            if (sip != null)
            {
                var veri = c.SiparisIceriks.Where(v => v.SiparisID == sip.ID && v.Durum == true).OrderByDescending(v => v.ID).ToList();
                List<DtoSiparisIcerik> ham = new List<DtoSiparisIcerik>();
                foreach (var x in veri)
                {
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    list.ID = Convert.ToInt32(x.ID);
                    if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    if (urun.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    if (x.Miktar != null) list.Miktar = Convert.ToInt32(x.Miktar).ToString(); else list.Miktar = "1";
                    if (bayi.IskontoOran > 0)
                    {
                        if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((x.BirimFiyat * x.Miktar) - (((x.BirimFiyat * bayi.IskontoOran) / 100) * x.Miktar)).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    }
                    else
                    {
                        if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((x.BirimFiyat) * x.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    }
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2"); else list.BirimFiyat = "0,00";
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.indirimFiyat = Convert.ToDecimal(x.BirimFiyat - ((x.BirimFiyat * bayi.IskontoOran) / 100)).ToString("N2") + para; else list.indirimFiyat = "0,00" + para;
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6 && o.UrunOzellikTurlariID != null)
                        {
                            if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                            else list.DeriRengi = "";
                        }
                        else if (o.UrunOzellikTurlariID == 7 && o.UrunOzellikTurlariID != null)
                        {
                            list.DeriRengi = "";
                            if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                            else list.AhsapRengi = "";
                        }
                        else
                        {
                            list.AhsapRengi = "";
                        }
                    }
                    if (x.Aciklama != null) if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = ""; else list.Aciklama = "";
                    if (kat.ID == 63) list.UrunTuru = "Özel Ürün"; else list.UrunTuru = "Normal";
                    ham.Add(list);
                }
                return Json(ham.OrderBy(v => v.ID));
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult SepetEkle([FromBody] SepetEkleModel model)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Bayilers.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (model.Ozellikler.FirstOrDefault(v => v.OzellikAdi == "0") == null)
                {
                    var sip = c.Siparis.FirstOrDefault(v => v.BayiID == kulid && v.Durum == true && v.BayiOnay == false);
                    int sipno = 0;
                    if (sip == null)
                    {
                        Siparis s = new Siparis();
                        s.BayiID = kulid;
                        s.SiparisBayiAciklama = "";
                        s.ToplamAdet = 0;
                        s.ToplamTeslimEdilen = 0;
                        s.ToplamTutar = 0;
                        s.IskontoOran = 0;
                        s.IstoktoToplam = 0;
                        s.AraToplam = 0;
                        s.KDVToplam = 0;
                        s.OnayDurum = false;
                        s.OnayAciklama = "";
                        s.Durum = true;
                        s.SiparisDurum = "Sipariş Bayi Onay Bekliyor...";
                        s.SiparisNo = "";
                        s.BayiOnay = false;
                        c.Siparis.Add(s);
                        c.SaveChanges();
                        var sonsip = c.Siparis.OrderByDescending(v => v.ID).FirstOrDefault(v => v.Durum == true && v.BayiID == kulid);
                        sipno = sonsip.ID;
                    }
                    else
                    {
                        sipno = sip.ID;
                    }
                    var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == model.ID && v.Durum == true);
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                    decimal? birimfiyat = 0;
                    if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                    else { birimfiyat = 0; sip.ParaBirimiID = 1; }
                    var varmi = c.SiparisIceriks.FirstOrDefault(v => v.UrunID == model.ID && v.SiparisID == sipno && v.Durum == true);
                    if (varmi == null)
                    {
                        SiparisIcerik i = new SiparisIcerik();
                        i.SiparisID = sipno;
                        i.UrunID = model.ID;
                        i.Miktar = model.Miktar;
                        i.Aciklama = model.Aciklama;
                        i.BirimFiyat = birimfiyat;
                        i.SatirToplam = birimfiyat * model.Miktar;
                        i.TeslimAdet = 0;
                        i.Durum = true;
                        if (bayi.KDVDurum == true)
                        {
                            decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                            i.KDVTutari = kdv;
                        }
                        else i.KDVTutari = 0;
                        i.TeslimAdet = 0;
                        c.SiparisIceriks.Add(i);
                        c.SaveChanges();
                        var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                        bool stokdurum = false;
                        int? stokid = 0;
                        int stid = 0;
                        foreach (var v in model.Ozellikler)
                        {
                            if (v.OzellikAdi != null)
                            {
                                int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                foreach (var x in stoktavarmi)
                                {
                                    var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                    foreach (var b in ozellikler)
                                    {
                                        if (b.UrunAltOzellikleriID == ozellikid) { stokdurum = true; stokid = b.UrunStokID; } else { stokdurum = false; break; }
                                    }
                                    if (stokdurum == true)
                                    {
                                        x.StokMiktari -= sipicid.Miktar;
                                    }
                                }

                            }
                        }
                        if (stokdurum == false)
                        {
                            List<UrunOzellikleri> oz = new List<UrunOzellikleri>();
                            foreach (var x in model.Ozellikler)
                            {
                                UrunOzellikleri o = new UrunOzellikleri();
                                o.UrunAltOzellikleriID = Convert.ToInt32(x.OzellikAdi);
                                oz.Add(o);
                            }
                            UrunStoklari st = new UrunStoklari();
                            st.UrunID = sipicid.UrunID;
                            st.StokTarihi = DateTime.Now;
                            st.StokMiktari = -sipicid.Miktar;
                            st.Durum = true;
                            c.UrunStoklaris.Add(st);
                            c.SaveChanges();
                            stid = Convert.ToInt32(c.UrunStoklaris.OrderByDescending(v => v.ID).FirstOrDefault().ID);

                            Formuller f = new Formuller(c);
                            f.stokozellikleri(oz, stid);
                        }
                        foreach (var x in model.Ozellikler)
                        {
                            int ozellikid = Convert.ToInt32(x.OzellikAdi);
                            SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                            so.SiaprisIcerikID = sipicid.ID;
                            so.UrunID = model.ID;
                            if (stokdurum == true)
                            {
                                so.UrunStoklariID = stokid;
                            }
                            else
                            {
                                so.UrunStoklariID = stid;
                            }
                            so.UrunAltOzellikID = ozellikid;
                            so.Durum = true;
                            c.SiparisIcerikUrunOzellikleris.Add(so);
                            c.SaveChanges();
                        }
                    }
                    else
                    {
                        bool aynisi = false;
                        var sipoz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == varmi.ID && v.Durum == true).ToList();
                        foreach (var x in sipoz)
                        {
                            foreach (var v in model.Ozellikler)
                            {
                                int ozid = Convert.ToInt32(v.OzellikAdi);
                                if (ozid == x.UrunAltOzellikID)
                                {
                                    aynisi = true;
                                }
                                else
                                {
                                    aynisi = false;
                                    break;
                                }
                            }
                        }
                        if (aynisi == true)
                        {
                            varmi.Miktar += model.Miktar;
                            varmi.Aciklama = model.Aciklama;
                            varmi.BirimFiyat = birimfiyat;
                            varmi.SatirToplam = birimfiyat * varmi.Miktar;
                            if (bayi.KDVDurum == true)
                            {
                                decimal kdv = Convert.ToDecimal((varmi.SatirToplam * 10) / 100);
                                varmi.KDVTutari = kdv;
                            }
                            else varmi.KDVTutari = 0;
                        }
                        else
                        {
                            SiparisIcerik i = new SiparisIcerik();
                            i.SiparisID = sipno;
                            i.UrunID = model.ID;
                            i.Miktar = model.Miktar;
                            i.Aciklama = model.Aciklama;
                            i.BirimFiyat = birimfiyat;
                            i.SatirToplam = birimfiyat * model.Miktar;
                            i.TeslimAdet = 0;
                            i.Durum = true;
                            if (bayi.KDVDurum == true)
                            {
                                decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                                i.KDVTutari = kdv;
                            }
                            else i.KDVTutari = 0;
                            i.TeslimAdet = 0;
                            c.SiparisIceriks.Add(i);
                            c.SaveChanges();
                            var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                            bool stokdurum = false;
                            int? stokid = 0;
                            int stid = 0;
                            foreach (var v in model.Ozellikler)
                            {
                                if (v.OzellikAdi != null)
                                {
                                    int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                    var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                    var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                    foreach (var x in stoktavarmi)
                                    {
                                        var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                        foreach (var b in ozellikler)
                                        {
                                            if (b.UrunAltOzellikleriID == ozellikid) { stokdurum = true; stokid = b.UrunStokID; } else { stokdurum = false; break; }
                                        }
                                        if (stokdurum == true)
                                        {
                                            x.StokMiktari -= sipicid.Miktar;
                                        }
                                    }

                                }
                            }
                            if (stokdurum == false)
                            {
                                List<UrunOzellikleri> oz = new List<UrunOzellikleri>();
                                foreach (var x in model.Ozellikler)
                                {
                                    UrunOzellikleri o = new UrunOzellikleri();
                                    o.UrunAltOzellikleriID = Convert.ToInt32(x.OzellikAdi);
                                    oz.Add(o);
                                }
                                UrunStoklari st = new UrunStoklari();
                                st.UrunID = sipicid.UrunID;
                                st.StokTarihi = DateTime.Now;
                                st.StokMiktari = -sipicid.Miktar;
                                st.Durum = true;
                                c.UrunStoklaris.Add(st);
                                c.SaveChanges();
                                stid = Convert.ToInt32(c.UrunStoklaris.OrderByDescending(v => v.ID).FirstOrDefault().ID);

                                Formuller f = new Formuller(c);
                                f.stokozellikleri(oz, stid);
                            }

                            foreach (var x in model.Ozellikler)
                            {
                                int ozellikid = Convert.ToInt32(x.OzellikAdi);
                                SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                                so.SiaprisIcerikID = sipicid.ID;
                                so.UrunID = model.ID;
                                if (stokdurum == true)
                                {
                                    so.UrunStoklariID = stokid;
                                }
                                else
                                {
                                    so.UrunStoklariID = stid;
                                }
                                so.UrunAltOzellikID = ozellikid;
                                so.Durum = true;
                                c.SiparisIcerikUrunOzellikleris.Add(so);
                                c.SaveChanges();
                            }
                        }
                    }
                    if (bayi.IskontoOran != null) sip.IskontoOran = bayi.IskontoOran; else sip.IskontoOran = 0;
                    c.SaveChanges();
                    siphesapla(sip.ID);
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Ürün Özelliklerini Boş Bırakmayınız..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult SepetEkle2([FromBody] SepetEkleModel model)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Bayilers.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (model.Ozellikler.FirstOrDefault(v => v.OzellikAdi == "0") == null)
                {
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == model.SiparisID);
                    int sipno = sip.ID;
                    var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == model.ID && v.Durum == true);
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                    decimal? birimfiyat = 0;
                    if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                    else { birimfiyat = 0; sip.ParaBirimiID = 1; }
                    var varmi = c.SiparisIceriks.FirstOrDefault(v => v.UrunID == model.ID && v.SiparisID == sipno && v.Durum == true);

                    if (varmi == null)
                    {
                        SiparisIcerik i = new SiparisIcerik();
                        i.SiparisID = sipno;
                        i.UrunID = model.ID;
                        i.Miktar = model.Miktar;
                        i.Aciklama = model.Aciklama;
                        i.BirimFiyat = birimfiyat;
                        i.SatirToplam = birimfiyat * model.Miktar;
                        i.TeslimAdet = 0;
                        i.Durum = true;
                        if (bayi.KDVDurum == true)
                        {
                            decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                            i.KDVTutari = kdv;
                        }
                        else i.KDVTutari = 0;
                        c.SiparisIceriks.Add(i);
                        c.SaveChanges();
                        var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                        bool stokdurum = false;
                        int? stokid = 0;
                        foreach (var v in model.Ozellikler)
                        {
                            if (v.OzellikAdi != null)
                            {
                                int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                foreach (var x in stoktavarmi)
                                {
                                    var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                    foreach (var b in ozellikler)
                                    {
                                        if (b.UrunAltOzellikleriID == ozellikid)
                                        { stokdurum = true; stokid = b.UrunStokID; }
                                        else
                                        { stokdurum = false; break; }
                                    }
                                }

                            }
                        }

                        foreach (var x in model.Ozellikler)
                        {
                            int ozellikid = Convert.ToInt32(x.OzellikAdi);
                            SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                            so.SiaprisIcerikID = sipicid.ID;
                            so.UrunID = model.ID;
                            if (stokdurum == true)
                            {
                                so.UrunStoklariID = stokid;
                            }
                            so.UrunAltOzellikID = ozellikid;
                            so.Durum = true;
                            c.SiparisIcerikUrunOzellikleris.Add(so);
                            c.SaveChanges();
                        }
                    }
                    else
                    {
                        bool aynisi = false;
                        var sipoz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == varmi.ID && v.Durum == true).ToList();
                        foreach (var x in sipoz)
                        {
                            foreach (var v in model.Ozellikler)
                            {
                                int ozid = Convert.ToInt32(v.OzellikAdi);
                                if (ozid == x.UrunAltOzellikID)
                                {
                                    aynisi = true;
                                }
                                else
                                {
                                    aynisi = false;
                                    break;
                                }
                            }
                        }
                        if (aynisi == true)
                        {
                            varmi.Miktar += model.Miktar;
                            varmi.Aciklama = model.Aciklama;
                            varmi.BirimFiyat = birimfiyat;
                            varmi.SatirToplam = birimfiyat * varmi.Miktar;
                            if (bayi.KDVDurum == true)
                            {
                                decimal kdv = Convert.ToDecimal((varmi.SatirToplam * 10) / 100);
                                varmi.KDVTutari = kdv;
                            }
                            else varmi.KDVTutari = 0;
                        }
                        else
                        {
                            SiparisIcerik i = new SiparisIcerik();
                            i.SiparisID = sipno;
                            i.UrunID = model.ID;
                            i.Miktar = model.Miktar;
                            i.Aciklama = model.Aciklama;
                            i.BirimFiyat = birimfiyat;
                            i.SatirToplam = birimfiyat * model.Miktar;
                            i.Durum = true;
                            i.TeslimAdet = 0;
                            if (bayi.KDVDurum == true)
                            {
                                decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                                i.KDVTutari = kdv;
                            }
                            else i.KDVTutari = 0;
                            c.SiparisIceriks.Add(i);
                            c.SaveChanges();
                            var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                            bool stokdurum = false;
                            int? stokid = 0;
                            foreach (var v in model.Ozellikler)
                            {
                                if (v.OzellikAdi != null)
                                {
                                    int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                    var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                    var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                    foreach (var x in stoktavarmi)
                                    {
                                        var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                        foreach (var b in ozellikler)
                                        {
                                            if (b.UrunAltOzellikleriID == ozellikid) { stokdurum = true; stokid = b.UrunStokID; } else { stokdurum = false; break; }
                                        }
                                    }

                                }
                            }

                            foreach (var x in model.Ozellikler)
                            {
                                int ozellikid = Convert.ToInt32(x.OzellikAdi);
                                SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                                so.SiaprisIcerikID = sipicid.ID;
                                so.UrunID = model.ID;
                                if (stokdurum == true)
                                {
                                    so.UrunStoklariID = stokid;
                                }
                                so.UrunAltOzellikID = ozellikid;
                                so.Durum = true;
                                c.SiparisIcerikUrunOzellikleris.Add(so);
                                c.SaveChanges();
                            }
                        }
                    }
                    if (bayi.IskontoOran != null) sip.IskontoOran = bayi.IskontoOran; else sip.IskontoOran = 0;
                    c.SaveChanges();
                    siphesapla(sip.ID);
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
                else
                {
                    result = new { status = "error", message = "Lütfen Ürün Özelliklerini Boş Bırakmayınız..." };
                }
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult SepetSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == id);
            de.Durum = false;
            //int stokid = 0;
            //var stok = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == id && v.Durum == true).ToList();
            //foreach (var x in stok)
            //{
            //    if (x.UrunStoklariID != null) stokid = Convert.ToInt32(x.UrunStoklariID);
            //}
            //if (stokid > 0) { var stokbul = c.UrunStoklaris.FirstOrDefault(v => v.ID == stokid).StokMiktari += de.Miktar; c.SaveChanges(); }
            c.SaveChanges();
            siphesapla(Convert.ToInt32(de.SiparisID));
            if (c.SiparisIceriks.Where(v => v.SiparisID == de.SiparisID && v.Durum == true).Count() <= 0)
            {
                var sip = c.Siparis.FirstOrDefault(v => v.ID == de.SiparisID);
                sip.Durum = false;
                c.SaveChanges();
            }
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SepetGuncelle(SiparisIcerik s)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == s.ID);
            if (s.Miktar != null)
                de.Miktar = s.Miktar;
            if (s.BirimFiyat != null)
                de.BirimFiyat = s.BirimFiyat;
            if (s.Aciklama != null)
                de.Aciklama = s.Aciklama;
            de.SatirToplam = de.BirimFiyat * de.Miktar;
            c.SaveChanges();
            Formuller form = new Formuller(c);
            form.siphesapla(Convert.ToInt32(de.SiparisID));
            result = new { status = "success", message = "Satır Güncellendi..." };

            return Json(result);
        }
        [HttpPost]
        public IActionResult SiparisOnay(int id, string musteri)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.Siparis.FirstOrDefault(v => v.BayiID == id && v.BayiOnay == false);
            sip.BayiOnay = true;
            sip.SiparisDurum = "Onay Bekliyor...";
            sip.SiparisBayiAciklama = musteri;
            c.SaveChanges();
            result = new { status = "success", message = "Sepet Onaylandı... Sipariş Durumunu Siparişlerim Sekmesinden Kontrol Edebilirsiniz..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SiparisList(int id)
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.BayiID == id && v.BayiOnay == true).OrderByDescending(v => v.SiparisTarihi).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                list.TerminHaftasi = x.TerminHaftasi;
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult SiparisList10(int id)
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.BayiID == id && v.BayiOnay == true).OrderByDescending(v => v.SiparisTarihi).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                ham.Add(list);
            }
            return Json(ham.Take(10));
        }
        [HttpPost]
        public IActionResult SepetIcerik(int id)
        {
            var x = c.Siparis.FirstOrDefault(v => v.Durum == true && v.BayiID == id && v.BayiOnay == false);
            if (x != null)
            {
                string para = "";
                if (x.ParaBirimiID == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == id);
                list.Unvan = bayi.Unvan.ToString();
                list.FirmaAdi = bayi.KullaniciAdi.ToString();
                list.Adres = bayi.Adres.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                list.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
                list.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
                list.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
                if (x.OnaylayanID != null && x.OnaylayanID > 0)
                    list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.OnaylayanID).AdSoyad.ToString();
                else
                    list.Kullanici = "";
                list.indirimlifiyat = Convert.ToDecimal(x.AraToplam - x.IstoktoToplam).ToString("N2") + para;
                var icerik = c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
                decimal toplamm3 = 0;
                decimal toplamkg = 0;
                decimal toplampaketadet = 0;
                foreach (var v in icerik)
                {
                    var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                    if (urun.BirimKG != null) toplamkg += Convert.ToDecimal(urun.BirimKG * v.Miktar);
                    if (urun.BirimM3 != null) toplamm3 += Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                    if (urun.PaketAdet != null) toplampaketadet += Convert.ToDecimal(urun.PaketAdet * v.Miktar);
                }
                list.toplamm3 = toplamm3.ToString("N2");
                list.toplamkg = toplamkg.ToString("N2");
                list.toplamparcaadet = toplampaketadet.ToString("N2");
                return Json(list);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult SepetBilgi(int id)
        {
            var sepet = c.Siparis.FirstOrDefault(v => v.BayiID == id && v.BayiOnay == false && v.Durum == true);
            int x = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == sepet.ID && v.Durum == true).Count());
            DtoSiparis list = new DtoSiparis();
            list.SepetAdet = x.ToString();
            return Json(list);
        }
        [HttpPost]
        public IActionResult SiparisSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            Siparis de = c.Siparis.FirstOrDefault(v => v.ID == id);
            //Kaydedilmemişlerle çakışmaması için bayi onayını kaldırıyoruz
            de.Durum = false;
            de.BayiOnay = false;
            Formuller f = new Formuller(c);
            f.SiparisSil(id);
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult TopluGuncelle([FromBody] List<UrunGuncelleDto> guncellenecekVeriler)
        {
            var result = new { status = "error", message = "İşlem Başarısız..." };
            foreach (var x in guncellenecekVeriler)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.Id);
                sipic.Miktar = x.Miktar;
                sipic.SatirToplam = sipic.BirimFiyat * x.Miktar;
                c.SaveChanges();
                Formuller form = new Formuller(c);
                form.siphesapla(Convert.ToInt32(sipic.SiparisID));
                c.SaveChanges();
            }

            result = new { status = "success", message = "İŞLEM BAŞARILI..." };
            return Json(result);

        }
        public class UrunGuncelleDto
        {
            public int Id { get; set; }
            public int Miktar { get; set; }
        }

        //Admin Panel İşlemleri
        [HttpPost]
        public IActionResult OnayBekleyenList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum != true && v.BayiOnay == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public async Task<IActionResult> SiparisOnayla(int id, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            if (sip.BayiOnay != true)
            {
                result = new { status = "error", message = "BAYİ HENÜZ SİPARİŞİ ONAYLAMAMIŞ!!!" };
                return Json(result);
            }
            if (sip.SiparisNo == null || sip.TerminHaftasi == null || sip.TeslimatSekli == null)
            {
                SiparisHata.Icerik = "SİPARİŞ BİLGİLERİ GİRİLMEDİ LÜTFEN GEREKLİ ALANLARI BOŞ DOLDURMAYINIZ!!!";
                return RedirectToAction("Index", "Siparis");
            }

            if (imagee != null)
            {
                var iceirk = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
                if (iceirk.Count() > 0)
                {

                    var dosyaAdi = Path.GetFileName(imagee.FileName);

                    var dosyaYolu = Path.Combine("wwwroot/Evraklar/SiparisOnayEvraklar", dosyaAdi);

                    using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                    {
                        await imagee.CopyToAsync(stream);
                    }

                    sip.SiparisDurum = "Sipariş Onaylandı...";

                    sip.DosyaYolu = dosyaYolu.Substring(7);

                    sip.OnayDurum = true;
                    sip.OnaylayanID = kulid;
                    sip.OnayTarihi = DateTime.Now;

                    Formuller formuller = new Formuller(c);
                    formuller.SiparisOnay(id, kulid);
                    c.SaveChanges();
                    foreach (var x in iceirk)
                    {
                        var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                        urun.StokMiktari -= x.Miktar;
                        c.SaveChanges();
                    }
                    SiparisHata.Icerik = "Sipariş Onaylandı...";
                }
                else
                {
                    SiparisHata.Icerik = "Sipariş İçeriği Bulunamadı Lütfen Ürün Ekleyip Tekrar Deneyiniz...";
                }
            }
            else
            {
                SiparisHata.Icerik = "Onay Evrağını Eklemeden Siparişi Onaylayamazsınız...";
            }
            return RedirectToAction("Index", "Siparis");
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;

                var icerik = c.TeslimatIceriks.Where(v => v.SiparisID == x.ID).ToList();
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                foreach (var v in icerik)
                {
                    var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                    decimal? birimfiyat = 0;
                    if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; x.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; x.ParaBirimiID = 2; }
                    else { birimfiyat = 0; x.ParaBirimiID = 1; }

                    IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * x.IskontoOran) / 100);
                    AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.ToplamTeslimTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KalanMiktar = (Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)) - Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar))).ToString();
                list.KalanTutar = (Convert.ToDecimal(x.ToplamTutar) - ((AraToplam + KDVToplam) - IstoktoToplam)).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        //Ana Sayfa Ürün Arama İşlemi
        [HttpPost]
        public IActionResult FirltreList()
        {
            var query = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true).OrderByDescending(x => x.SiparisTarihi).ToList();
            List<DtoSiparisIcerik> ham = new List<DtoSiparisIcerik>();
            foreach (var a in query)
            {
                var icerik = c.SiparisIceriks.Where(v => v.SiparisID == a.ID && v.Durum == true).ToList();
                foreach (var x in icerik)
                {
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == a.BayiID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    list.ID = Convert.ToInt32(x.ID);
                    list.UrunID = x.UrunID.ToString();
                    if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "";
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "";
                    if (urun.Boyut != null) list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    list.SiparisAdi = a.SiparisBayiAciklama.ToString();

                    // Adet bilgileri
                    list.DepoAdet = c.DepoIsEmirleris.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true)
                        .Sum(v => v.GelenAdet - v.GidenAdet).ToString();
                    list.BoyaAdet = c.BoyaIsEmirleris.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true)
                        .Sum(v => v.GelenAdet - v.GidenAdet).ToString();
                    list.DosemeAdet = c.DosemeIsEmirleris.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true)
                        .Sum(v => v.GelenAdet - v.GidenAdet).ToString();
                    list.AmbalajAdet = c.AmbalajIsEmirleris.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true)
                        .Sum(v => v.GelenAdet - v.GidenAdet).ToString();
                    list.SevkiyatAdet = c.SevkiyatIsEmirleris.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true)
                        .Sum(v => v.GelenAdet - x.TeslimAdet).ToString();

                    string bayiadi = "";
                    if (bayi != null)
                    {
                        if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString();
                    }
                    list.BayiID = bayiadi;
                    list.SiparisID = a.SiparisNo.ToString();
                    if (a.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(a.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";
                    list.Miktar = Convert.ToInt32(x.Miktar).ToString();
                    list.HazirAdet = Convert.ToInt32(x.HazirAdet).ToString();
                    int teslimadet = Convert.ToInt32(x.TeslimAdet);
                    list.KalanMiktar = Convert.ToInt32(x.Miktar - teslimadet).ToString();
                    if (x.TeslimAdet != null) list.TeslimAdet = Convert.ToInt32(x.TeslimAdet).ToString(); else list.TeslimAdet = "0";
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6)
                        {
                            if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                            else list.DeriRengi = "";
                        }
                        else if (o.UrunOzellikTurlariID == 7)
                        {
                            list.DeriRengi = "";
                            if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                            else list.AhsapRengi = "";
                        }
                        else
                        {
                            list.AhsapRengi = "";
                        }
                    }
                    if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                    ham.Add(list);
                }
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult KaydedilmeyenList()
        {
            var veri = c.Siparis.Where(v => v.Durum == false && v.OnayDurum != true && v.BayiOnay == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (x.SiparisBayiAciklama != null) bayiadi += " - " + x.SiparisBayiAciklama.ToString();
                }
                list.BayiID = bayiadi;
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult KTList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.ToplamTeslimEdilen > 0 && v.ToplamTeslimEdilen != v.ToplamAdet).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;

                var icerik = c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                int teslimmik = 0;
                foreach (var v in icerik)
                {
                    var tes = c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID);
                    if (tes.Durum == true)
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                        decimal? birimfiyat = 0;
                        if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; x.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; x.ParaBirimiID = 2; }
                        else { birimfiyat = 0; x.ParaBirimiID = 1; }

                        IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * x.IskontoOran) / 100);
                        AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                        teslimmik += Convert.ToInt32(v.Miktar);
                    }
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.ToplamTeslimTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KalanMiktar = (Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)) - teslimmik).ToString();
                list.KalanTutar = (Convert.ToDecimal(x.ToplamTutar) - ((AraToplam + KDVToplam) - IstoktoToplam)).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult YHList()
        {
            var veri = c.SiparisIceriks.Where(v => v.Durum == true && v.YuklemeyeHazir == true && v.Miktar != v.TeslimAdet && v.TeslimDurum != true).ToList();
            if (veri.Count() > 0)
            {
                List<DtoSiparisIcerik> icerikler = new List<DtoSiparisIcerik>();
                foreach (var x in veri)
                {
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    if (sip.OnayDurum == true)
                    {
                        DtoSiparisIcerik list = new DtoSiparisIcerik();
                        var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                        var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                        list.ID = Convert.ToInt32(x.ID);
                        list.UrunID = x.UrunID.ToString();
                        list.UrunKodu = urun.UrunKodu.ToString();
                        var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                        if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                        if (urun.Boyut != null)
                            list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                        string bayiadi = "";
                        if (bayi != null)
                        {
                            if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                            if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
                        }
                        list.BayiID = bayiadi;
                        list.SiparisID = sip.SiparisNo.ToString();
                        if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";
                        list.Miktar = Convert.ToInt32(x.Miktar).ToString();
                        list.HazirAdet = Convert.ToInt32(x.HazirAdet).ToString();
                        list.TeslimAdet = Convert.ToInt32(x.TeslimAdet).ToString();
                        list.KalanAdet = Convert.ToInt32(x.Miktar - x.TeslimAdet).ToString();
                        var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                        foreach (var v in ozellik)
                        {
                            var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                            if (o.UrunOzellikTurlariID == 6)
                            {
                                if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                                else list.DeriRengi = "";
                            }
                            else if (o.UrunOzellikTurlariID == 7)
                            {
                                list.DeriRengi = "";
                                if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                                else list.AhsapRengi = "";
                            }
                            else
                            {
                                list.AhsapRengi = "";
                            }
                        }
                        if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                        icerikler.Add(list);

                    }
                }
                return Json(icerikler);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparisIcerik> ham = new List<DtoSiparisIcerik>();
            foreach (var a in veri)
            {
                var icerik = c.SiparisIceriks.Where(v => v.SiparisID == a.ID && v.Durum == true).ToList();
                foreach (var x in icerik)
                {
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == a.BayiID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    list.ID = Convert.ToInt32(a.ID);
                    list.UrunID = x.UrunID.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    string bayiadi = "";
                    if (bayi != null)
                    {
                        if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                        if (a.SiparisBayiAciklama != null) bayiadi += " - " + a.SiparisBayiAciklama.ToString();
                    }
                    list.BayiID = bayiadi;
                    list.SiparisID = a.SiparisNo.ToString();
                    if (a.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(a.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";
                    list.Miktar = Convert.ToInt32(x.Miktar).ToString();
                    list.HazirAdet = Convert.ToInt32(x.HazirAdet).ToString();
                    list.KalanMiktar = Convert.ToInt32(x.Miktar - x.TeslimAdet).ToString();
                    if (x.TeslimAdet != null) list.TeslimAdet = Convert.ToInt32(x.TeslimAdet).ToString();
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6)
                        {
                            if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                            else list.DeriRengi = "";
                        }
                        else if (o.UrunOzellikTurlariID == 7)
                        {
                            list.DeriRengi = "";
                            if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                            else list.AhsapRengi = "";
                        }
                        else
                        {
                            list.AhsapRengi = "";
                        }
                    }
                    if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                    ham.Add(list);
                }
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult TeslimEdilebilirList()
        {
            List<DtoSiparisIcerik> icerik = new List<DtoSiparisIcerik>();
            var hazir = c.SevkiyatIsEmirleris.Where(v => v.BitirmeDurum == true && v.Durum == true).ToList();
            foreach (var x in hazir)
            {
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID && v.TeslimDurum != true);
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoSiparisIcerik list = new DtoSiparisIcerik();
                list.ID = Convert.ToInt32(x.SiparisIcerikID);
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAciklama = urun.UrunAdi.ToString();
                list.Miktar = Convert.ToInt32(sipic.Miktar).ToString();
                list.Durum = sip.SiparisDurum.ToString();
                icerik.Add(list);

            }
            return Json(icerik);
        }
        [HttpPost]
        public IActionResult SiparisDetay(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            string para = "";
            if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
            if (sip != null)
            {
                var veri = c.SiparisIceriks.Where(v => v.SiparisID == sip.ID && v.Durum == true).ToList();
                List<DtoSiparisIcerik> ham = new List<DtoSiparisIcerik>();
                foreach (var x in veri)
                {
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    list.ID = Convert.ToInt32(x.ID);
                    if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "";
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "";
                    if (urun.Boyut != null)
                        list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    if (urun.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    if (x.Aciklama != null) if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = ""; else list.Aciklama = "Açıklama Yok!";
                    list.Miktar = Convert.ToInt32(x.Miktar).ToString();
                    list.KalanMiktar = Convert.ToInt32(x.Miktar - x.TeslimAdet).ToString();
                    if (x.TeslimAdet != null) list.TeslimAdet = Convert.ToInt32(x.TeslimAdet).ToString(); else list.TeslimAdet = "0";
                    if (bayi.IskontoOran > 0)
                    {
                        if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((x.BirimFiyat * x.Miktar) - (((x.BirimFiyat * bayi.IskontoOran) / 100) * x.Miktar)).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    }
                    else
                    {
                        if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((x.BirimFiyat) * x.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    }
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.indirimFiyat = Convert.ToDecimal(x.BirimFiyat - ((x.BirimFiyat * bayi.IskontoOran) / 100)).ToString("N2") + para; else list.indirimFiyat = "0,00" + para;
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6)
                        {
                            if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                            else list.DeriRengi = "";
                        }
                        else if (o.UrunOzellikTurlariID == 7)
                        {
                            list.DeriRengi = "";
                            if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                            else list.AhsapRengi = "";
                        }
                        else
                        {
                            list.AhsapRengi = "";
                        }
                    }
                    if (kat.ID == 63) list.UrunTuru = "Özel Ürün"; else list.UrunTuru = "Normal";
                    ham.Add(list);
                }
                return Json(ham.OrderBy(v => v.ID));
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult SiparisDetayBilgi(int id)
        {
            var x = c.Siparis.FirstOrDefault(v => v.ID == id);
            if (x != null)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (x.ParaBirimiID == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.TeslimTarihi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama.ToString(); else list.SiparisBayiAciklama = "";
                if (x.TerminTarihi != null) list.TerminTarihi = Convert.ToDateTime(x.TerminTarihi).ToString("dd/MM/yyyy"); else list.TerminTarihi = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                list.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
                list.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
                list.DosyaYolu = x.DosyaYolu;
                list.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
                if (x.OnayDurum == true) list.OnayDurum = "Onaylandı"; else list.OnayDurum = "Onaylanmadı";
                if (x.OnaylayanID != null && x.OnaylayanID > 0)
                    list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.OnaylayanID).AdSoyad.ToString();
                else
                    list.Kullanici = "";
                list.indirimlifiyat = Convert.ToDecimal(x.AraToplam - x.IstoktoToplam).ToString("N2") + para;
                if (bayi.Yetkili != null) list.Yetkili = bayi.Yetkili.ToString(); else list.Yetkili = "";
                if (bayi.Unvan != null) list.Unvan = bayi.Unvan.ToString(); else list.Unvan = "";
                if (bayi.KullaniciAdi != null) list.FirmaAdi = bayi.KullaniciAdi.ToString(); else list.FirmaAdi = "";
                if (bayi.Telefon != null) list.Telefon = bayi.Telefon.ToString(); else list.Telefon = "";
                if (bayi.Adres != null) list.Adres = bayi.Adres.ToString(); else list.Adres = "";
                if (x.TeslimatSekli != null) list.TeslimatSekli = x.TeslimatSekli.ToString(); else list.TeslimatSekli = "";
                if (x.TerminHaftasi != null) list.TerminHaftasi = x.TerminHaftasi.ToString(); else list.TerminHaftasi = "";
                var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
                decimal toplamm3 = 0;
                decimal toplamkg = 0;
                decimal toplampaketadet = 0;
                foreach (var v in icerik)
                {
                    var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                    if (urun.BirimKG != null) toplamkg += Convert.ToDecimal(urun.BirimKG * v.Miktar);
                    if (urun.BirimM3 != null) toplamm3 += Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                    if (urun.PaketAdet != null) toplampaketadet += Convert.ToDecimal(urun.PaketAdet * v.Miktar);
                }
                list.toplamm3 = toplamm3.ToString("N2");
                list.toplamkg = toplamkg.ToString("N2");
                list.toplamparcaadet = toplampaketadet.ToString("N2");
                return Json(list);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult SiparisBilgiDuzenle(Siparis s)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            //if (s.SiparisTarihi.Value.Day < DateTime.Now.Day)
            //{
            //    result = new { status = "error", message = "Sipariş Tarihi Eski Tarih Olamaz!!!" };
            //    return Json(result);
            //}
            Siparis de = c.Siparis.FirstOrDefault(v => v.ID == s.ID);

            if (s.SiparisBayiAciklama != null)
                de.SiparisBayiAciklama = s.SiparisBayiAciklama;
            if (s.SiparisNo != null)
                de.SiparisNo = s.SiparisNo;
            else if (s.SiparisNo == null && (de.SiparisNo == null || de.SiparisNo.Length == 0))
            {
                de.SiparisNo = "F" + DateTime.Now.Year.ToString() + (c.Siparis.Count() + 1).ToString();
            }
            if (s.SiparisTarihi != null && de.SiparisTarihi != null)
                de.SiparisTarihi = s.SiparisTarihi;
            else de.SiparisTarihi = DateTime.Now;
            if (s.TeslimatSekli != null)
                de.TeslimatSekli = s.TeslimatSekli;
            else de.TeslimatSekli = "FABRİKA TESLİM";
            if (s.TerminHaftasi != null)
            {
                string terminHaftasi = s.TerminHaftasi;
                string res = GetFormattedWeekRange(terminHaftasi);
                de.TerminHaftasi = res;
            }
            c.SaveChanges();
            result = new { status = "success", message = "Bilgiler Güncellendi..." };
            return Json(result);
        }
        public static string GetFormattedWeekRange(string weekString)
        {
            string[] parts = weekString.Split('-');
            int year = int.Parse(parts[0]);
            int weekNumber = int.Parse(parts[1].Substring(1));

            DateTime jan1 = new DateTime(year, 1, 1);
            DateTime firstMonday = jan1.AddDays((8 - (int)jan1.DayOfWeek) % 7);

            DateTime startDate = firstMonday.AddDays((weekNumber - 1) * 7);

            DateTime endDate = startDate.AddDays(6);

            string startMonthName = startDate.ToString("MMMM", CultureInfo.GetCultureInfo("tr-TR"));
            string endMonthName = endDate.ToString("MMMM", CultureInfo.GetCultureInfo("tr-TR"));

            return $"({startMonthName} - {startDate.Day}) / ({endMonthName} - {endDate.Day}) - ({endDate.Year})";
        }
        public IActionResult SiparisYeniOnay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            if (sip.Durum == true)
            {
                Formuller formuller = new Formuller(c);
                formuller.SiparisOnay(id, kulid);
                c.SaveChanges();
            }
            sip.Durum = true;
            c.SaveChanges();
            SiparisHata.Icerik = "Sipariş Kaydedildi...";
            return RedirectToAction("Index", "Siparis");
        }
        [HttpPost]
        public IActionResult SYSSepetEkle([FromBody] SepetEkleModel model)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Bayilers.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var sip = c.Siparis.FirstOrDefault(v => v.ID == model.SiparisID);
                int sipno = sip.ID;
                var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == model.ID && v.Durum == true);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                decimal? birimfiyat = 0;
                if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                else { birimfiyat = 0; sip.ParaBirimiID = 1; }
                var varmi = c.SiparisIceriks.FirstOrDefault(v => v.UrunID == model.ID && v.SiparisID == sipno && v.Durum == true);
                if (model.Ozellikler.FirstOrDefault(v => v.OzellikAdi == "0") == null)
                {
                    if (varmi == null)
                    {
                        SiparisIcerik i = new SiparisIcerik();
                        i.SiparisID = sipno;
                        i.UrunID = model.ID;
                        i.Miktar = model.Miktar;
                        i.Aciklama = model.Aciklama;
                        i.BirimFiyat = birimfiyat;
                        i.SatirToplam = birimfiyat * model.Miktar;
                        i.Durum = true;
                        i.TeslimAdet = 0;
                        if (bayi.KDVDurum == true)
                        {
                            decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                            i.KDVTutari = kdv;
                        }
                        else i.KDVTutari = 0;
                        c.SiparisIceriks.Add(i);
                        c.SaveChanges();
                        var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                        if (sip.OnayDurum == true)
                        {
                            var urun = c.Urunlers.FirstOrDefault(v => v.ID == model.ID);
                            urun.StokMiktari -= model.Miktar;
                            c.SaveChanges();
                        }
                        bool stokdurum = false;
                        int? stokid = 0;
                        foreach (var v in model.Ozellikler)
                        {
                            if (v.OzellikAdi != null)
                            {
                                int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                foreach (var x in stoktavarmi)
                                {
                                    var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                    foreach (var b in ozellikler)
                                    {
                                        if (b.UrunAltOzellikleriID == ozellikid)
                                        { stokdurum = true; stokid = b.UrunStokID; }
                                        else
                                        { stokdurum = false; break; }
                                    }
                                }

                            }
                        }

                        foreach (var x in model.Ozellikler)
                        {
                            int ozellikid = Convert.ToInt32(x.OzellikAdi);
                            SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                            so.SiaprisIcerikID = sipicid.ID;
                            so.UrunID = model.ID;
                            if (stokdurum == true)
                            {
                                so.UrunStoklariID = stokid;
                            }
                            so.UrunAltOzellikID = ozellikid;
                            so.Durum = true;
                            c.SiparisIcerikUrunOzellikleris.Add(so);
                            c.SaveChanges();
                        }
                    }
                    else
                    {
                        bool aynisi = false;
                        var sipoz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == varmi.ID && v.Durum == true).ToList();
                        foreach (var x in sipoz)
                        {
                            foreach (var v in model.Ozellikler)
                            {
                                int ozid = Convert.ToInt32(v.OzellikAdi);
                                if (ozid == x.UrunAltOzellikID)
                                {
                                    aynisi = true;
                                }
                                else
                                {
                                    aynisi = false;
                                    break;
                                }
                            }
                        }
                        if (aynisi == true)
                        {
                            if (sip.OnayDurum == true)
                            {
                                var urun = c.Urunlers.FirstOrDefault(v => v.ID == model.ID);
                                urun.StokMiktari += varmi.Miktar;
                                urun.StokMiktari -= model.Miktar;
                                c.SaveChanges();
                            }
                            varmi.Miktar += model.Miktar;
                            varmi.Aciklama = model.Aciklama;
                            varmi.BirimFiyat = birimfiyat;
                            varmi.SatirToplam = birimfiyat * varmi.Miktar;
                            if (bayi.KDVDurum == true)
                            {
                                decimal kdv = Convert.ToDecimal((varmi.SatirToplam * 10) / 100);
                                varmi.KDVTutari = kdv;
                            }
                            else varmi.KDVTutari = 0;
                        }
                        else
                        {
                            SiparisIcerik i = new SiparisIcerik();
                            i.SiparisID = sipno;
                            i.UrunID = model.ID;
                            i.Miktar = model.Miktar;
                            i.Aciklama = model.Aciklama;
                            i.BirimFiyat = birimfiyat;
                            i.SatirToplam = birimfiyat * model.Miktar;
                            i.Durum = true;
                            if (bayi.KDVDurum == true)
                            {
                                decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                                i.KDVTutari = kdv;
                            }
                            else i.KDVTutari = 0;
                            c.SiparisIceriks.Add(i);
                            c.SaveChanges();
                            var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                            bool stokdurum = false;
                            int? stokid = 0;
                            foreach (var v in model.Ozellikler)
                            {
                                if (v.OzellikAdi != null)
                                {
                                    int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                    var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                    var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                    foreach (var x in stoktavarmi)
                                    {
                                        var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                        foreach (var b in ozellikler)
                                        {
                                            if (b.UrunAltOzellikleriID == ozellikid) { stokdurum = true; stokid = b.UrunStokID; } else { stokdurum = false; break; }
                                        }
                                    }

                                }
                            }

                            foreach (var x in model.Ozellikler)
                            {
                                int ozellikid = Convert.ToInt32(x.OzellikAdi);
                                SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                                so.SiaprisIcerikID = sipicid.ID;
                                so.UrunID = model.ID;
                                if (stokdurum == true)
                                {
                                    so.UrunStoklariID = stokid;
                                }
                                so.UrunAltOzellikID = ozellikid;
                                so.Durum = true;
                                c.SiparisIcerikUrunOzellikleris.Add(so);
                                c.SaveChanges();
                            }
                        }
                    }
                    if (bayi.IskontoOran != null) sip.IskontoOran = bayi.IskontoOran; else sip.IskontoOran = 0;
                    c.SaveChanges();
                    siphesapla(sip.ID);
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
                else
                {
                    result = new { status = "error", message = "Lütfen Ürün Özelliklerini Boş Bırakmayınız..." };
                }
            }
            return Json(result);
        }
        public void siphesapla(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            decimal? bayioran = sip.IskontoOran;
            decimal toplamtutar = 0;
            decimal toplamadet = 0;
            decimal iskontotoplam = 0;

            toplamtutar = Convert.ToDecimal(c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).Sum(v => v.SatirToplam));
            toplamadet = Convert.ToDecimal(c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).Sum(v => v.Miktar));
            iskontotoplam = Convert.ToDecimal((toplamtutar * bayioran) / 100);

            sip.ToplamAdet = toplamadet;
            sip.IstoktoToplam = iskontotoplam;
            sip.ToplamTutar = toplamtutar - iskontotoplam;
            sip.AraToplam = toplamtutar;
            if (bayi.KDVDurum == true)
            {
                decimal kdv = ((toplamtutar - iskontotoplam) * 10) / 100;
                sip.KDVToplam = kdv;
                sip.ToplamTutar = sip.ToplamTutar + kdv;
            }
            else
            {
                sip.KDVToplam = 0;
            }
            c.SaveChanges();
        }
        [HttpPost]
        public IActionResult SYSSepetSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == id);
            de.Durum = false;

            var sip = c.Siparis.FirstOrDefault(v => v.ID == de.SiparisID);
            if (sip.OnayDurum == true)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == de.UrunID);
                urun.StokMiktari += de.Miktar;
                c.SaveChanges();
            }

            c.SaveChanges();
            siphesapla(Convert.ToInt32(de.SiparisID));
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SYSSepetGuncelle(SiparisIcerik s)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == s.ID);

            var sip = c.Siparis.FirstOrDefault(v => v.ID == de.SiparisID);
            if (sip.OnayDurum == true)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == de.UrunID);
                urun.StokMiktari += de.Miktar;
                urun.StokMiktari -= s.Miktar;
                c.SaveChanges();
            }
            if (s.Aciklama != null)
                de.Aciklama = s.Aciklama;
            if (s.BirimFiyat != null)
                de.BirimFiyat = s.BirimFiyat;
            if (s.Miktar != null)
                de.Miktar = s.Miktar;
            de.SatirToplam = de.BirimFiyat * de.Miktar;
            c.SaveChanges();
            siphesapla(Convert.ToInt32(de.SiparisID));
            result = new { status = "success", message = "Miktar Güncellendi..." };

            return Json(result);
        }
        [HttpPost]
        public IActionResult SYSSiparisOnay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.Siparis.FirstOrDefault(v => v.BayiID == id && v.BayiOnay == false);
            sip.BayiOnay = true;
            sip.SiparisDurum = "Onay Bekliyor...";
            var siparisadimturlari = c.SiparisAdimTurlaris.Where(v => v.Durum == true).OrderBy(v => v.ID).ToList();
            foreach (var x in siparisadimturlari)
            {
                SiparisAdimlari a = new SiparisAdimlari();
                a.SiparisAdimTurlariID = x.ID;
                a.SiparisID = sip.ID;
            }
            c.SaveChanges();
            result = new { status = "success", message = "Sepet Onaylandı... Sipariş Durumunu Siparişlerim Sekmesinden Kontrol Edebilirsiniz..." };
            return Json(result);
        }

        [HttpPost]
        public IActionResult GunList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.SiparisTarihi.Value >= DateTime.Today).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;

                var icerik = c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                int teslimmik = 0;
                foreach (var v in icerik)
                {
                    var tes = c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID);
                    if (tes.Durum == true)
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                        decimal? birimfiyat = 0;
                        if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; x.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; x.ParaBirimiID = 2; }
                        else { birimfiyat = 0; x.ParaBirimiID = 1; }

                        IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * x.IskontoOran) / 100);
                        AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                        teslimmik += Convert.ToInt32(v.Miktar);
                    }
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.ToplamTeslimTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KalanMiktar = (Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)) - teslimmik).ToString();
                list.KalanTutar = (Convert.ToDecimal(x.ToplamTutar) - ((AraToplam + KDVToplam) - IstoktoToplam)).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult HaftaList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.SiparisTarihi.Value > DateTime.Now.AddDays(-7)).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;

                var icerik = c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                int teslimmik = 0;
                foreach (var v in icerik)
                {
                    var tes = c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID);
                    if (tes.Durum == true)
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                        decimal? birimfiyat = 0;
                        if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; x.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; x.ParaBirimiID = 2; }
                        else { birimfiyat = 0; x.ParaBirimiID = 1; }

                        IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * x.IskontoOran) / 100);
                        AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                        teslimmik += Convert.ToInt32(v.Miktar);
                    }
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.ToplamTeslimTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KalanMiktar = (Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)) - teslimmik).ToString();
                list.KalanTutar = (Convert.ToDecimal(x.ToplamTutar) - ((AraToplam + KDVToplam) - IstoktoToplam)).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult AyList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.SiparisTarihi.Value.Month == DateTime.Now.Month).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;

                var icerik = c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                int teslimmik = 0;
                foreach (var v in icerik)
                {
                    var tes = c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID);
                    if (tes.Durum == true)
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                        decimal? birimfiyat = 0;
                        if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; x.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; x.ParaBirimiID = 2; }
                        else { birimfiyat = 0; x.ParaBirimiID = 1; }

                        IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * x.IskontoOran) / 100);
                        AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                        teslimmik += Convert.ToInt32(v.Miktar);
                    }
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.ToplamTeslimTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KalanMiktar = (Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)) - teslimmik).ToString();
                list.KalanTutar = (Convert.ToDecimal(x.ToplamTutar) - ((AraToplam + KDVToplam) - IstoktoToplam)).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult YilList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.SiparisTarihi.Value.Year == DateTime.Now.Year).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.KullaniciAdi.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;

                var icerik = c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                int teslimmik = 0;
                foreach (var v in icerik)
                {
                    var tes = c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID);
                    if (tes.Durum == true)
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                        decimal? birimfiyat = 0;
                        if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; x.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; x.ParaBirimiID = 2; }
                        else { birimfiyat = 0; x.ParaBirimiID = 1; }

                        IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * x.IskontoOran) / 100);
                        AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                        teslimmik += Convert.ToInt32(v.Miktar);
                    }
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.ToplamTeslimTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KalanMiktar = (Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)) - teslimmik).ToString();
                list.KalanTutar = (Convert.ToDecimal(x.ToplamTutar) - ((AraToplam + KDVToplam) - IstoktoToplam)).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }

        //Teslimat İşlemleri
        [HttpPost]
        public IActionResult SeciliTeslimatList(int id)
        {
            var teslimurun = c.TeslimatIceriks.Where(v => v.SiparisIcerikID == id && v.Durum == true).ToList();
            List<DtoTeslimat> tes = new List<DtoTeslimat>();

            foreach (var a in teslimurun)
            {
                var x = c.Teslimats.Where(v => v.ID == a.TeslimatID).FirstOrDefault();
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                DtoTeslimat te = new DtoTeslimat();
                te.ID = x.ID;
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString();
                }
                te.BayiID = bayiadi;
                te.TeslimatNo = x.TeslimatNo.ToString();
                if (x.TeslimatTarihi != null) te.TeslimatTarihi = Convert.ToDateTime(x.TeslimatTarihi).ToString("dd/MM/yyyy"); else te.TeslimatTarihi = "";
                te.KayitTarihi = Convert.ToDateTime(x.KayitTarihi).ToString("dd/MM/yyyy");
                te.Miktar = Convert.ToDecimal(c.TeslimatIceriks.Where(v => v.TeslimatID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                if (x.TamamlanmaDurum == true) te.TamamlanmaDurum = "Teslimat Tamamlandı."; else te.TamamlanmaDurum = "Teslimat Bekliyor.";
                tes.Add(te);
            }
            return Json(tes.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult TeslimatList()
        {
            var teslimatlar = c.Teslimats.Where(v => v.Durum == true).ToList();
            if (teslimatlar.Count() > 0)
            {
                List<DtoTeslimat> tes = new List<DtoTeslimat>();
                foreach (var x in teslimatlar)
                {
                    string bayiadi = "";
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    DtoTeslimat te = new DtoTeslimat();
                    te.ID = x.ID;
                    if (bayi != null)
                    {
                        if (bayi.KullaniciAdi != null) te.BayiID = bayi.KullaniciAdi.ToString();
                    }
                    if (x.AracPlaka != null) te.AracPlaka = x.AracPlaka.ToString();
                    te.KayitTarihi = Convert.ToDateTime(x.KayitTarihi).ToString("dd/MM/yyyy");
                    te.Miktar = Convert.ToDecimal(c.TeslimatIceriks.Where(v => v.TeslimatID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                    if (x.TamamlanmaDurum == true) te.TamamlanmaDurum = "Teslimat Tamamlandı."; else te.TamamlanmaDurum = "Teslimat Bekliyor.";
                    tes.Add(te);
                }
                return Json(tes.OrderByDescending(v => v.ID));
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult YukHazirList()
        {
            var veri = c.SiparisIceriks.Where(v => v.Durum == true && v.HazirAdet != null).ToList();
            List<DtoSiparisIcerik> icerikler = new List<DtoSiparisIcerik>();
            foreach (var x in veri)
            {
                if (x.HazirAdet > 0)
                {
                    int teslimmi = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true).Sum(v => v.Miktar));
                    if (x.TeslimAdet == 0 || x.TeslimAdet < teslimmi || x.TeslimAdet == null || x.Miktar > x.TeslimAdet)
                    {
                        var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                        if (sip.OnayDurum == true)
                        {
                            DtoSiparisIcerik list = new DtoSiparisIcerik();
                            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                            var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                            list.ID = Convert.ToInt32(x.ID);
                            list.UrunID = x.UrunID.ToString();
                            list.UrunKodu = urun.UrunKodu.ToString();
                            var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                            if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                            if (urun.Boyut != null)
                                list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                            string bayiadi = "";
                            if (bayi != null)
                            {
                                if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                                if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
                            }
                            list.BayiID = bayiadi;
                            list.SiparisID = sip.SiparisNo.ToString();
                            if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";
                            list.Miktar = Convert.ToInt32(x.Miktar).ToString();
                            list.HazirAdet = Convert.ToInt32(x.HazirAdet).ToString();
                            if (x.TeslimAdet != null) list.TeslimAdet = Convert.ToInt32(x.Miktar - x.TeslimAdet).ToString(); else list.TeslimAdet = "0";
                            string stozellik = "";
                            var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                            foreach (var v in ozellik)
                            {
                                var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                                if (o.UrunOzellikTurlariID == 6)
                                {
                                    if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                                    else list.DeriRengi = "";
                                }
                                else if (o.UrunOzellikTurlariID == 7)
                                {
                                    list.DeriRengi = "";
                                    if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                                    else list.AhsapRengi = "";
                                }
                                else
                                {
                                    list.AhsapRengi = "";
                                }
                            }
                            if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                            icerikler.Add(list);
                        }
                    }
                }
            }
            return Json(icerikler);
        }
        [HttpPost]
        public IActionResult TeslimatOlustur([FromBody] List<SelectedItem> selectedItems)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                List<DtoSiparisIcerik> list = new List<DtoSiparisIcerik>();
                foreach (var x in selectedItems)
                {
                    DtoSiparisIcerik i = new DtoSiparisIcerik();
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.Id);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == sipic.SiparisID);
                    if (sipic.HazirAdet >= Convert.ToInt32(x.ExtraInfo))
                    {
                        i.ID = sipic.ID;
                        i.Miktar = x.ExtraInfo;
                        i.SiparisID = sipic.SiparisID.ToString();
                        i.BayiID = sip.BayiID.ToString();
                        list.Add(i);
                    }
                    else
                    {
                        result = new { status = "error", message = "Seçilen Ürünler Arasında Hazır Olan Miktardan Büyük Adetler İşaretlendi!!! Lütfen Kontrol Edip Tekrar Deneyiniz..." };
                        return Json(result);
                    }
                }
                bool hepsiAyniMi = list.All(s => s.BayiID == list.First().BayiID);
                if (!hepsiAyniMi)
                {
                    result = new { status = "error", message = "Seçilen Ürünler Farklı Bayilere Ait Lütfen Seçilen Ürünleri Kontrol Ediniz..." };
                }
                else
                {
                    int bayino = Convert.ToInt32(list.FirstOrDefault().BayiID);
                    int siparisno = Convert.ToInt32(list.FirstOrDefault().SiparisID);
                    var aciktesvarmi = c.Teslimats.FirstOrDefault(v => v.BayiID == bayino && v.Durum == true && v.TamamlanmaDurum != true);
                    if (aciktesvarmi != null)
                    {
                        //Aktif Teslimat Formu Var Direkt Seçilenler İçine Aktarılacak
                        foreach (var x in list)
                        {
                            var varmi = c.TeslimatIceriks.FirstOrDefault(v => v.SiparisIcerikID == x.ID);
                            if (varmi == null)
                            {
                                var icerik = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.ID);
                                var birim = c.Urunlers.FirstOrDefault(v => v.ID == icerik.UrunID);
                                TeslimatIcerik t = new TeslimatIcerik();
                                t.SiparisIcerikID = x.ID;
                                t.SiparisID = icerik.SiparisID;
                                t.TeslimatID = aciktesvarmi.ID;
                                t.UrunID = icerik.UrunID;
                                t.BirimID = birim.BirimID;
                                t.Miktar = Convert.ToInt32(x.Miktar);
                                t.Durum = true;
                                c.TeslimatIceriks.Add(t);
                                c.SaveChanges();
                            }
                            else
                            {
                                varmi.Miktar = Convert.ToInt32(x.Miktar);
                                c.SaveChanges();
                            }
                        }
                        return Json(new { status = "success", message = "Açık Teslimat Formu Bulundu Ve Seçilen Ürünler Eklendi! Lütfen Teslimat Formunu Düzenlemeyi Unutmayınız.", redirectUrl = Url.Action("TeslimatDetay", "Siparis", new { id = aciktesvarmi.ID }) });
                    }
                    else
                    {
                        //Aktif Teslimat Yok Yenisi Oluşturulacak
                        Teslimat te = new Teslimat();
                        te.KayitTarihi = DateTime.Now;
                        te.KullaniciID = kulid;
                        te.SiparisID = siparisno;
                        te.Aciklama = "";
                        te.TeslimatNo = "";
                        te.TeslimEden = "";
                        te.TeslimAlan = "";
                        te.AracPlaka = "";
                        te.Durum = true;
                        te.BayiID = bayino;
                        te.TamamlanmaDurum = false;
                        c.Teslimats.Add(te);
                        c.SaveChanges();
                        var tes = c.Teslimats.OrderByDescending(v => v.ID).FirstOrDefault();
                        foreach (var x in list)
                        {
                            var icerik = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.ID);
                            var birim = c.Urunlers.FirstOrDefault(v => v.ID == icerik.UrunID);
                            TeslimatIcerik t = new TeslimatIcerik();
                            t.SiparisIcerikID = x.ID;
                            t.SiparisID = icerik.SiparisID;
                            t.TeslimatID = tes.ID;
                            t.UrunID = icerik.UrunID;
                            t.BirimID = birim.BirimID;
                            t.Miktar = Convert.ToInt32(x.Miktar);
                            t.Durum = true;
                            c.TeslimatIceriks.Add(t);
                            c.SaveChanges();
                        }
                        return Json(new { status = "success", message = "Yeni Teslimat Formu Oluşturuldu Ve Seçilen Ürünler Eklendi! Lütfen Teslimat Formunu Düzenlemeyi Unutmayınız.", redirectUrl = Url.Action("TeslimatDetay", "Siparis", new { id = tes.ID }) });
                    }
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult TeslimatIcerik(int id)
        {
            var tes = c.Teslimats.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == tes.BayiID);
            string para = "";
            if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
            if (tes != null)
            {
                var veri = c.TeslimatIceriks.Where(v => v.TeslimatID == tes.ID && v.Durum == true).ToList();
                List<DtoTeslimatIcerik> ham = new List<DtoTeslimatIcerik>();
                foreach (var x in veri)
                {
                    var sippic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    DtoTeslimatIcerik list = new DtoTeslimatIcerik();
                    list.ID = Convert.ToInt32(x.ID);
                    string bayiadi = "";
                    if (bayi != null)
                    {
                        if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                        if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
                    }
                    list.BayiID = bayiadi;
                    list.SiparisNo = sip.SiparisNo.ToString();
                    list.UrunID = x.UrunID.ToString();
                    if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "";
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString() + " / " + urun.UrunAciklama.ToString(); else list.UrunAciklama = "";
                    if (urun.Boyut != null) list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    if (sip != null) list.SiparisID = sip.SiparisNo.ToString();
                    if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";
                    list.SiparisMiktari = sipic.Miktar.ToString();
                    list.Miktar = x.Miktar.ToString();
                    list.HazirMiktar = sipic.HazirAdet.ToString();
                    list.KalanMiktar = (sipic.Miktar - sipic.TeslimAdet).ToString();
                    if (bayi.IskontoOran > 0)
                    {
                        if (sippic.SatirToplam != null && sippic.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal(((sippic.BirimFiyat * bayi.IskontoOran) / 100) * x.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    }
                    else
                    {
                        if (sippic.SatirToplam != null && sippic.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((sippic.BirimFiyat) * x.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    }
                    if (sippic.BirimFiyat != null && sippic.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(sippic.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                    if (sippic.BirimFiyat != null && sippic.BirimFiyat > 0) list.indirimFiyat = Convert.ToDecimal((sippic.BirimFiyat * bayi.IskontoOran) / 100).ToString("N2") + para; else list.indirimFiyat = "0,00" + para;
                    string stozellik = "";
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == sipic.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6)
                        {
                            if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                            else list.DeriRengi = "";
                        }
                        else if (o.UrunOzellikTurlariID == 7)
                        {
                            list.DeriRengi = "";
                            if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                            else list.AhsapRengi = "";
                        }
                        else
                        {
                            list.AhsapRengi = "";
                        }
                    }
                    if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                    if (sipic.Aciklama != null) if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = ""; else list.Aciklama = "";
                    ham.Add(list);
                }
                return Json(ham.OrderBy(v => v.ID));
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult TeslimatIcerikSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            TeslimatIcerik de = c.TeslimatIceriks.FirstOrDefault(v => v.ID == id);
            de.Durum = false;
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult TeslimatIcerikDuzenle(TeslimatIcerik s)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            TeslimatIcerik de = c.TeslimatIceriks.FirstOrDefault(v => v.ID == s.ID);
            if (s.Miktar != null)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == de.SiparisIcerikID);
                if (((sipic.HazirAdet - sipic.TeslimAdet) - s.Miktar) == 0 || ((sipic.HazirAdet - sipic.TeslimAdet) - s.Miktar) > 0)
                {
                    de.Miktar = s.Miktar;
                    c.SaveChanges();
                    result = new { status = "success", message = "Miktar Güncellendi..." };
                }
                else result = new { status = "errror", message = "Teslim Edilecek Miktarı Kontrol Ediniz!!!" };
            }
            else
                result = new { status = "errror", message = "Miktar Boş Geçilemez..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult TeslimatDetayBilgi(int id)
        {
            var x = c.Teslimats.FirstOrDefault(v => v.ID == id);
            if (x != null)
            {
                var sip = c.Siparis.FirstOrDefault(a => a.ID == x.SiparisID);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (sip.ParaBirimiID == 2) para = " $"; else para = " ₺";
                DtoTeslimat list = new DtoTeslimat();
                list.ID = Convert.ToInt32(x.ID);
                if (x.TeslimatTarihi != null) list.TeslimatTarihi = Convert.ToDateTime(x.TeslimatTarihi).ToString("dd/MM/yyyy"); else list.TeslimatTarihi = "";
                if (x.AracPlaka != null) list.AracPlaka = x.AracPlaka.ToString(); else list.AracPlaka = "";
                if (x.TeslimAlan != null) list.TeslimAlan = x.TeslimAlan.ToString(); else list.TeslimAlan = "";
                if (x.TeslimEden != null) list.TeslimEden = x.TeslimEden.ToString(); else list.TeslimEden = "";
                if (x.TeslimatNo != null) list.TeslimatNo = x.TeslimatNo.ToString(); else list.TeslimatNo = "";
                if (x.TeslimSekli != null) list.TeslimSekli = x.TeslimSekli.ToString(); else list.TeslimSekli = "";
                if (x.TeslimAdres != null) list.TeslimAdres = x.TeslimAdres.ToString(); else list.TeslimAdres = "";
                if (x.KullaniciID != null) list.KullaniciID = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).AdSoyad.ToString(); else list.KullaniciID = "";

                if (bayi.Unvan != null) list.FaturaUnvan = bayi.Unvan.ToString(); else list.FaturaUnvan = "";
                if (bayi.KullaniciAdi != null) list.MusteriAdi = bayi.KullaniciAdi.ToString(); else list.MusteriAdi = "";
                if (bayi.Telefon != null) list.Telefon = bayi.Telefon.ToString(); else list.Telefon = "";
                if (bayi.Adres != null) list.Adres = bayi.Adres.ToString(); else list.Adres = "";
                if (x.TamamlanmaDurum == true) list.TamamlanmaDurum = "Tamamlandi"; else list.TamamlanmaDurum = "Bekliyor";

                var icerik = c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).ToList();
                decimal toplamm3 = 0;
                decimal toplamkg = 0;
                decimal toplampaketadet = 0;
                decimal indirimlifiyat = 0;
                decimal KDVToplam = 0;
                decimal ToplamTutar = 0;
                decimal AraToplam = 0;
                decimal IstoktoToplam = 0;
                decimal teslimmiktar = Convert.ToDecimal(c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).Sum(v => v.Miktar));
                foreach (var v in icerik)
                {
                    var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                    decimal? birimfiyat = 0;
                    if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                    else { birimfiyat = 0; sip.ParaBirimiID = 1; }


                    var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                    if (urun.BirimKG != null) toplamkg += Convert.ToDecimal(urun.BirimKG * v.Miktar);
                    if (urun.BirimM3 != null) toplamm3 += Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                    if (urun.PaketAdet != null) toplampaketadet += Convert.ToDecimal(urun.PaketAdet * v.Miktar);


                    IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * sip.IskontoOran) / 100);
                    AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
                }
                if (bayi.KDVDurum == true)
                {
                    decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                    KDVToplam += kdv;
                }
                else
                {
                    KDVToplam = 0;
                }
                list.indirimlifiyat = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.KDVToplam = KDVToplam.ToString("N2") + para;
                list.ToplamTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
                list.AraToplam = AraToplam.ToString("N2") + para;
                list.IstoktoToplam = IstoktoToplam.ToString("N2") + para;

                list.ToplamM3 = toplamm3.ToString("N2");
                list.ToplamKG = toplamkg.ToString("N2");
                list.ToplamParca = toplampaketadet.ToString("N2");
                list.ToplamAdet = teslimmiktar.ToString();

                list.DosyaYolu = x.DosyaYolu;

                return Json(list);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult TeslimatGuncelle(Teslimat t)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (t.AracPlaka != null && t.TeslimAlan != null && t.TeslimatNo != null)
            {
                var tes = c.Teslimats.FirstOrDefault(v => v.ID == t.ID);
                if (tes.TamamlanmaDurum != true)
                {
                    tes.AracPlaka = t.AracPlaka;
                    if (t.TeslimatTarihi != null) tes.TeslimatTarihi = t.TeslimatTarihi; else tes.TeslimatTarihi = DateTime.Now;
                    tes.TeslimatNo = t.TeslimatNo;
                    tes.TeslimAlan = t.TeslimAlan;
                    c.SaveChanges();
                    result = new { status = "success", message = "Teslimat Bilgileri Güncellendi..." };
                }
                else
                {
                    result = new { status = "error", message = "Teslimat Zaten Gerçekleştirilmiş Yeni Tekrar Belge Kaydı Rakamlarda Karışıklığa Yol Açacaktır... Belge Kaydedilmedi..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Gerekli Alanları Boş Bırakmayınız..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult TeslimatSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            Teslimat de = c.Teslimats.FirstOrDefault(v => v.ID == id);
            de.Durum = false;
            var icerik = c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).ToList();
            foreach (var x in icerik)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                sipic.TeslimAdet -= Convert.ToInt32(x.Miktar);
                x.Durum = false;
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == sipic.UrunID);
                urun.StokMiktari += Convert.ToInt32(x.Miktar);
                c.SaveChanges();
            }
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> TeslimatOnayla(int id, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var tes = c.Teslimats.FirstOrDefault(v => v.ID == id);

            if (tes.AracPlaka == null || tes.AracPlaka.Length <= 0 || tes.TeslimAlan == null || tes.TeslimAlan.Length <= 0 || tes.TeslimatNo == null || tes.TeslimatNo.Length <= 0)
            {
                TeslimatHata.Icerik = "TESLİMAT BİLGİLERİ GİRİLMEDİ LÜTFEN KONTROL EDİNİZ!!!";
                return RedirectToAction("Teslimatlar", "Siparis");
            }

            if (imagee != null)
            {

                var dosyaAdi = Path.GetFileName(imagee.FileName);

                var dosyaYolu = Path.Combine("wwwroot/Evraklar/TeslimatEvraklar", dosyaAdi);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await imagee.CopyToAsync(stream);
                }
                var sifiryok = c.TeslimatIceriks.Where(v => v.TeslimatID == tes.ID && v.Durum == true && v.Miktar == null && v.Miktar <= 0).FirstOrDefault();
                if (sifiryok == null)
                {
                    if (tes.TamamlanmaDurum != true)
                    {
                        var icerik = c.TeslimatIceriks.Where(v => v.TeslimatID == tes.ID && v.Durum == true).ToList();
                        foreach (var x in icerik)
                        {
                            var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                            var s = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                            s.TeslimAdet += Convert.ToInt32(x.Miktar);
                            s.HazirAdet -= Convert.ToInt32(x.Miktar);
                            sip.ToplamTeslimEdilen += Convert.ToInt32(x.Miktar);
                            if (s.Miktar == s.TeslimAdet)
                            {
                                s.TeslimDurum = true;
                            }
                            c.SaveChanges();

                            var sevkiseirleri = c.SevkiyatIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == x.SiparisIcerikID && v.Durum == true);
                            sevkiseirleri.KalanAdet -= Convert.ToInt32(x.Miktar);
                            //sevkiseirleri.GidenAdet += Convert.ToInt32(x.Miktar);
                            sevkiseirleri.KullaniciID = kulid;
                            if (sevkiseirleri.GidenAdet == sevkiseirleri.GelenAdet)
                            {
                                sevkiseirleri.BitirmeDurum = true;
                                sevkiseirleri.BitisTarihi = DateTime.Now;
                            }
                            c.SaveChanges();

                            Formuller f = new Formuller(c);
                            f.SiparisSonMu(Convert.ToInt32(x.SiparisID));
                        }

                        tes.DosyaYolu = dosyaYolu.Substring(7);
                        tes.TamamlanmaDurum = true;
                        c.SaveChanges();
                        TeslimatHata.Icerik = "Teslimat Kaydı Başarılı...";
                    }
                }
                else
                {
                    TeslimatHata.Icerik = "Lütfen Teslim Edilecek Ürünlerin Miktar Alanlarını Boş Bırakmayınız...";
                }
            }
            else
            {
                TeslimatHata.Icerik = "Onay Evrağını Eklemeden Teslimatı Onaylayamazsınız...";
            }
            return RedirectToAction("Teslimatlar", "Siparis");
        }
        public class SelectedItem
        {
            public int Id { get; set; }
            public string ExtraInfo { get; set; }
        }

    }
}