using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class SatinAlmaController : Controller
    {
        private readonly Context c;
        public SatinAlmaController(Context context)
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
                List<SelectListItem> birim = (from v in c.Birimlers.Where(v => v.Durum == true).ToList()
                                              select new SelectListItem
                                              {
                                                  Text = v.BirimAdi.ToString(),
                                                  Value = v.ID.ToString()
                                              }).ToList();

                ViewBag.birim = birim;

                List<SelectListItem> urunler = (from v in c.Urunlers.Where(v => v.Durum == true).ToList()
                                                select new SelectListItem
                                                {
                                                    Text = v.UrunKodu.ToString() + " - " + v.UrunAdi.ToString(),
                                                    Value = v.ID.ToString()
                                                }).ToList();

                ViewBag.urunler = urunler;
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Talepler()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                List<SelectListItem> birim = (from v in c.Birimlers.Where(v => v.Durum == true).ToList()
                                              select new SelectListItem
                                              {
                                                  Text = v.BirimAdi.ToString(),
                                                  Value = v.ID.ToString()
                                              }).ToList();

                ViewBag.birim = birim;

                List<SelectListItem> urunler = (from v in c.Urunlers.Where(v => v.Durum == true).ToList()
                                                select new SelectListItem
                                                {
                                                    Text = v.UrunKodu.ToString() + " - " + v.UrunAdi.ToString(),
                                                    Value = v.ID.ToString()
                                                }).ToList();

                ViewBag.urunler = urunler;

                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Aktif()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SatinAlmaHata.Icerik;
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
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Gun()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Hafta()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Ay()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Yil()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
    }
}
