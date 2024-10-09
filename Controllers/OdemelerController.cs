using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class OdemelerController : Controller
    {
        private readonly Context c;
        public OdemelerController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = OdemelerHata.Icerik;
                return View();
            }
        }
    }
}
