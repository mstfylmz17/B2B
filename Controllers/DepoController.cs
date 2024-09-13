using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class DepoController : Controller
    {
        private readonly Context c;
        public DepoController(Context context)
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
                ViewBag.hata = DepoHata.Icerik;
                return View();
            }
        }
        public IActionResult SDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
                var adim = c.SiparisAdimlaris.FirstOrDefault(v => v.SiparisID == id);
                if (adim.GorulduMu != true)
                {
                    adim.GorulduMu = true;
                    adim.GorenKullanici = Convert.ToInt32(Cerez);
                    adim.OkunmaTarih = DateTime.Now;
                    c.SaveChanges();
                }
                int bayiid = Convert.ToInt32(sip.BayiID);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                if (bayi.KDVDurum == true) ViewBag.kdv = "10"; else ViewBag.kdv = "0";
                string kdvbilgi = "";
                if (bayi.KDVBilgi != null) kdvbilgi = bayi.KDVBilgi.ToString();
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString() + " (" + kdvbilgi + ")";
                ViewBag.id = id;
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult Kismi()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = DepoHata.Icerik;
                return View();
            }
        }
        public IActionResult Gecmis()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = DepoHata.Icerik;
                return View();
            }
        }
        public IActionResult Devam()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = DepoHata.Icerik;
                return View();
            }
        }
        public IActionResult BoyaSevk()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = DepoHata.Icerik;
                return View();
            }
        }
        public IActionResult UrunSevk()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = UrunHata.Icerik;
                return View();
            }
        }
    }
}
