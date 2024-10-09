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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var ted = c.Tedarikcilers.Where(v => v.Durum == true).ToList().OrderBy(v => v.TedarikciKodu);
                List<SelectListItem> tedarikciler = new List<SelectListItem>();
                foreach (var x in ted)
                {
                    string text = "";
                    if (x.TedarikciKodu != null) text = x.TedarikciKodu.ToString();
                    if (x.Unvan != null) text += " - " + x.Unvan.ToString();
                    if (x.FirmaAdi != null) text += " - " + x.FirmaAdi.ToString();
                    SelectListItem list = new SelectListItem();
                    list.Value = x.ID.ToString();
                    list.Text = text;
                    tedarikciler.Add(list);
                }
                ViewBag.tedarikciler = tedarikciler;
                List<SelectListItem> parabirimi = (from x in c.ParaBirimleris.Where(x => x.Durum == true).ToList()
                                                   select new SelectListItem
                                                   {
                                                       Text = x.ParaBirimAdi.ToString(),
                                                       Value = x.ID.ToString()
                                                   }).ToList();
                ViewBag.parabirimi = parabirimi;
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Talepler()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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

                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Bekleyen()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult Devam()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult DevamDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.id = id;
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult MalKabul()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult MalKabulDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.id=id;
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Detay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.id = id;
                ViewBag.hata = SatinAlmaHata.Icerik;
                return View();
            }
        }
        public IActionResult Aktif()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
