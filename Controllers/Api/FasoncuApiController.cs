using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class FasoncuApiController : Controller
    {
        private readonly Context c;
        public FasoncuApiController(Context context)
        {
            c = context;
        }
        //Bayi İşlemleri
        [HttpPost]
        public IActionResult FasoncuList()
        {
            var veri = c.Fasonculars.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoFasoncular> ham = new List<DtoFasoncular>();
            foreach (var x in veri)
            {
                DtoFasoncular list = new DtoFasoncular();
                list.ID = Convert.ToInt32(x.ID);
                if (x.Unvan != null) list.Unvan = x.Unvan.ToString(); else list.Unvan = "Tanımlanmamış...";
                if (x.Adres != null) list.Adres = x.Adres.ToString(); else list.Adres = "Tanımlanmamış...";
                if (x.Telefon != null) list.Telefon = x.Telefon.ToString(); else list.Telefon = "Tanımlanmamış...";
                if (x.EPosta != null) list.EPosta = x.EPosta.ToString(); else list.EPosta = "Tanımlanmamış...";
                if (x.Yetkili != null) list.Yetkili = x.Yetkili.ToString(); else list.Yetkili = "Tanımlanmamış...";
                if (x.FasoncuKodu != null) list.FasoncuKodu = x.FasoncuKodu.ToString(); else list.FasoncuKodu = "Tanımlanmamış...";
                if (x.Adres != null) list.Adres = x.Adres.ToString(); else list.Adres = "Tanımlanmamış...";
                if (x.VergiDairesi != null) list.VergiDairesi = x.VergiDairesi.ToString(); else list.VergiDairesi = "Tanımlanmamış...";
                if (x.VergiNo != null) list.Adres = x.VergiNo.ToString(); else list.VergiNo = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult FasoncuEkle(Fasoncular d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.Unvan != null && d.Telefon != null)
                {
                    Fasoncular de = new Fasoncular();
                    de.Unvan = d.Unvan;
                    de.Telefon = d.Telefon;
                    de.EPosta = d.EPosta;
                    de.Yetkili = d.Yetkili;
                    de.Adres = d.Adres;
                    de.VergiNo = d.VergiNo;
                    de.VergiDairesi = d.VergiDairesi;
                    de.FasoncuKodu = d.FasoncuKodu;
                    de.Durum = true;
                    c.Fasonculars.Add(de);
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
        public IActionResult FasoncuSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Fasoncular de = c.Fasonculars.FirstOrDefault(v => v.ID == id);
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
        public IActionResult FasoncuDuzenle(Fasoncular d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Fasoncular de = c.Fasonculars.FirstOrDefault(v => v.ID == d.ID);
                if (d.Unvan != null) de.Unvan = d.Unvan;
                if (d.Adres != null) de.Adres = d.Adres;
                if (d.Telefon != null) de.Telefon = d.Telefon;
                if (d.EPosta != null) de.EPosta = d.EPosta;
                if (d.Yetkili != null) de.Yetkili = d.Yetkili;
                if (d.VergiDairesi != null) de.VergiDairesi = d.VergiDairesi;
                if (d.VergiNo != null) de.VergiNo = d.VergiNo;
                if (d.FasoncuKodu != null) de.FasoncuKodu = d.FasoncuKodu;
                c.SaveChanges();
                result = new { status = "success", message = "Güncelleme Başarılı..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
    }
}