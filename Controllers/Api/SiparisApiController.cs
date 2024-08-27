using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VNNB2B.Models;
using VNNB2B.Models.Hata;

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
        public IActionResult SepetSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == id);
            de.Durum = false;
            c.SaveChanges();
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
                siphesapla(Convert.ToInt32(de.SiparisID));
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
            de.Durum = false;
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
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

            sip.SiparisDurum = "Sipariş Onaylandı...";

            if (imagee != null)
            {
                var dosyaAdi = Path.GetFileName(imagee.FileName);

                var dosyaYolu = Path.Combine("wwwroot/Evraklar/SiparisOnayEvraklar", dosyaAdi);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await imagee.CopyToAsync(stream);
                }

                sip.DosyaYolu = dosyaYolu;

                sip.OnayDurum = true;
                sip.OnaylayanID = kulid;
                sip.OnayTarihi = DateTime.Now;
                sip.OnayAciklama = OnayAciklama;

                Formuller formuller = new Formuller(c);
                formuller.SiparisOnay(id);

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
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true && v.SiparisDurum == "Hazır...").OrderByDescending(v => v.ID).ToList();
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
    }
}