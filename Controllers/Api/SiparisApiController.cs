using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                    if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "Açıklama Yok!";
                    if (x.Miktar != null) list.Miktar = Convert.ToInt32(x.Miktar).ToString(); else list.Miktar = "1";
                    if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal(x.SatirToplam).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6) list.DeriRengi = o.OzellikAdi.ToString(); else if (o.UrunOzellikTurlariID == 7) { if (o != null) list.AhsapRengi = o.OzellikAdi.ToString(); else list.AhsapRengi = ""; }
                    }
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
                    i.Durum = true;
                    if (bayi.KDVDurum == true)
                    {
                        decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                        i.KDVTutari = kdv;
                        i.SatirToplam = i.SatirToplam + kdv;
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
                            varmi.SatirToplam = varmi.SatirToplam + kdv;
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
                            i.SatirToplam = i.SatirToplam + kdv;
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
            {
                de.Miktar = s.Miktar;
                de.SatirToplam = de.BirimFiyat * s.Miktar;
                c.SaveChanges();
                Formuller form = new Formuller(c);
                form.siphesapla(Convert.ToInt32(de.SiparisID));
                result = new { status = "success", message = "Miktar Güncellendi..." };
            }
            else
                result = new { status = "errror", message = "Miktar Boş Geçilemez..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SiparisOnay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
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
        public IActionResult SiparisList(int id)
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.BayiID == id && v.BayiOnay == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
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
            var veri = c.Siparis.Where(v => v.Durum == true && v.BayiID == id && v.BayiOnay == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.ID).Take(10));
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
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                list.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
                list.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
                list.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
                if (x.OnaylayanID != null)
                    list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.OnaylayanID).AdSoyad.ToString();
                else
                    list.Kullanici = "";
                list.indirimlifiyat = Convert.ToDecimal(x.AraToplam - x.IstoktoToplam).ToString("N2") + para;
                var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
                decimal toplamm3 = 0;
                decimal toplamkg = 0;
                decimal toplampaketadet = 0;
                foreach (var v in icerik)
                {
                    var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                    if (urun.BirimKG != null) toplamkg = Convert.ToDecimal(urun.BirimKG * v.Miktar);
                    if (urun.BirimM3 != null) toplamm3 = Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                    if (urun.PaketAdet != null) toplampaketadet = Convert.ToDecimal(urun.PaketAdet * v.Miktar);
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
            int x = c.SiparisIceriks.Where(v => v.SiparisID == sepet.ID && v.Durum == true).Count();
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
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
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
                list.BayiID = bayi.Unvan.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
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
        public async Task<IActionResult> SiparisOnayla(int id, string OnayAciklama, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            if (sip.BayiOnay != true)
            {
                result = new { status = "error", message = "Bayi Henüz Siparişi Onaylamamış..." };
                return Json(result);
            }


            if (imagee != null)
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
                sip.OnayAciklama = OnayAciklama;

                Formuller formuller = new Formuller(c);
                formuller.SiparisOnay(id, kulid);

                c.SaveChanges();
                SiparisHata.Icerik = "Sipariş Onaylandı...";
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
                list.BayiID = bayi.Unvan.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
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
                list.BayiID = bayi.Unvan.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
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
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true && v.ToplamTeslimEdilen > 0).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.Unvan.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
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
        public IActionResult YHList()
        {
            var veri = c.SiparisIceriks.Where(v => v.Durum == true && v.YuklemeyeHazir == true).ToList();
            if (veri.Count() > 0)
            {
                List<DtoSiparisIcerik> icerikler = new List<DtoSiparisIcerik>();
                foreach (var x in veri)
                {
                    int teslimmi = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true).Sum(v => v.Miktar));
                    if (x.TeslimAdet == 0 || x.TeslimAdet < teslimmi)
                    {
                        var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                        if (sip.OnayDurum == true)
                        {
                            DtoSiparisIcerik list = new DtoSiparisIcerik();
                            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                            var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                            list.ID = Convert.ToInt32(x.ID);
                            if (urun != null)
                            {
                                list.UrunID = "";
                                if (urun.UrunKodu != null) list.UrunID = urun.UrunKodu.ToString() + " / ";
                                if (urun.UrunAdi != null) list.UrunID += urun.UrunAdi.ToString();
                            }
                            string bayiadi = "";
                            if (bayi != null)
                            {
                                if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                                if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                            }
                            list.BayiID = bayiadi;
                            list.SiparisID = sip.SiparisNo.ToString();
                            if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("d"); else list.SiparisTarihi = "";
                            list.Miktar = x.Miktar.ToString();
                            if (x.TeslimAdet != null) list.TeslimAdet = x.TeslimAdet.ToString(); else list.TeslimAdet = "0";
                            var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                            foreach (var v in ozellik)
                            {
                                var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                                if (o.UrunOzellikTurlariID == 6) list.DeriRengi = o.OzellikAdi.ToString(); else if (o.UrunOzellikTurlariID == 7) { if (o != null) list.AhsapRengi = o.OzellikAdi.ToString(); else list.AhsapRengi = ""; }
                            }
                            if (list.DeriRengi != null) list.Aciklama += "Deri Rengi - " + list.DeriRengi.ToString() + " / ";
                            if (list.AhsapRengi != null) list.Aciklama += "Ahşap Rengi - " + list.AhsapRengi.ToString() + " / ";
                            if (x.Aciklama != null) list.Aciklama += "Not (Açıklama) - " + x.Aciklama.ToString();
                            icerikler.Add(list);
                        }
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
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.Unvan.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
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
                    if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                    if (x.Miktar != null) list.Miktar = Convert.ToInt32(x.Miktar).ToString(); else list.Miktar = "1";
                    if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal(x.SatirToplam).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6) list.DeriRengi = o.OzellikAdi.ToString(); else if (o.UrunOzellikTurlariID == 7) { if (o != null) list.AhsapRengi = o.OzellikAdi.ToString(); else list.AhsapRengi = ""; }
                    }
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
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                list.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
                list.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
                list.DosyaYolu = x.DosyaYolu;
                list.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
                if (x.OnaylayanID != null)
                    list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.OnaylayanID).AdSoyad.ToString();
                else
                    list.Kullanici = "";
                list.indirimlifiyat = Convert.ToDecimal(x.AraToplam - x.IstoktoToplam).ToString("N2") + para;
                if (bayi.Yetkili != null) list.Yetkili = bayi.Yetkili.ToString(); else list.Yetkili = "";
                if (bayi.Unvan != null) list.Unvan = bayi.Unvan.ToString(); else list.Unvan = "";
                if (bayi.KullaniciAdi != null) list.FirmaAdi = bayi.KullaniciAdi.ToString(); else list.FirmaAdi = "";
                if (bayi.Telefon != null) list.Telefon = bayi.Telefon.ToString(); else list.Telefon = "";
                if (bayi.Adres != null) list.Adres = bayi.Adres.ToString(); else list.Adres = "";
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
        public IActionResult SiparisYeniOnay(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
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
                    if (bayi.KDVDurum == true)
                    {
                        decimal kdv = Convert.ToDecimal((i.SatirToplam * 10) / 100);
                        i.KDVTutari = kdv;
                        i.SatirToplam = i.SatirToplam + kdv;
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
                            varmi.SatirToplam = varmi.SatirToplam + kdv;
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
                            i.SatirToplam = i.SatirToplam + kdv;
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
                decimal kdv = (toplamtutar * 10) / 100;
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
            //int stokid = 0;
            //var stok = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == id && v.Durum == true).ToList();
            //foreach (var x in stok)
            //{
            //    if (x.UrunStoklariID != null) stokid = Convert.ToInt32(x.UrunStoklariID);
            //}
            //if (stokid > 0) { var stokbul = c.UrunStoklaris.FirstOrDefault(v => v.ID == stokid).StokMiktari += de.Miktar; c.SaveChanges(); }
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
            if (s.Miktar != null)
            {
                de.Miktar = s.Miktar;
                de.SatirToplam = de.BirimFiyat * s.Miktar;
                c.SaveChanges();
                Formuller form = new Formuller(c);
                form.siphesapla(Convert.ToInt32(de.SiparisID));
                result = new { status = "success", message = "Miktar Güncellendi..." };
            }
            else
                result = new { status = "errror", message = "Miktar Boş Geçilemez..." };
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

        //Teslimat İşlemleri
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
                    DtoTeslimat te = new DtoTeslimat();
                    te.ID = x.ID;
                    if (bayi != null)
                    {
                        if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                        if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                    }
                    te.BayiID = bayiadi;
                    te.TeslimatNo = x.TeslimatNo.ToString();
                    te.KayitTarihi = Convert.ToDateTime(x.KayitTarihi).ToString("d");
                    te.Miktar = Convert.ToDecimal(c.TeslimatIceriks.Where(v => v.TeslimatID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                    if (x.TamamlanmaDurum == true) te.TamamlanmaDurum = "Teslimat Tamamlandı."; else te.TamamlanmaDurum = "Teslimat Bekliyor.";
                    tes.Add(te);
                }
                return Json(tes.OrderBy(v => v.ID));
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
            if (veri.Count() > 0)
            {
                List<DtoSiparisIcerik> icerikler = new List<DtoSiparisIcerik>();
                foreach (var x in veri)
                {
                    if (x.HazirAdet > 0)
                    {
                        int teslimmi = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisIcerikID == x.ID && v.Durum == true).Sum(v => v.Miktar));
                        if (x.TeslimAdet == 0 || x.TeslimAdet < teslimmi)
                        {
                            var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                            if (sip.OnayDurum == true)
                            {
                                DtoSiparisIcerik list = new DtoSiparisIcerik();
                                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                                list.ID = Convert.ToInt32(x.ID);
                                if (urun != null)
                                {
                                    list.UrunID = "";
                                    if (urun.UrunKodu != null) list.UrunID = urun.UrunKodu.ToString() + " / ";
                                    if (urun.UrunAdi != null) list.UrunID += urun.UrunAdi.ToString();
                                }
                                string bayiadi = "";
                                if (bayi != null)
                                {
                                    if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                                    if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                                }
                                list.BayiID = bayiadi;
                                list.SiparisID = sip.SiparisNo.ToString();
                                if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("d"); else list.SiparisTarihi = "";
                                list.Miktar = x.Miktar.ToString();
                                if (x.TeslimAdet != null) list.TeslimAdet = x.TeslimAdet.ToString(); else list.TeslimAdet = "0";
                                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                                foreach (var v in ozellik)
                                {
                                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                                    if (o.UrunOzellikTurlariID == 6) list.DeriRengi = o.OzellikAdi.ToString(); else if (o.UrunOzellikTurlariID == 7) { if (o != null) list.AhsapRengi = o.OzellikAdi.ToString(); else list.AhsapRengi = ""; }
                                }
                                if (list.DeriRengi != null) list.Aciklama += "Deri Rengi - " + list.DeriRengi.ToString() + " / ";
                                if (list.AhsapRengi != null) list.Aciklama += "Ahşap Rengi - " + list.AhsapRengi.ToString() + " / ";
                                if (x.Aciklama != null) list.Aciklama += "Not (Açıklama) - " + x.Aciklama.ToString();
                                icerikler.Add(list);
                            }
                        }
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
        public IActionResult TeslimatOlustur([FromBody] List<int> selectedIds)
        {
            decimal kur = 0;
            decimal kureuro = 0;
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                List<DtoSiparisIcerik> list = new List<DtoSiparisIcerik>();
                foreach (var x in selectedIds)
                {
                    DtoSiparisIcerik i = new DtoSiparisIcerik();
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == sipic.SiparisID);
                    i.ID = sipic.ID;
                    i.SiparisID = sipic.SiparisID.ToString();
                    i.BayiID = sip.BayiID.ToString();
                    list.Add(i);
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
                            var icerik = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.ID);
                            var birim = c.Urunlers.FirstOrDefault(v => v.ID == icerik.UrunID);
                            TeslimatIcerik t = new TeslimatIcerik();
                            t.SiparisIcerikID = x.ID;
                            t.SiparisID = icerik.SiparisID;
                            t.TeslimatID = aciktesvarmi.ID;
                            t.UrunID = icerik.UrunID;
                            t.BirimID = birim.BirimID;
                            t.Miktar = 0;
                            t.Durum = true;
                            c.TeslimatIceriks.Add(t);
                            c.SaveChanges();
                        }
                        return Json(new { status = "success", message = "Açık Teslimat Formu Bulundu Ve Seçilen Ürünler Eklendi! Lütfen Teslimat Formunu Düzenlemeyi Unutmayınız. Teslimat No -->" + aciktesvarmi.TeslimatNo.ToString(), redirectUrl = Url.Action("TeslimatDetay", "Siparis", new { id = aciktesvarmi.ID }) });
                    }
                    else
                    {
                        //Aktif Teslimat Yok Yenisi Oluşturulacak
                        Teslimat te = new Teslimat();
                        te.KayitTarihi = DateTime.Now;
                        te.KullaniciID = kulid;
                        te.SiparisID = siparisno;
                        te.Aciklama = "";
                        te.TeslimatNo = "STE - " + (c.Teslimats.Count() + 1).ToString();
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
                            t.Miktar = 0;
                            t.Durum = true;
                            c.TeslimatIceriks.Add(t);
                            c.SaveChanges();
                        }
                        return Json(new { status = "success", message = "Yeni Teslimat Formu Oluşturuldu Ve Seçilen Ürünler Eklendi! Lütfen Teslimat Formunu Düzenlemeyi Unutmayınız. Teslimat No -->" + tes.TeslimatNo.ToString(), redirectUrl = Url.Action("TeslimatDetay", "Siparis", new { id = tes.ID }) });
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
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    DtoTeslimatIcerik list = new DtoTeslimatIcerik();
                    list.ID = Convert.ToInt32(x.ID);
                    if (urun.UrunKodu != null) list.UrunID = urun.UrunKodu.ToString(); else list.UrunID = "";
                    if (urun.UrunAdi != null) list.UrunID = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunID = "";
                    if (urun.Boyut != null) list.UrunID += " <br/> " + urun.Boyut.ToString();
                    if (sip != null) list.SiparisID = sip.SiparisNo.ToString();
                    list.SiparisMiktari = sipic.Miktar.ToString();
                    list.Miktar = x.Miktar.ToString();
                    list.KalanMiktar = (sipic.Miktar - x.Miktar).ToString();
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6) list.DeriRengi = o.OzellikAdi.ToString(); else if (o.UrunOzellikTurlariID == 7) { if (o != null) list.AhsapRengi = o.OzellikAdi.ToString(); else list.AhsapRengi = ""; }
                    }
                    if (list.DeriRengi != null) list.Aciklama += "Deri Rengi - " + list.DeriRengi.ToString() + " / ";
                    if (list.AhsapRengi != null) list.Aciklama += "Ahşap Rengi - " + list.AhsapRengi.ToString() + " / ";
                    if (sipic.Aciklama != null) list.Aciklama += "Not (Açıklama) - " + sipic.Aciklama.ToString();
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
                if ((sipic.Miktar - s.Miktar) == 0 || (sipic.Miktar - s.Miktar) > 0)
                {
                    de.Miktar = s.Miktar;
                    c.SaveChanges();
                    result = new { status = "success", message = "Miktar Güncellendi..." };
                }
                else result = new { status = "errror", message = "Teslim Edilecek Miktar Sipariş Edilen Miktardan Fazla Olamaz!!!" };
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
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                DtoTeslimat list = new DtoTeslimat();
                list.ID = Convert.ToInt32(x.ID);
                if (x.TeslimatTarihi != null) list.TeslimatTarihi = Convert.ToDateTime(x.TeslimatTarihi).ToString("d"); else list.TeslimatTarihi = "";
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

                var icerik = c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).ToList();
                decimal toplamm3 = 0;
                decimal toplamkg = 0;
                decimal toplampaketadet = 0;
                decimal teslimmiktar = Convert.ToDecimal(c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).Sum(v => v.Miktar));
                foreach (var v in icerik)
                {
                    var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                    if (urun.BirimKG != null) toplamkg += Convert.ToDecimal(urun.BirimKG * v.Miktar);
                    if (urun.BirimM3 != null) toplamm3 += Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                    if (urun.PaketAdet != null) toplampaketadet += Convert.ToDecimal(urun.PaketAdet * v.Miktar);
                }
                list.ToplamM3 = toplamm3.ToString("N2");
                list.ToplamKG = toplamkg.ToString("N2");
                list.ToplamParca = toplampaketadet.ToString("N2");
                list.ToplamAdet = teslimmiktar.ToString();
                return Json(list);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult TeslimOnayla(Teslimat t)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (t.AracPlaka != null && t.TeslimAlan != null && t.TeslimEden != null && t.TeslimAdres != null && t.TeslimSekli != null)
            {
                var tes = c.Teslimats.FirstOrDefault(v => v.ID == t.ID);
                tes.TamamlanmaDurum = true;
                tes.AracPlaka = t.AracPlaka;
                if (t.TeslimatTarihi != null) tes.TeslimatTarihi = t.TeslimatTarihi; else tes.TeslimatTarihi = DateTime.Now;
                tes.TeslimEden = t.TeslimEden;
                tes.TeslimAlan = t.TeslimAlan;
                tes.TeslimSekli = t.TeslimSekli;
                tes.TeslimAdres = t.TeslimAdres;
                var icerik = c.TeslimatIceriks.Where(v => v.TeslimatID == t.ID && v.Durum == true).ToList();
                foreach (var x in icerik)
                {
                    var s = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    s.TeslimAdet = Convert.ToInt32(x.Miktar);
                    if (s.Miktar == s.TeslimAdet)
                    {
                        s.TeslimDurum = true;
                    }
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == s.SiparisID).ToplamTeslimEdilen = c.SiparisIceriks.Where(v => v.SiparisID == s.SiparisID && v.Durum == true).Sum(v => v.TeslimAdet);
                    c.SaveChanges();
                }
                Formuller f = new Formuller(c);
                f.SiparisSonMu(Convert.ToInt32(tes.SiparisID));
                c.SaveChanges();
                result = new { status = "success", message = "Teslimat Kaydı Başarılı..." };
            }
            else
            {
                result = new { status = "error", message = "Lütfen Gerekli Alanları Boş Bırakmayınız..." };
            }
            return Json(result);
        }
    }
}