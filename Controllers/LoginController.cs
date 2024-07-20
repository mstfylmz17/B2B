using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class LoginController : Controller
    {
        private readonly Context c;
        public LoginController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            ViewBag.hata = LoginHata.Icerik;
            return View();
        }
        [HttpPost]
        public IActionResult Index(Kullanici k)
        {
            var kul = c.Kullanicis.FirstOrDefault(v => v.KullaniciAdi == k.KullaniciAdi && v.Sifre == k.Sifre && v.Durum == true);
            if (kul != null)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1), // Çerezin geçerlilik süresini belirleyin
                    HttpOnly = true, // Çerez sadece HTTP istekleriyle erişilebilir olsun
                    Secure = true, // Çerez sadece HTTPS üzerinden gönderilsin
                    SameSite = SameSiteMode.Strict // Çerez sadece aynı site üzerinden gönderilsin
                };

                HttpContext.Response.Cookies.Append("EnvanterTakipCerez", kul.ID.ToString(), cookieOptions);
                AdminHata.Icerik = "Hos Geldin... " + kul.AdSoyad;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                LoginHata.Icerik = "Kullanici Bilgileri Yanlis...";
                return RedirectToAction("Index");
            }
        }
        public IActionResult Cikis()
        {
            HttpContext.Response.Cookies.Delete("EnvanterTakipCerez");
            return RedirectToAction("Index");
        }
    }
}
