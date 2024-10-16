﻿using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

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
            return Json(ham.OrderBy(v => v.BirimAdi));
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
        public IActionResult BirimDuzenle(Birimler d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.BirimAdi != null)
                {
                    Birimler de = c.Birimlers.FirstOrDefault(v => v.ID == d.ID);
                    de.BirimAdi = d.BirimAdi;
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Güncellendi..." };
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
        [HttpPost]
        public IActionResult BirimBilgi(int id)
        {
            var x = c.Birimlers.FirstOrDefault(v => v.ID == id);
            DtoBirimler list = new DtoBirimler();
            if (x.BirimAdi != null) list.BirimAdi = x.BirimAdi.ToString(); else list.BirimAdi = "Tanımlanmamış...";
            return Json(list);
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
        //Kategori İşlemleri
        [HttpPost]
        public async Task<IActionResult> KategoriList(int id)
        {
            if (id == 4)
            {
                var veri = c.UrunKategoris.Where(v => v.Durum == true)
                                          .OrderByDescending(v => v.ID)
                                          .Select(x => new DtoUrunKategori
                                          {
                                              ID = x.ID,
                                              Adi = x.Adi ?? "",
                                              SiraNo = x.SiraNo,
                                          }).ToList();

                return Json(veri.OrderBy(v => v.SiraNo));
            }
            else
            {
                var veri = c.UrunKategoris.Where(v => v.Durum == true && v.ID != 63)
                                          .OrderByDescending(v => v.ID)
                                          .Select(x => new DtoUrunKategori
                                          {
                                              ID = x.ID,
                                              Adi = x.Adi ?? "",
                                              SiraNo = x.SiraNo,
                                          }).ToList();

                return Json(veri.OrderBy(v => v.SiraNo));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetResim(int id)
        {
            var urun = await c.UrunKategoris.FindAsync(id);
            if (urun == null || urun.Resim == null)
            {
                return NotFound();
            }
            return File(urun.Resim, "image/jpeg");
        }
        //Resim Küçültme
        private async Task<byte[]> CompressImageAsync(byte[] imageBytes, int width, int height, int quality)
        {
            using (var inputStream = new MemoryStream(imageBytes))
            using (var outputStream = new MemoryStream())
            {
                using (var image = await Image.LoadAsync(inputStream))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(width, height)
                    }));

                    var encoder = new JpegEncoder
                    {
                        Quality = quality // 0-100 arası kalite (daha düşük kalite, daha küçük boyut)
                    };

                    await image.SaveAsJpegAsync(outputStream, encoder);
                }

                return outputStream.ToArray();
            }
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

                if (d.Adi != null && d.SiraNo != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imagee.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray();

                        UrunKategori kat = new UrunKategori();
                        kat.Adi = d.Adi;
                        kat.Kodu = d.Kodu;
                        kat.Resim = bytes;
                        kat.SiraNo = d.SiraNo;
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
        public async Task<IActionResult> KategoriDuzenle(UrunKategori d, IFormFile image)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunKategori kat = c.UrunKategoris.FirstOrDefault(v => v.ID == d.ID);
                if (image != null)
                {
                    if (image.Length > 0)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();

                            kat.Resim = bytes;
                            c.SaveChanges();
                        }
                    }
                }
                kat.Adi = d.Adi;
                kat.SiraNo = d.SiraNo;
                c.SaveChanges();
                result = new { status = "success", message = "Kayıt Güncellendi..." };
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
        [HttpPost]
        public IActionResult KategoriBilgi(int id)
        {
            var x = c.UrunKategoris.FirstOrDefault(v => v.ID == id);
            DtoUrunKategori list = new DtoUrunKategori();
            if (x.Adi != null) list.Adi = x.Adi.ToString(); else list.Adi = "Tanımlanmamış...";
            if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim); else list.Resim = "";
            list.SiraNo = x.SiraNo;
            return Json(list);
        }
        //Kullanıcı Yetki İşlemleri
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
            var veri = c.PanelTanimlaris.Where(v => v.Durum == true && v.KullaniciID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoYetkiTanimlari> ham = new List<DtoYetkiTanimlari>();
            foreach (var x in veri)
            {
                int depid = Convert.ToInt32(x.DepartmanID);
                DtoYetkiTanimlari list = new DtoYetkiTanimlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.DepartmanID != null) list.YetkiTurlariID = c.Panellers.FirstOrDefault(v => v.ID == depid).PanelAdi.ToString(); else list.YetkiTurlariID = "";
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
                PanelTanimlari de = c.PanelTanimlaris.FirstOrDefault(v => v.ID == id);
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
                    PanelTanimlari k = new PanelTanimlari();
                    k.KullaniciID = d.KullaniciID;
                    k.DepartmanID = d.YetkiTurlariID;
                    k.Durum = true;
                    c.PanelTanimlaris.Add(k);
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
                UrunOzelikTurlari de = c.UrunOzelikTurlaris.FirstOrDefault(v => v.ID == id);
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
        public IActionResult UrunAltOzellikTurlariDuzenle(UrunAltOzellikleri d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.OzellikAdi != null)
                {
                    UrunAltOzellikleri kat = c.UrunAltOzellikleris.FirstOrDefault(v => v.ID == d.ID);
                    kat.OzellikAdi = d.OzellikAdi;
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
        [HttpPost]
        public IActionResult UrunAltOzellikBilgi(int id)
        {
            var x = c.UrunAltOzellikleris.FirstOrDefault(v => v.ID == id);
            DtoUrunAltOzellikleri list = new DtoUrunAltOzellikleri();
            list.OzellikAdi = x.OzellikAdi.ToString();
            return Json(list);
        }
        //Yetki İşlemleri
        [HttpPost]
        public IActionResult UrunTuruList()
        {
            var veri = c.UrunTurlaris.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunTurlari> ham = new List<DtoUrunTurlari>();
            foreach (var x in veri)
            {
                DtoUrunTurlari list = new DtoUrunTurlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunGrubuAdi != null) list.UrunGrubuAdi = x.UrunGrubuAdi.ToString(); else list.UrunGrubuAdi = "Tanımlanmamış...";
                list.SiraNo = x.SiraNo;
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.SiraNo));
        }
        [HttpPost]
        public IActionResult UrunTuruEkle(UrunTurlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {

                if (d.UrunGrubuAdi != null && d.SiraNo != null)
                {

                    UrunTurlari kat = new UrunTurlari();
                    kat.UrunGrubuAdi = d.UrunGrubuAdi;
                    kat.SiraNo = d.SiraNo;
                    kat.Durum = true;
                    c.UrunTurlaris.Add(kat);
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
        public IActionResult UrunTuruDuzenle(UrunTurlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunTurlari kat = c.UrunTurlaris.FirstOrDefault(v => v.ID == d.ID);
                kat.UrunGrubuAdi = d.UrunGrubuAdi;
                kat.SiraNo = d.SiraNo;
                kat.Durum = true;
                c.SaveChanges();
                result = new { status = "success", message = "Kayıt Güncellendi..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult UrunTuruSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunTurlari de = c.UrunTurlaris.FirstOrDefault(v => v.ID == id);
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
        public IActionResult UrunTuruBilgi(int id)
        {
            var x = c.UrunTurlaris.FirstOrDefault(v => v.ID == id);
            DtoUrunTurlari list = new DtoUrunTurlari();
            list.SiraNo = x.SiraNo;
            list.UrunGrubuAdi = x.UrunGrubuAdi;
            return Json(list);
        }
    }
}
