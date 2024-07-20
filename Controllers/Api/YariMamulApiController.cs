using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class YariMamulApiController : Controller
    {
        private readonly Context c;
        public YariMamulApiController(Context context)
        {
            c = context;
        }
        //Yarı Mamul İşlemleri
        [HttpPost]
        public IActionResult YariMamulList()
        {
            var veri = c.YariMamuls.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoYariMamul> ham = new List<DtoYariMamul>();
            foreach (var x in veri)
            {
                DtoYariMamul list = new DtoYariMamul();
                list.ID = Convert.ToInt32(x.ID);
                if (x.Kodu != null) list.Kodu = x.Kodu.ToString(); else list.Kodu = "Tanımlanmamış...";
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.BirimID = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.BirimID = "Tanımlanmamış...";
                if (x.YariMamulGrupID != null) list.YariMamulGrupID = c.YariMamulGruplaris.FirstOrDefault(v => v.ID == x.YariMamulGrupID).Adi.ToString(); else list.YariMamulGrupID = "Tanımlanmamış...";
                if (x.Stok != null) list.Stok = x.Stok.ToString(); else list.Stok = "0";
                list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public async Task<IActionResult> YariMamulEkle(YariMamul d, IFormFile imagee)
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

                if (d.Aciklama != null && d.Kodu != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imagee.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray();

                        YariMamul kat = new YariMamul();
                        kat.Aciklama = d.Aciklama;
                        kat.Kodu = d.Kodu;
                        kat.Stok = d.Stok;
                        kat.BirimID = d.BirimID;
                        kat.YariMamulGrupID = d.YariMamulGrupID;
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                        kat.Resim = bytes;
                        kat.Durum = true;
                        kat.Stok = 0;
                        c.YariMamuls.Add(kat);
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
        public async Task<IActionResult> YariMamulDuzenle(YariMamul d, IFormFile imagee)
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
                YariMamul kat = c.YariMamuls.FirstOrDefault(v => v.ID == d.ID);
                if (imagee != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imagee.CopyToAsync(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        kat.Resim = bytes;
                    }
                }
                if (d.Aciklama != null) kat.Aciklama = d.Aciklama;
                if (d.Kodu != null) kat.Kodu = d.Kodu;
                if (d.Stok != null) kat.Stok = d.Stok;
                if (d.BirimID != null) kat.BirimID = d.BirimID;
                if (d.YariMamulGrupID != null) kat.YariMamulGrupID = d.YariMamulGrupID;
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
        public IActionResult YariMamulSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                YariMamul de = c.YariMamuls.FirstOrDefault(v => v.ID == id);
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
