using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class BLoginController : Controller
    {
        private readonly Context c;
        public BLoginController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            ViewBag.hata = BLoginHata.Icerik;
            return View();
        }
        [HttpPost]
        public IActionResult Index(Bayiler k)
        {
            var kul = c.Bayilers.FirstOrDefault(v => v.KullaniciAdi == k.KullaniciAdi && v.Sifre == k.Sifre && v.Durum == true);
            if (kul != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1), // Çerezin geçerlilik süresini belirleyin
                    HttpOnly = true, // Çerez sadece HTTP istekleriyle erişilebilir olsun
                    Secure = true, // Çerez sadece HTTPS üzerinden gönderilsin
                    SameSite = SameSiteMode.Strict // Çerez sadece aynı site üzerinden gönderilsin
                };

                HttpContext.Response.Cookies.Append("VNNBayiCerez", kul.ID.ToString(), cookieOptions);
                AdminHata.Icerik = "Hos Geldin... " + kul.Unvan;
                return RedirectToAction("menu", "BLogin");
            }
            else
            {
                LoginHata.Icerik = "Kullanici Bilgileri Yanlis...";
                return RedirectToAction("Index");
            }
        }
        public IActionResult Cikis()
        {
            HttpContext.Response.Cookies.Delete("VNNBayiCerez");
            return RedirectToAction("Index");
        }


        public IActionResult menu()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                ViewBag.hata = BLoginHata.Icerik;
                return View();
            }
        }
    }
}
