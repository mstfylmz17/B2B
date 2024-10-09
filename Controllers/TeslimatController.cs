using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class TeslimatController : Controller
    {
        private readonly Context c;
        public TeslimatController(Context context)
        {
            c = context;
        }
        public IActionResult Gun()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
        public IActionResult Hafta()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
        public IActionResult Ay()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
        public IActionResult Yil()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
    }
}
