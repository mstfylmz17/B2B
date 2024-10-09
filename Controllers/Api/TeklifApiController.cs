using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using static VNNB2B.Controllers.Api.UrunApiController;
using VNNB2B.Models;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers.Api
{
    public class TeklifApiController : Controller
    {
        private readonly Context c;
        public TeklifApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult TeklifList()
        {
            var veri = c.Teklifs.OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.BayiID = bayi.Unvan.ToString();
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.TeklifIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                ham.Add(list);
            }
            return Json(ham);
        }
        public IActionResult SiparisYeniOnay(int id)
        {
            var sip = c.Teklifs.FirstOrDefault(v => v.ID == id);
            sip.Durum = true;
            c.SaveChanges();
            TeklifHata.Icerik = "Teklif Kaydedildi...";
            return RedirectToAction("Index", "Teklif");
        }
        [HttpPost]
        public IActionResult SiparisDetay(int id)
        {
            var sip = c.Teklifs.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            string para = "";
            if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
            if (sip != null)
            {
                var veri = c.TeklifIceriks.Where(v => v.SiparisID == sip.ID && v.Durum == true).ToList();
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
                    var ozellik = c.TeklifIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
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
            var x = c.Teklifs.FirstOrDefault(v => v.ID == id);
            if (x != null)
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                string para = "";
                if (x.ParaBirimiID == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";

                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
                list.SiparisBayiAciklama = x.SiparisBayiAciklama.ToString();
                if (x.TerminTarihi != null) list.TerminTarihi = Convert.ToDateTime(x.TerminTarihi).ToString("dd/MM/yyyy"); else list.TerminTarihi = "HENÜZ BELİRTİLMEDİ / NOT YET SPECIFIED";
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                list.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
                list.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
                list.DosyaYolu = x.DosyaYolu;
                list.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
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
                var icerik = c.TeklifIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
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
        public IActionResult SYSSepetEkle([FromBody] SepetEkleModel model)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Bayilers.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var sip = c.Teklifs.FirstOrDefault(v => v.ID == model.SiparisID);
                int sipno = sip.ID;
                var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == model.ID && v.Durum == true);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                decimal? birimfiyat = 0;
                if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                else { birimfiyat = 0; sip.ParaBirimiID = 1; }
                var varmi = c.TeklifIceriks.FirstOrDefault(v => v.UrunID == model.ID && v.SiparisID == sipno && v.Durum == true);
                if (varmi == null)
                {
                    TeklifIcerik i = new TeklifIcerik();
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
                    c.TeklifIceriks.Add(i);
                    c.SaveChanges();
                    var sipicid = c.TeklifIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

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
                        TeklifIcerikUrunOzellikleri so = new TeklifIcerikUrunOzellikleri();
                        so.SiaprisIcerikID = sipicid.ID;
                        so.UrunID = model.ID;
                        if (stokdurum == true)
                        {
                            so.UrunStoklariID = stokid;
                        }
                        so.UrunAltOzellikID = ozellikid;
                        so.Durum = true;
                        c.TeklifIcerikUrunOzellikleris.Add(so);
                        c.SaveChanges();
                    }
                }
                else
                {
                    bool aynisi = false;
                    var sipoz = c.TeklifIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == varmi.ID && v.Durum == true).ToList();
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
                        TeklifIcerik i = new TeklifIcerik();
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
                        c.TeklifIceriks.Add(i);
                        c.SaveChanges();
                        var sipicid = c.TeklifIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

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
                            TeklifIcerikUrunOzellikleri so = new TeklifIcerikUrunOzellikleri();
                            so.SiaprisIcerikID = sipicid.ID;
                            so.UrunID = model.ID;
                            if (stokdurum == true)
                            {
                                so.UrunStoklariID = stokid;
                            }
                            so.UrunAltOzellikID = ozellikid;
                            so.Durum = true;
                            c.TeklifIcerikUrunOzellikleris.Add(so);
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
            var sip = c.Teklifs.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            decimal? bayioran = sip.IskontoOran;
            decimal toplamtutar = 0;
            decimal toplamadet = 0;
            decimal iskontotoplam = 0;

            toplamtutar = Convert.ToDecimal(c.TeklifIceriks.Where(v => v.SiparisID == id && v.Durum == true).Sum(v => v.SatirToplam));
            toplamadet = Convert.ToDecimal(c.TeklifIceriks.Where(v => v.SiparisID == id && v.Durum == true).Sum(v => v.Miktar));
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
            TeklifIcerik de = c.TeklifIceriks.FirstOrDefault(v => v.ID == id);
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
        public IActionResult SYSSepetGuncelle(TeklifIcerik s)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            TeklifIcerik de = c.TeklifIceriks.FirstOrDefault(v => v.ID == s.ID);
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
            var sip = c.Teklifs.FirstOrDefault(v => v.BayiID == id && v.BayiOnay == false);
            sip.BayiOnay = true;
            sip.SiparisDurum = "Onay Bekliyor...";
            c.SaveChanges();
            result = new { status = "success", message = "Sepet Onaylandı... Teklif Durumunu Siparişlerim Sekmesinden Kontrol Edebilirsiniz..." };
            return Json(result);
        }
    }
}
