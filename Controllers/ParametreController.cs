using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class ParametreController : Controller
    {
        private readonly Context c;
        public ParametreController(Context context)
        {
            c = context;
        }
        public IActionResult Birim()
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = ParametreHata.Icerik;
                return View();
            }
        }
        public IActionResult Departman()
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = ParametreHata.Icerik;
                return View();
            }
        }
    }
}
