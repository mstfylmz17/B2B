using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class HammaddeApiController : Controller
    {
        private readonly Context c;
        public HammaddeApiController(Context context)
        {
            c = context;
        }
        //Hammadde İşlemleri
        [HttpPost]
        public IActionResult HammaddeList()
        {
            var veri = c.Hammaddes.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoHammadde> ham = new List<DtoHammadde>();
            foreach (var x in veri)
            {
                DtoHammadde list = new DtoHammadde();
                list.ID = Convert.ToInt32(x.ID);
                if (x.HammaddeKodu != null) list.HammaddeKodu = x.HammaddeKodu.ToString(); else list.HammaddeKodu = "Tanımlanmamış...";
                if (x.HammaddeAciklama != null) list.HammaddeAciklama = x.HammaddeAciklama.ToString(); else list.HammaddeAciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.BirimID = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.BirimID = "Tanımlanmamış...";
                if (x.Stok != null) list.Stok = x.Stok.ToString(); else list.Stok = "0";
                list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public async Task<IActionResult> HammaddeEkle(Hammadde d, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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

                if (d.HammaddeAciklama != null && d.HammaddeKodu != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imagee.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray();

                        Hammadde kat = new Hammadde();
                        kat.HammaddeAciklama = d.HammaddeAciklama;
                        kat.HammaddeKodu = d.HammaddeKodu;
                        kat.Stok = d.Stok;
                        kat.BirimID = d.BirimID;
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                        kat.Resim = bytes;
                        kat.Durum = true;
                        kat.Stok = 0;
                        c.Hammaddes.Add(kat);
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
        public async Task<IActionResult> HammaddeDuzenle(Hammadde d, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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
                Hammadde kat = c.Hammaddes.FirstOrDefault(v => v.ID == d.ID);
                if (imagee != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imagee.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        kat.Resim = bytes;
                    }
                }
                if (d.HammaddeAciklama != null) kat.HammaddeAciklama = d.HammaddeAciklama;
                if (d.HammaddeKodu != null) kat.HammaddeKodu = d.HammaddeKodu;
                if (d.Stok != null) kat.Stok = d.Stok;
                if (d.BirimID != null) kat.BirimID = d.BirimID;
                if (d.KritikStokMiktari != null) kat.KritikStokMiktari = d.KritikStokMiktari;
                if (d.Stok != null) kat.Stok = d.Stok;
                c.SaveChanges();
                result = new { status = "success", message = "Kayıt Başarılı..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult HammaddeSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Hammadde de = c.Hammaddes.FirstOrDefault(v => v.ID == id);
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
