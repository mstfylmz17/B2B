using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class FasoncularController : Controller
    {
        private readonly Context c;
        public FasoncularController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = FasoncularHata.Icerik;
                return View();
            }
        }
        public IActionResult Detay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var bayi = c.Fasonculars.FirstOrDefault(v => v.ID == id);
                DtoFasoncular veri = new DtoFasoncular();
                if (bayi.Unvan != null) veri.Unvan = bayi.Unvan; else veri.Unvan = "Tanımlı Değil...";
                if (bayi.Adres != null) veri.Adres = bayi.Adres; else veri.Adres = "Tanımlı Değil...";
                if (bayi.Adres != null) veri.Adres = bayi.Adres; else veri.Adres = "Tanımlı Değil...";
                if (bayi.Telefon != null) veri.Telefon = bayi.Telefon; else veri.Telefon = "Tanımlı Değil...";
                if (bayi.EPosta != null) veri.EPosta = bayi.EPosta; else veri.EPosta = "Tanımlı Değil...";
                if (bayi.Yetkili != null) veri.Yetkili = bayi.Yetkili; else veri.Yetkili = "Tanımlı Değil...";
                if (bayi.VergiDairesi != null) veri.VergiDairesi = bayi.VergiDairesi.ToString(); else veri.VergiDairesi = "Tanımlı Değil...";
                if (bayi.VergiNo != null) veri.VergiNo = bayi.VergiNo.ToString(); else veri.VergiNo = "Tanımlı Değil...";
                if (bayi.FasoncuKodu != null) veri.FasoncuKodu = bayi.FasoncuKodu.ToString(); else veri.FasoncuKodu = "Tanımlı Değil...";
                ViewBag.id = id;
                ViewBag.hata = FasoncularHata.Icerik;
                return View(veri);
            }
        }
    }
}
