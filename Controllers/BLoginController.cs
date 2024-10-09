using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models;
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
                Formuller f = new Formuller(c);
                f.SipSil();
                HttpContext.Response.Cookies.Append("VNNBayiCerez", kul.ID.ToString(), cookieOptions);
                BLoginHata.Icerik = "VNN OFİS MOBİLYALARI B2B PANELİNE HOŞ GELDİNİZ<br /><br />WELCOME TO VNN OFFICE FURNITURE B2B PANEL <br /><br />" + kul.Unvan;
                return RedirectToAction("menu", "BLogin");
            }
            else
            {
                BLoginHata.Icerik = "GİRİŞ BİLGİLERİ HATALI!!<br /><br />LOGIN INFORMATION IS INCORRECT!!";
                return RedirectToAction("Index");
            }
        }
        public IActionResult Cikis()
        {
            HttpContext.Response.Cookies.Delete("VNNBayiCerez");
            BLoginHata.Icerik = "OTURUM SONLANDIRILDI...<br /><br />SESSION TERMINATED...";
            return RedirectToAction("Index");
        }


        public IActionResult menu()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                ViewBag.bayiid = Cerez;
                ViewBag.hata = BLoginHata.Icerik;
                return View();
            }
        }
        public IActionResult UrunDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                int bayiid = Convert.ToInt32(Cerez);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                string kdvbilgi = "";
                if (bayi.KDVBilgi != null) kdvbilgi = bayi.KDVBilgi.ToString();
                if (bayi.KDVDurum == true) ViewBag.kdv = "10" + " (" + kdvbilgi + ")"; else ViewBag.kdv = "0" + " (" + kdvbilgi + ")";
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString();
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString();
                ViewBag.hata = BLoginHata.Icerik;
                return View();
            }
        }
        public IActionResult Siparis()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                BLoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
                int bayiid = Convert.ToInt32(sip.BayiID);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                string kdvbilgi = "";
                if (bayi.KDVBilgi != null) kdvbilgi = bayi.KDVBilgi.ToString();
                if (bayi.KDVDurum == true) ViewBag.kdv = "10" + " (" + kdvbilgi + ")"; else ViewBag.kdv = "0" + " (" + kdvbilgi + ")";
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString();
                ViewBag.id = id;
                ViewBag.hata = BLoginHata.Icerik;
                return View();
            }
        }
        public IActionResult TeslimatDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "BLogin");
            }
            else
            {
                var sip = c.Teslimats.FirstOrDefault(v => v.ID == id);
                int bayiid = Convert.ToInt32(sip.BayiID);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                if (bayi.KDVDurum == true) ViewBag.kdv = "10"; else ViewBag.kdv = "0";
                string kdvbilgi = "";
                if (bayi.KDVBilgi != null) kdvbilgi = bayi.KDVBilgi.ToString();
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString() + " (" + kdvbilgi + ")";
                ViewBag.id = id;
                ViewBag.id = id;
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
    }
}