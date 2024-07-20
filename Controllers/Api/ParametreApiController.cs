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
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
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
    }
}
