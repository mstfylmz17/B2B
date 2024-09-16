using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using VNNB2B.Models;

namespace VNNB2B.Controllers.Api
{
    public class UrunApiController : Controller
    {
        private readonly Context c;
        private readonly IWebHostEnvironment _env;
        public UrunApiController(Context context, IWebHostEnvironment env)
        {
            c = context;
            _env = env;
        }
        [HttpPost]
        public async Task<IActionResult> UrunList()
        {
            var veri = await c.Urunlers
                .Where(v => v.Durum == true)
                .OrderByDescending(v => v.ID)
                .Select(x => new DtoUrunler
                {
                    ID = x.ID,
                    UrunKodu = x.UrunKodu ?? "",
                    UrunAdi = x.UrunAdi ?? "",
                    UrunAciklama = x.UrunAciklama ?? "",
                    KritikStokMiktari = x.KritikStokMiktari.ToString() ?? "",
                    Birim = c.Birimlers.FirstOrDefault(b => b.ID == x.BirimID).BirimAdi ?? "",
                    UrunKategori = c.UrunKategoris.FirstOrDefault(k => k.ID == x.UrunKategoriID).Adi ?? "",
                    UrunTuru = c.UrunTurlaris.FirstOrDefault(t => t.ID == x.UrunTuruID).UrunGrubuAdi ?? "",
                    StokMiktari = x.StokMiktari.ToString() ?? "0",
                    Durum = (c.UrunStoklaris
                            .Where(s => s.UrunID == x.ID && s.Durum == true)
                            .Sum(s => s.StokMiktari) < x.KritikStokMiktari) ? "Red" : "Normal"
                })
                .ToListAsync();
            return Json(veri.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> GetResim(int id)
        {
            var urun = await c.Urunlers.FindAsync(id);
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
        public async Task<IActionResult> SonUrun()
        {
            var veri = await c.Urunlers
                .Where(v => v.Durum == true)
                .OrderByDescending(v => v.ID)
                .Select(x => new DtoUrunler
                {
                    ID = x.ID,
                    UrunKodu = x.UrunKodu ?? "Tanımlanmamış...",
                    UrunAdi = x.UrunAdi ?? "Tanımlanmamış...",
                    UrunAciklama = x.UrunAciklama ?? "Tanımlanmamış...",
                    KritikStokMiktari = x.KritikStokMiktari.ToString() ?? "Tanımlanmamış...",
                    Birim = c.Birimlers.FirstOrDefault(b => b.ID == x.BirimID).BirimAdi ?? "Tanımlanmamış...",
                    UrunKategori = c.UrunKategoris.FirstOrDefault(k => k.ID == x.UrunKategoriID).Adi ?? "Tanımlanmamış...",
                    UrunTuru = c.UrunTurlaris.FirstOrDefault(t => t.ID == x.UrunTuruID).UrunGrubuAdi ?? "Tanımlanmamış...",
                    StokMiktari = c.UrunStoklaris
                        .Where(s => s.ID == x.ID && s.Durum == true && s.StokMiktari > 0)
                        .Sum(s => s.StokMiktari)
                        .ToString(),
                    Durum = (c.UrunStoklaris
                            .Where(s => s.ID == x.ID && s.Durum == true && s.StokMiktari > 0)
                            .Sum(s => s.StokMiktari) < x.KritikStokMiktari) ? "Red" : "Normal"
                })
                .FirstOrDefaultAsync();
            return Json(veri);
        }
        [HttpPost]
        public async Task<IActionResult> GetResimText(int id)
        {
            var i = await c.SatinAlmaTalepleri.FirstOrDefaultAsync(v => v.ID == id);
            var urun = await c.Urunlers.FindAsync(i.UrunID);
            if (urun == null || urun.Resim == null)
            {
                return NotFound();
            }
            var base64Image = Convert.ToBase64String(urun.Resim);
            return Ok(base64Image);
        }
        [HttpPost]
        public async Task<IActionResult> HammaddeHirdavatList()
        {
            var veri = await c.Urunlers
                .Where(v => v.Durum == true && v.UrunTuruID != 3)
                .OrderByDescending(v => v.ID)
                .Select(x => new DtoUrunler
                {
                    ID = x.ID,
                    UrunKodu = x.UrunKodu ?? "",
                    UrunAdi = x.UrunAdi ?? "",
                    UrunAciklama = x.UrunAciklama ?? "",
                    KritikStokMiktari = x.KritikStokMiktari.ToString() ?? "",
                    Birim = c.Birimlers.FirstOrDefault(b => b.ID == x.BirimID).BirimAdi ?? "",
                    UrunKategori = c.UrunKategoris.FirstOrDefault(k => k.ID == x.UrunKategoriID).Adi ?? "",
                    UrunTuru = c.UrunTurlaris.FirstOrDefault(t => t.ID == x.UrunTuruID).UrunGrubuAdi ?? "",
                    StokMiktari = x.StokMiktari.ToString() ?? "0",
                    Durum = (c.UrunStoklaris
                            .Where(s => s.UrunID == x.ID && s.Durum == true)
                            .Sum(s => s.StokMiktari) < x.KritikStokMiktari) ? "Red" : "Normal"
                })
                .ToListAsync();
            return Json(veri.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> KategoriUrunleriList()
        {
            var veri = await c.Urunlers
                .Where(v => v.Durum == true && v.UrunTuruID == 3)
                .OrderByDescending(v => v.ID)
                .Select(x => new DtoUrunler
                {
                    ID = x.ID,
                    UrunKodu = x.UrunKodu ?? "",
                    UrunAdi = x.UrunAdi ?? "",
                    UrunAciklama = x.UrunAciklama ?? "",
                    KritikStokMiktari = x.KritikStokMiktari.ToString() ?? "",
                    Birim = c.Birimlers.FirstOrDefault(b => b.ID == x.BirimID).BirimAdi ?? "",
                    UrunKategori = c.UrunKategoris.FirstOrDefault(k => k.ID == x.UrunKategoriID).Adi ?? "",
                    UrunTuru = c.UrunTurlaris.FirstOrDefault(t => t.ID == x.UrunTuruID).UrunGrubuAdi ?? "",
                    StokMiktari = c.UrunStoklaris
                        .Where(s => s.ID == x.ID && s.Durum == true && s.StokMiktari > 0)
                        .Sum(s => s.StokMiktari)
                        .ToString(),
                    Durum = (c.UrunStoklaris
                            .Where(s => s.ID == x.ID && s.Durum == true && s.StokMiktari > 0)
                            .Sum(s => s.StokMiktari) < x.KritikStokMiktari) ? "Red" : "Normal"
                })
                .ToListAsync();
            return Json(veri.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> UrunEkle(Urunler d, IFormFile imagee, string FiyatTL, string FiyatUSD, List<int> UrunOzellikleri)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var kodbosmu = c.Urunlers.FirstOrDefault(v => v.UrunKodu == d.UrunKodu && v.Durum == true);
                if (kodbosmu != null)
                {
                    result = new { status = "error", message = "Ürün Kodu Daha Önce Farklı Bir Ürüne Atanmış Lütfen Farklı Bir Ürün Kdu Tanımlayınız..." };
                }
                else
                {
                    if (d.UrunKodu != null && d.UrunAdi != null && d.UrunTuruID != null && d.BirimID != null)
                    {
                        Urunler kat = new Urunler();
                        kat.UrunKodu = d.UrunKodu;
                        kat.UrunKategoriID = d.UrunKategoriID;
                        kat.UrunAdi = d.UrunAdi;
                        kat.UrunAciklama = d.UrunAciklama;
                        kat.UrunTuruID = d.UrunTuruID;
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                        kat.BirimKG = d.BirimKG;
                        kat.BirimM3 = d.BirimM3;
                        kat.Boyut = d.Boyut;
                        kat.PaketAdet = d.PaketAdet;
                        kat.BirimID = d.BirimID;
                        kat.Durum = true;
                        c.Urunlers.Add(kat);
                        c.SaveChanges();
                        var kaydedilen = c.Urunlers.OrderByDescending(v => v.ID).FirstOrDefault(v => v.Durum == true);
                        if (imagee != null)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                await imagee.CopyToAsync(memoryStream);
                                byte[] bytes = memoryStream.ToArray();

                                var compressedImage = await CompressImageAsync(bytes, 800, 600, 95);
                                kaydedilen.Resim = compressedImage;
                                c.SaveChanges();
                            }
                        }
                        foreach (var v in UrunOzellikleri)
                        {
                            if (v != null && v > 0)
                            {
                                UrunOzellikTanimlari oz = new UrunOzellikTanimlari();
                                oz.UrunOzellikTurlariID = v;
                                oz.UrunID = kaydedilen.ID;
                                oz.Durum = true;
                                c.UrunOzellikTanimlaris.Add(oz);
                                c.SaveChanges();
                            }
                        }
                        try
                        {
                            var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == kaydedilen.ID && v.Durum == true);
                            decimal? tlfiat = Convert.ToDecimal(FiyatTL);
                            decimal? usdfiyat = Convert.ToDecimal(FiyatUSD);

                            UrunFiyatlari f = new UrunFiyatlari();
                            f.FiyatTL = tlfiat;
                            f.FiyatUSD = usdfiyat;
                            f.UrunID = kaydedilen.ID;
                            f.FiyatTarihi = DateTime.Now;
                            f.Durum = true;
                            c.UrunFiyatlaris.Add(f);
                            c.SaveChanges();

                            if (fiyat != null)
                            {
                                fiyat.Durum = false;
                            }
                            c.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            result = new { status = "error", message = ex.Message };
                        }
                        result = new { status = "success", message = "Kayıt Başarılı..." };
                    }
                    else
                    {
                        result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
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
        public IActionResult UrunSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Urunler de = c.Urunlers.FirstOrDefault(v => v.ID == id);
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
        public IActionResult UrunStokList(int id)
        {
            var veri = c.UrunStoklaris.Where(v => v.Durum == true && v.UrunID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunStoklari> ham = new List<DtoUrunStoklari>();
            foreach (var x in veri)
            {
                var sipno = c.SiparisIcerikUrunOzellikleris.FirstOrDefault(v => v.UrunStoklariID == x.ID);
                string ozellik = "";
                var ozellikleri = c.UrunOzellikleris.Where(v => v.UrunStokID == x.ID && v.Durum == true).ToList();
                foreach (var v in ozellikleri)
                {
                    var ozelliktanim = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikleriID);
                    var ozellikturu = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == ozelliktanim.UrunOzellikTurlariID);
                    ozellik += ozellikturu.OzellikAdi.ToString() + " / " + ozelliktanim.OzellikAdi.ToString() + "<br/>";
                }
                DtoUrunStoklari list = new DtoUrunStoklari();
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == id);
                list.ID = Convert.ToInt32(x.ID);
                list.StokTarihi = Convert.ToDateTime(x.StokTarihi).ToString("g");
                list.StokMiktari = x.StokMiktari.ToString();
                list.UrunOzellikleriID = ozellik.ToString();
                if (sipno != null)
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == sipno.SiaprisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == sipic.SiparisID);
                    list.SiparisNo = sip.SiparisNo.ToString();
                }
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> UrunDuzenle(Urunler d, IFormFile imagee, string FiyatTL, string FiyatUSD)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Urunler kat = c.Urunlers.FirstOrDefault(v => v.ID == d.ID);
                var kodbosmu = c.Urunlers.FirstOrDefault(v => v.UrunKodu == d.UrunKodu);
                if (kodbosmu != null && d.UrunKodu != null && d.UrunKodu != kat.UrunKodu)
                {
                    result = new { status = "error", message = "Ürün Kodu Daha Önce Farklı Bir Ürüne Atanmış Lütfen Farklı Bir Ürün Kdu Tanımlayınız..." };
                }
                else
                {
                    if (d.UrunKodu != null)
                        kat.UrunKodu = d.UrunKodu;
                    if (d.UrunAdi != null)
                        kat.UrunAdi = d.UrunAdi;
                    if (d.UrunAciklama != null)
                        kat.UrunAciklama = d.UrunAciklama;
                    if (d.UrunTuruID != null)
                        kat.UrunTuruID = d.UrunTuruID;
                    if (d.KritikStokMiktari != null)
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                    if (d.BirimID != null)
                        kat.BirimID = d.BirimID;
                    if (d.UrunKategoriID != null)
                        kat.UrunKategoriID = d.UrunKategoriID;
                    if (d.PaketAdet != null)
                        kat.PaketAdet = d.PaketAdet;
                    if (d.Boyut != null)
                        kat.Boyut = d.Boyut;
                    if (d.BirimM3 != null)
                        kat.BirimM3 = d.BirimM3;
                    if (d.BirimKG != null)
                        kat.BirimKG = d.BirimKG;
                    if (imagee != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await imagee.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();

                            var compressedImage = await CompressImageAsync(bytes, 800, 600, 95);
                            kat.Resim = compressedImage;
                            c.SaveChanges();
                        }
                    }
                    try
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == d.ID && v.Durum == true);
                        decimal? tlfiat = Convert.ToDecimal(FiyatTL);
                        decimal? usdfiyat = Convert.ToDecimal(FiyatUSD);

                        UrunFiyatlari f = new UrunFiyatlari();
                        f.FiyatTL = tlfiat;
                        f.FiyatUSD = usdfiyat;
                        f.UrunID = d.ID;
                        f.FiyatTarihi = DateTime.Now;
                        f.Durum = true;
                        c.UrunFiyatlaris.Add(f);
                        c.SaveChanges();
                        if (fiyat != null)
                        {
                            fiyat.Durum = false;
                        }
                        c.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result = new { status = "error", message = ex.Message };
                    }
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult UrunOzellikList(int id)
        {
            var veri = c.UrunOzellikTanimlaris.Where(v => v.Durum == true && v.UrunID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunOzellikTanimlari> ham = new List<DtoUrunOzellikTanimlari>();
            foreach (var x in veri)
            {
                decimal guncel = 0;
                DtoUrunOzellikTanimlari list = new DtoUrunOzellikTanimlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunOzellikTurlariID != null) list.UrunOzellikTanimi = c.UrunOzelikTurlaris.FirstOrDefault(v => v.ID == x.UrunOzellikTurlariID).OzellikAdi.ToString(); else list.UrunOzellikTanimi = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult UrunOzellikEkle(UrunOzellikTanimlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.UrunOzellikTurlariID != null)
                {
                    UrunOzellikTanimlari kat = new UrunOzellikTanimlari();
                    kat.UrunOzellikTurlariID = d.UrunOzellikTurlariID;
                    kat.UrunID = d.UrunID;
                    kat.Durum = true;
                    c.UrunOzellikTanimlaris.Add(kat);
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
                else
                {
                    result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
                }
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult OzellikSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunOzellikTanimlari de = c.UrunOzellikTanimlaris.FirstOrDefault(v => v.ID == id);
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
        public IActionResult UrunDetay(int id)
        {
            var x = c.Urunlers.Where(v => v.ID == id).FirstOrDefault();

            decimal guncel = 0;
            DtoUrunler list = new DtoUrunler();
            list.ID = Convert.ToInt32(x.ID);
            if (x.UrunKodu != null) list.UrunKodu = x.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
            if (x.UrunAdi != null) list.UrunAdi = x.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
            if (x.UrunAciklama != null) list.UrunAciklama = x.UrunAciklama.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
            if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
            if (x.BirimID != null) list.Birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.Birim = "Tanımlanmamış...";
            if (x.UrunKategoriID != null) list.UrunKategori = c.UrunKategoris.FirstOrDefault(v => v.ID == x.UrunKategoriID).Adi.ToString(); else list.UrunKategori = "Tanımlanmamış...";
            list.UrunTuru = c.UrunTurlaris.FirstOrDefault(v => v.ID == x.UrunTuruID).UrunGrubuAdi.ToString();
            list.StokMiktari = c.UrunStoklaris.Where(v => v.ID == x.ID && v.Durum == true && v.StokMiktari > 0).Sum(v => v.StokMiktari).ToString();
            if (guncel < x.KritikStokMiktari) list.Durum = "Red";
            if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
            var veri = c.UrunOzellikTanimlaris.Where(v => v.Durum == true && v.UrunID == id).OrderBy(v => v.ID).ToList();
            List<DtoUrunOzellikTanimlari> ham = new List<DtoUrunOzellikTanimlari>();
            foreach (var a in veri)
            {
                DtoUrunOzellikTanimlari l = new DtoUrunOzellikTanimlari();
                l.ID = Convert.ToInt32(a.UrunOzellikTurlariID);
                if (a.UrunOzellikTurlariID != null) l.UrunOzellikTanimi = c.UrunOzelikTurlaris.FirstOrDefault(v => v.ID == a.UrunOzellikTurlariID).OzellikAdi.ToString(); else l.UrunOzellikTanimi = "";
                ham.Add(l);
            }
            list.UrunOzellikleri = ham;
            return Json(list);
        }
        [HttpPost]
        public IActionResult KatUrunList(int id)
        {
            var veri = c.Urunlers
                .Where(v => v.Durum == true && v.UrunTuruID == 3 && v.UrunKategoriID == id)
                .OrderByDescending(v => v.ID)
                .Select(x => new DtoUrunler
                {
                    ID = x.ID,
                    UrunKodu = x.UrunKodu ?? "Tanımlanmamış...",
                    UrunAdi = x.UrunAdi ?? "Tanımlanmamış...",
                    UrunAciklama = x.UrunAciklama ?? "Tanımlanmamış...",
                    KritikStokMiktari = x.KritikStokMiktari.ToString() ?? "Tanımlanmamış...",
                    Birim = c.Birimlers.FirstOrDefault(b => b.ID == x.BirimID).BirimAdi ?? "Tanımlanmamış...",
                    UrunKategori = c.UrunKategoris.FirstOrDefault(k => k.ID == x.UrunKategoriID).Adi ?? "Tanımlanmamış...",
                    UrunTuru = c.UrunTurlaris.FirstOrDefault(t => t.ID == x.UrunTuruID).UrunGrubuAdi ?? "Tanımlanmamış...",
                    StokMiktari = c.UrunStoklaris
                        .Where(s => s.ID == x.ID && s.Durum == true && s.StokMiktari > 0)
                        .Sum(s => s.StokMiktari)
                        .ToString(),
                    Durum = (c.UrunStoklaris
                            .Where(s => s.ID == x.ID && s.Durum == true && s.StokMiktari > 0)
                            .Sum(s => s.StokMiktari) < x.KritikStokMiktari) ? "Red" : "Normal"
                })
                .ToList();
            return Json(veri.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult UrunAltOzellikleri(int id)
        {
            var veri = c.UrunAltOzellikleris.Where(v => v.Durum == true && v.UrunOzellikTurlariID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunAltOzellikleri> ham = new List<DtoUrunAltOzellikleri>();
            foreach (var x in veri)
            {
                DtoUrunAltOzellikleri list = new DtoUrunAltOzellikleri();
                list.ID = Convert.ToInt32(x.ID);
                if (x.OzellikAdi != null) list.OzellikAdi = x.OzellikAdi.ToString(); else list.OzellikAdi = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.OzellikAdi));
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
                var sip = c.Siparis.FirstOrDefault(v => v.BayiID == kulid && v.Durum == true && v.BayiOnay == false);
                int sipno = 0;
                if (sip == null)
                {
                    Siparis s = new Siparis();
                    s.BayiID = kulid;
                    s.SiparisBayiAciklama = "";
                    s.SiparisTarihi = DateTime.Now;
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
                    s.SiparisNo = kul.BayiKodu + " " + (c.Siparis.Count() + 1).ToString();
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
                        i.SatirToplam = i.SatirToplam + kdv;
                    }
                    else i.KDVTutari = 0;
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
                        i.TeslimAdet = 0;
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

        public class SepetEkleModel
        {
            public int SiparisID { get; set; }
            public int ID { get; set; }
            public string? Aciklama { get; set; }
            public int? Miktar { get; set; }
            public List<DtoUrunAltOzellikleri>? Ozellikler { get; set; }
        }
    }
}
