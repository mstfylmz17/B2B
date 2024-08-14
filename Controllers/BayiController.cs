using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class BayiController : Controller
    {
        private readonly Context c;
        public BayiController(Context context)
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
                ViewBag.hata = BayiHata.Icerik;
                return View();
            }
        }
        public IActionResult Detay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                List<SelectListItem> parabirimleri = (from v in c.ParaBirimleris.Where(v => v.Durum == true).ToList()
                                                      select new SelectListItem
                                                      {
                                                          Text = v.ParaBirimAdi.ToString(),
                                                          Value = v.ID.ToString()
                                                      }).ToList();

                ViewBag.parabirimleri = parabirimleri;

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == id);
                DtoBayiler veri = new DtoBayiler();
                if (bayi.Unvan != null) veri.Unvan = bayi.Unvan; else veri.Unvan = "Tanımlı Değil...";
                if (bayi.KullaniciAdi != null) veri.KullaniciAdi = bayi.KullaniciAdi; else veri.KullaniciAdi = "Tanımlı Değil...";
                if (bayi.Sifre != null) veri.Sifre = bayi.Sifre; else veri.Sifre = "Tanımlı Değil...";
                if (bayi.Adres != null) veri.Adres = bayi.Adres; else veri.Adres = "Tanımlı Değil...";
                if (bayi.Telefon != null) veri.Telefon = bayi.Telefon; else veri.Telefon = "Tanımlı Değil...";
                if (bayi.EPosta != null) veri.EPosta = bayi.EPosta; else veri.EPosta = "Tanımlı Değil...";
                if (bayi.Yetkili != null) veri.Yetkili = bayi.Yetkili; else veri.Yetkili = "Tanımlı Değil...";
                if (bayi.IskontoOran != null) veri.IskontoOran = bayi.IskontoOran.ToString(); else veri.IskontoOran = "Tanımlı Değil...";
                if (bayi.AlisVerisLimiti != null) veri.AlisVerisLimiti = bayi.AlisVerisLimiti.ToString(); else veri.AlisVerisLimiti = "Tanımlı Değil...";
                if (bayi.BayiKodu != null) veri.BayiKodu = bayi.BayiKodu.ToString(); else veri.BayiKodu = "Tanımlı Değil...";
                if (bayi.ParaBirimi != null)
                {
                    var para = c.ParaBirimleris.FirstOrDefault(v => v.ID == bayi.ParaBirimi);
                    if (para != null) veri.ParaBirimi = para.ParaBirimAdi.ToString();
                }
                else veri.ParaBirimi = "TL";
                ViewBag.id = id;
                ViewBag.hata = BayiHata.Icerik;
                return View(veri);
            }
        }
    }
}
