using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult YetkiTurlari()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult Kategori()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult Kullanici()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {

                List<SelectListItem> departman = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.DepartmanAdi.ToString(),
                                                      Value = x.ID.ToString()
                                                  }).ToList();

                ViewBag.departman = departman;
                ViewBag.hata = ParametreHata.Icerik;
                return View();
            }
        }
        public IActionResult KullaniciDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                List<SelectListItem> departman = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.DepartmanAdi.ToString(),
                                                      Value = x.ID.ToString()
                                                  }).ToList();

                ViewBag.departman = departman;
                List<SelectListItem> yetkiler = (from x in c.Panellers.Where(x => x.Durum == true).ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = x.PanelAdi.ToString(),
                                                     Value = x.ID.ToString()
                                                 }).ToList();

                ViewBag.yetkiler = yetkiler;
                ViewBag.id = id;
                ViewBag.hata = ParametreHata.Icerik;
                var kul = c.Kullanicis.FirstOrDefault(v => v.ID == id);
                DtoKullanici model = new DtoKullanici();
                model.AdSoyad = kul.AdSoyad;
                model.KullaniciAdi = kul.KullaniciAdi;
                model.DepartmanID = c.Departmans.FirstOrDefault(v => v.ID == kul.DepartmanID).DepartmanAdi.ToString();
                model.Adres = kul.Adres;
                model.EPosta = kul.EPosta;
                model.Telefon = kul.Telefon;
                return View(model);
            }
        }
        public IActionResult Ozellik()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public IActionResult OzellikDetay(int id)
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
                ViewBag.hata = ParametreHata.Icerik;
                return View();
            }
        }
        public IActionResult UrunTurlari()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
