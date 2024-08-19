using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
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
        public IActionResult Hesap()
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
        public IActionResult Urunler()
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
        public IActionResult UrunDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == id);
                DtoUrunKategori list = new DtoUrunKategori();
                list.ID = Convert.ToInt32(kat.ID);
                if (kat.Adi != null) list.Adi = kat.Adi.ToString(); else list.Adi = "Tanımlanmamış...";
                list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(kat.Resim);
                list.Kodu = kat.Kodu;
                ViewBag.id = id;
                ViewBag.hata = BLoginHata.Icerik;
                return View(list);
            }
        }
        public IActionResult Odeme()
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
        public IActionResult Sepet()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                int bayiid = Convert.ToInt32(Cerez);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                if (bayi.KDVDurum == true) ViewBag.kdv = "10"; else ViewBag.kdv = "0";                
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString();
                ViewBag.hata = BLoginHata.Icerik;
                return View();
            }
        }
        public IActionResult Siparis()
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
        public IActionResult SipDet(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                ViewBag.id = id;
                ViewBag.hata = BLoginHata.Icerik;
                return View();
            }
        }
    }
}