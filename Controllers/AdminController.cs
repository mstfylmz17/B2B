using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class AdminController : Controller
    {
        private readonly Context c;
        public AdminController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = AdminHata.Icerik;
                return View();
            }
        }
    }
}
