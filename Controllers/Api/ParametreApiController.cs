using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class ParametreApiController : Controller
    {
        private readonly Context c;
        public ParametreApiController(Context context)
        {
            c = context;
        }
        //Departman İşlemleri
        [HttpPost]
        public IActionResult DepartmanList()
        {
            var veri = c.Departmans.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepartman> ham = new List<DtoDepartman>();
            foreach (var x in veri)
            {
                DtoDepartman list = new DtoDepartman();
                list.ID = Convert.ToInt32(x.ID);
                if (x.DepartmanAdi != null) list.DepartmanAdi = x.DepartmanAdi.ToString(); else list.DepartmanAdi = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult DepartmanEkle(Departman d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.DepartmanAdi != null)
                {
                    Departman de = new Departman();
                    de.DepartmanAdi = d.DepartmanAdi;
                    de.Durum = true;
                    c.Departmans.Add(de);
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
        public IActionResult DepartmanSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Departman de = c.Departmans.FirstOrDefault(v => v.ID == id);
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
        //Birim İşlemleri
        [HttpPost]
        public IActionResult BirimList()
        {
            var veri = c.Birimlers.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoBirimler> ham = new List<DtoBirimler>();
            foreach (var x in veri)
            {
                DtoBirimler list = new DtoBirimler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.BirimAdi != null) list.BirimAdi = x.BirimAdi.ToString(); else list.BirimAdi = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult BirimEkle(Birimler d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.BirimAdi != null)
                {
                    Birimler de = new Birimler();
                    de.BirimAdi = d.BirimAdi;
                    de.Durum = true;
                    c.Birimlers.Add(de);
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
        public IActionResult BirimSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Birimler de = c.Birimlers.FirstOrDefault(v => v.ID == id);
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
        //Yetki İşlemleri
        [HttpPost]
        public IActionResult YetkiList()
        {
            var veri = c.YetkiTurlaris.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoYetkiTurlari> ham = new List<DtoYetkiTurlari>();
            foreach (var x in veri)
            {
                DtoYetkiTurlari list = new DtoYetkiTurlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.YetkiAdi != null) list.YetkiAdi = x.YetkiAdi.ToString(); else list.YetkiAdi = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult YetkiEkle(YetkiTurlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.YetkiAdi != null)
                {
                    YetkiTurlari de = new YetkiTurlari();
                    de.YetkiAdi = d.YetkiAdi;
                    de.Durum = true;
                    c.YetkiTurlaris.Add(de);
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
        public IActionResult YetkiSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                YetkiTurlari de = c.YetkiTurlaris.FirstOrDefault(v => v.ID == id);
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
        //Yetki İşlemleri
        [HttpPost]
        public IActionResult KategoriList()
        {
            var veri = c.UrunKategoris.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunKategori> ham = new List<DtoUrunKategori>();
            foreach (var x in veri)
            {
                DtoUrunKategori list = new DtoUrunKategori();
                list.ID = Convert.ToInt32(x.ID);
                if (x.Adi != null) list.Adi = x.Adi.ToString(); else list.Adi = "Tanımlanmamış...";
                list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                list.Kodu = x.Kodu;
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public async Task<IActionResult> KategoriEkle(UrunKategori d, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (imagee == null || imagee.Length == 0)
                {
                    result = new { status = "error", message = "Resim Boş Geçilemez..." };
                    return Json(result);
                }

                if (d.Adi != null && d.Kodu != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imagee.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray();

                        UrunKategori kat = new UrunKategori();
                        kat.Adi = d.Adi;
                        kat.Kodu = d.Kodu;
                        kat.Resim = bytes;
                        kat.Durum = true;
                        c.UrunKategoris.Add(kat);
                        c.SaveChanges();
                    }
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
        public IActionResult KategoriSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunKategori de = c.UrunKategoris.FirstOrDefault(v => v.ID == id);
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
        //Yetki İşlemleri
        [HttpPost]
        public IActionResult KullaniciList()
        {
            var veri = c.Kullanicis.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoKullanici> ham = new List<DtoKullanici>();
            foreach (var x in veri)
            {
                DtoKullanici list = new DtoKullanici();
                list.ID = Convert.ToInt32(x.ID);
                if (x.AdSoyad != null) list.AdSoyad = x.AdSoyad.ToString(); else list.AdSoyad = "Tanımlanmamış...";
                if (x.KullaniciAdi != null) list.KullaniciAdi = x.KullaniciAdi.ToString(); else list.KullaniciAdi = "Tanımlanmamış...";
                if (x.Sifre != null) list.Sifre = x.Sifre.ToString(); else list.Sifre = "Tanımlanmamış...";
                if (x.Adres != null) list.Adres = x.Adres.ToString(); else list.Adres = "Tanımlanmamış...";
                if (x.Telefon != null) list.Telefon = x.Telefon.ToString(); else list.Telefon = "Tanımlanmamış...";
                if (x.EPosta != null) list.EPosta = x.EPosta.ToString(); else list.EPosta = "Tanımlanmamış...";
                if (x.DepartmanID != null) list.DepartmanID = c.Departmans.FirstOrDefault(v => v.ID == x.DepartmanID).DepartmanAdi.ToString(); else list.DepartmanID = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult YetkiTurlariID(int id)
        {
            var veri = c.YetkiTanimlaris.Where(v => v.Durum == true && v.KullaniciID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoYetkiTanimlari> ham = new List<DtoYetkiTanimlari>();
            foreach (var x in veri)
            {
                DtoYetkiTanimlari list = new DtoYetkiTanimlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.YetkiTurlariID != null) list.YetkiTurlariID = c.YetkiTurlaris.FirstOrDefault(v => v.ID == x.YetkiTurlariID).YetkiAdi.ToString(); else list.YetkiTurlariID = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult KullaniciYetkiSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                YetkiTanimlari de = c.YetkiTanimlaris.FirstOrDefault(v => v.ID == id);
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
        public IActionResult KullaniciYetkiEkle(YetkiTanimlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.YetkiTurlariID != null)
                {
                    YetkiTanimlari k = new YetkiTanimlari();
                    k.KullaniciID = d.KullaniciID;
                    k.YetkiTurlariID = d.YetkiTurlariID;
                    k.Durum = true;
                    c.YetkiTanimlaris.Add(k);
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
        public IActionResult KullaniciEkle(Kullanici d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.KullaniciAdi != null && d.Sifre != null && d.AdSoyad != null)
                {
                    Kullanici k = new Kullanici();
                    k.AdSoyad = d.AdSoyad.ToString();
                    k.DepartmanID = d.DepartmanID;
                    k.Sifre = d.Sifre;
                    k.Telefon = d.Telefon;
                    k.KullaniciAdi = d.KullaniciAdi;
                    k.Adres = d.Adres;
                    k.EPosta = d.EPosta;
                    k.Durum = true;
                    c.Kullanicis.Add(k);
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
        public IActionResult KullaniciSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Kullanici de = c.Kullanicis.FirstOrDefault(v => v.ID == id);
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
        public IActionResult KullaniciSifreDeğis(int id, string Yeni, string Yeni1, string Eski)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
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
                Kullanici de = c.Kullanicis.FirstOrDefault(v => v.ID == id);
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
        //Ürün Özellik Türleri İşlemleri
        [HttpPost]
        public IActionResult UrunOzellikTurlariList()
        {
            var veri = c.UrunOzelikTurlaris.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunOzelikTurlari> ham = new List<DtoUrunOzelikTurlari>();
            foreach (var x in veri)
            {
                DtoUrunOzelikTurlari list = new DtoUrunOzelikTurlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.OzellikAdi != null) list.OzellikAdi = x.OzellikAdi.ToString(); else list.OzellikAdi = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public async Task<IActionResult> UrunOzellikTurlariEkle(UrunOzelikTurlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.OzellikAdi != null)
                {
                    UrunOzelikTurlari kat = new UrunOzelikTurlari();
                    kat.OzellikAdi = d.OzellikAdi;
                    kat.Durum = true;
                    c.UrunOzelikTurlaris.Add(kat);
                    c.SaveChanges();
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
        public IActionResult UrunOzellikTurlariSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunOzellikleri de = c.UrunOzellikleris.FirstOrDefault(v => v.ID == id);
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
        //Ürün Alt Özellik Türleri İşlemleri
        [HttpPost]
        public IActionResult UrunAltOzellikTurlariList(int id)
        {
            var veri = c.UrunAltOzellikleris.Where(v => v.Durum == true && v.UrunOzellikTurlariID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunAltOzellikleri> ham = new List<DtoUrunAltOzellikleri>();
            foreach (var x in veri)
            {
                DtoUrunAltOzellikleri list = new DtoUrunAltOzellikleri();
                list.ID = Convert.ToInt32(x.ID);
                if (x.OzellikAdi != null) list.OzellikAdi = x.OzellikAdi.ToString(); else list.OzellikAdi = "Tanımlanmamış...";
                list.UrunOzellikTurlariID = c.UrunOzelikTurlaris.FirstOrDefault(v => v.ID == x.UrunOzellikTurlariID).OzellikAdi.ToString();
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult UrunAltOzellikTurlariEkle(UrunAltOzellikleri d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.OzellikAdi != null)
                {
                    UrunAltOzellikleri kat = new UrunAltOzellikleri();
                    kat.UrunOzellikTurlariID = d.UrunOzellikTurlariID;
                    kat.OzellikAdi = d.OzellikAdi;
                    kat.Durum = true;
                    c.UrunAltOzellikleris.Add(kat);
                    c.SaveChanges();
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
        public IActionResult UrunAltOzellikTurlariSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunAltOzellikleri de = c.UrunAltOzellikleris.FirstOrDefault(v => v.ID == id);
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
    }
}
