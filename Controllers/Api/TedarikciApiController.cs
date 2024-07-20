using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers.Api
{
    public class TedarikciApiController : Controller
    {
        private readonly Context c;
        public TedarikciApiController(Context context)
        {
            c = context;
        }
        //Bayi İşlemleri
        [HttpPost]
        public IActionResult TedarikciList()
        {
            var veri = c.Tedarikcilers.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoTedarikciler> ham = new List<DtoTedarikciler>();
            foreach (var x in veri)
            {
                DtoTedarikciler list = new DtoTedarikciler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.Unvan != null) list.Unvan = x.Unvan.ToString(); else list.Unvan = "Tanımlanmamış...";
                if (x.Adres != null) list.Adres = x.Adres.ToString(); else list.Adres = "Tanımlanmamış...";
                if (x.Telefon != null) list.Telefon = x.Telefon.ToString(); else list.Telefon = "Tanımlanmamış...";
                if (x.EPosta != null) list.EPosta = x.EPosta.ToString(); else list.EPosta = "Tanımlanmamış...";
                if (x.VergiNo != null) list.VergiNo = x.VergiNo.ToString(); else list.VergiNo = "Tanımlanmamış...";
                if (x.VergiDairesi != null) list.VergiDairesi = x.VergiDairesi.ToString(); else list.VergiDairesi = "Tanımlanmamış...";
                if (x.Yetkili != null) list.Yetkili = x.Yetkili.ToString(); else list.Yetkili = "Tanımlanmamış...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult TedarikciEkle(Tedarikciler d)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.Unvan != null && d.Telefon != null)
                {
                    Tedarikciler de = new Tedarikciler();
                    de.Unvan = d.Unvan;
                    de.Telefon = d.Telefon;
                    de.EPosta = d.EPosta;
                    de.Yetkili = d.Yetkili;
                    de.VergiNo = d.VergiNo;
                    de.VergiDairesi = d.VergiDairesi;
                    de.Durum = true;
                    c.Tedarikcilers.Add(de);
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
        public IActionResult TedarikciSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Tedarikciler de = c.Tedarikcilers.FirstOrDefault(v => v.ID == id);
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
        public IActionResult TedarikciDuzenle(Tedarikciler d)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Tedarikciler de = c.Tedarikcilers.FirstOrDefault(v => v.ID == d.ID);
                if (d.Unvan != null) de.Unvan = d.Unvan;
                if (d.Telefon != null) de.Telefon = d.Telefon;
                if (d.EPosta != null) de.EPosta = d.EPosta;
                if (d.Yetkili != null) de.Yetkili = d.Yetkili;
                if (d.VergiNo != null) de.VergiNo = d.VergiNo;
                if (d.VergiDairesi != null) de.VergiDairesi = d.VergiDairesi;
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
        public IActionResult TedarikciSiparisList(int id)
        {
            var veri = c.SatinAlmas.Where(v => v.Durum == true && v.TedarikciID == id).OrderByDescending(v => v.ID).ToList();
            var tedarikci = c.Tedarikcilers.FirstOrDefault(v => v.ID == id);
            List<DtoSatinAlma> ham = new List<DtoSatinAlma>();
            foreach (var x in veri)
            {
                DtoSatinAlma list = new DtoSatinAlma();
                list.ID = Convert.ToInt32(x.ID);

                ham.Add(list);
            }
            return Json(ham);
        }
    }
}
