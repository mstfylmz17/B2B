using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers.Api
{
    public class BayiApiController : Controller
    {
        private readonly Context c;
        public BayiApiController(Context context)
        {
            c = context;
        }
        //Bayi İşlemleri
        [HttpPost]
        public IActionResult BayiList()
        {
            var veri = c.Bayilers.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoBayiler> ham = new List<DtoBayiler>();
            foreach (var x in veri)
            {
                DtoBayiler list = new DtoBayiler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.Unvan != null) list.Unvan = x.Unvan.ToString(); else list.Unvan = "Tanımlanmamış...";
                if (x.KullaniciAdi != null) list.KullaniciAdi = x.KullaniciAdi.ToString(); else list.KullaniciAdi = "Tanımlanmamış...";
                if (x.Sifre != null) list.Sifre = x.Sifre.ToString(); else list.Sifre = "Tanımlanmamış...";
                if (x.Adres != null) list.Adres = x.Adres.ToString(); else list.Adres = "Tanımlanmamış...";
                if (x.Telefon != null) list.Telefon = x.Telefon.ToString(); else list.Telefon = "Tanımlanmamış...";
                if (x.EPosta != null) list.EPosta = x.EPosta.ToString(); else list.EPosta = "Tanımlanmamış...";
                if (x.Yetkili != null) list.Yetkili = x.Yetkili.ToString(); else list.Yetkili = "Tanımlanmamış...";
                if (x.IskontoOran != null) list.IskontoOran = x.IskontoOran.ToString(); else list.IskontoOran = "Tanımlanmamış...";
                if (x.AlisVerisLimiti != null) list.AlisVerisLimiti = x.AlisVerisLimiti.ToString(); else list.AlisVerisLimiti = "Tanımlanmamış...";
                if (x.BayiKodu != null) list.BayiKodu = x.BayiKodu.ToString(); else list.BayiKodu = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult BayiEkle(Bayiler d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.Unvan != null && d.KullaniciAdi != null && d.Sifre != null && d.Telefon != null)
                {
                    Bayiler de = new Bayiler();
                    de.Unvan = d.Unvan;
                    de.KullaniciAdi = d.KullaniciAdi;
                    de.Sifre = d.Sifre;
                    de.Telefon = d.Telefon;
                    de.EPosta = d.EPosta;
                    de.Yetkili = d.Yetkili;
                    de.IskontoOran = 0;
                    de.AlisVerisLimiti = 0;
                    de.BayiKodu = d.BayiKodu;
                    de.Durum = true;
                    c.Bayilers.Add(de);
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
                else
                {
                    result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult BayiSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Bayiler de = c.Bayilers.FirstOrDefault(v => v.ID == id);
                de.Durum = false;
                c.SaveChanges();
                result = new { status = "success", message = "Kayıt Silindi..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult BayiDuzenle(Bayiler d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Bayiler de = c.Bayilers.FirstOrDefault(v => v.ID == d.ID);
                if (d.Unvan != null) de.Unvan = d.Unvan;
                if (d.KullaniciAdi != null) de.KullaniciAdi = d.KullaniciAdi;
                if (d.Telefon != null) de.Telefon = d.Telefon;
                if (d.EPosta != null) de.EPosta = d.EPosta;
                if (d.Yetkili != null) de.Yetkili = d.Yetkili;
                if (d.IskontoOran != null) de.IskontoOran = d.IskontoOran;
                if (d.AlisVerisLimiti != null) de.AlisVerisLimiti = d.AlisVerisLimiti;
                if (d.BayiKodu != null) de.BayiKodu = d.BayiKodu;
                c.SaveChanges();
                result = new { status = "success", message = "Güncelleme Başarılı..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult BayiSiparisList(int id)
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.BayiID == id).OrderByDescending(v => v.ID).ToList();
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == id);
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                var adim = c.SiparisAdimlaris.OrderBy(v => v.ID).FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == false);
                var adimturu = c.SiparisAdimTurlaris.FirstOrDefault(v => v.ID == adim.SiparisAdimTurlariID);
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("g");
                list.SiparisNo = x.SiparisNo.ToString();
                if (bayi.Telefon != null) list.Telefon = bayi.Telefon.ToString(); else list.Telefon = "Belirtilmemiş...";
                list.SiparisDurumu = adimturu.Aciklama.ToString();
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult BayiSifreDeğis(int id, string Yeni, string Yeni1, string Eski)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Bayilers.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (Yeni == null || Yeni1 == null || Eski == null)
                {
                    result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
                    return Json(result);
                }
                if (Yeni != Yeni1)
                {
                    result = new { status = "error", message = "Tanımlamak İstediğiniz Şifreler Uyuşmuyor!" };
                    return Json(result);
                }
                Bayiler de = c.Bayilers.FirstOrDefault(v => v.ID == id);
                if (Eski == de.Sifre)
                {
                    de.Sifre = Yeni;
                    c.SaveChanges();
                }
                else
                {
                    result = new { status = "error", message = "Eski Şifre Uyuşmuyor Lütfen Tekrar Deneyiniz..." };
                    return Json(result);
                }
                result = new { status = "success", message = "Şifre Değiştirildi..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
    }
}
