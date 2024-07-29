using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class UrunApiController : Controller
    {
        private readonly Context c;
        public UrunApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult UrunList()
        {
            var veri = c.Urunlers.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunler> ham = new List<DtoUrunler>();
            foreach (var x in veri)
            {
                decimal guncel = 0;
                DtoUrunler list = new DtoUrunler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunKodu != null) list.UrunKodu = x.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                if (x.UrunAdi != null) list.UrunAdi = x.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (x.UrunAciklama != null) list.UrunAciklama = x.UrunAciklama.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.Birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.Birim = "Tanımlanmamış...";
                list.UrunTuru = c.UrunTurlaris.FirstOrDefault(v => v.ID == x.UrunTuruID).UrunGrubuAdi.ToString();
                list.StokMiktari = c.UrunStoklaris.Where(v => v.ID == x.ID && v.Durum == true && v.StokMiktari > 0).Sum(v => v.StokMiktari).ToString();
                if (guncel < x.KritikStokMiktari) list.Durum = "Red";
                if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> UrunEkle(Urunler d, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var kodbosmu = c.Urunlers.FirstOrDefault(v => v.UrunKodu == d.UrunKodu);
                if (kodbosmu != null)
                {
                    result = new { status = "error", message = "Ürün Kodu Daha Önce Farklı Bir Ürüne Atanmış Lütfen Farklı Bir Ürün Kdu Tanımlayınız..." };
                }
                else
                {
                    if (d.UrunKodu != null && d.UrunAdi != null && d.UrunAciklama != null && d.UrunTuruID != null && d.BirimID != null)
                    {
                        Urunler kat = new Urunler();
                        kat.UrunKodu = d.UrunKodu;
                        kat.UrunAdi = d.UrunAdi;
                        kat.UrunAciklama = d.UrunAciklama;
                        kat.UrunTuruID = d.UrunTuruID;
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                        kat.BirimID = d.BirimID;
                        if (imagee != null)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                await imagee.CopyToAsync(memoryStream);
                                byte[] bytes = memoryStream.ToArray();

                                kat.Resim = bytes;
                            }
                        }
                        kat.Durum = true;
                        c.Urunlers.Add(kat);
                        c.SaveChanges();
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
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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
                    ozellik += ozellikturu.OzellikAdi.ToString() + " - " + ozelliktanim.OzellikAdi.ToString();
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
        public async Task<IActionResult> UrunDuzenle(Urunler d, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var kodbosmu = c.Urunlers.FirstOrDefault(v => v.UrunKodu == d.UrunKodu);
                if (kodbosmu != null)
                {
                    result = new { status = "error", message = "Ürün Kodu Daha Önce Farklı Bir Ürüne Atanmış Lütfen Farklı Bir Ürün Kdu Tanımlayınız..." };
                }
                else
                {
                    Urunler kat = c.Urunlers.FirstOrDefault(v => v.ID == d.ID);
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
                    if (imagee != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await imagee.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();

                            kat.Resim = bytes;
                        }
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
        public async Task<IActionResult> UrunOzellikEkle(UrunOzellikTanimlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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
    }
}
