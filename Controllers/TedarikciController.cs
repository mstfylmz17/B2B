using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class TedarikciController : Controller
    {
        private readonly Context c;
        public TedarikciController(Context context)
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
                ViewBag.hata = TedarikciHata.Icerik;
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
                var bayi = c.Tedarikcilers.FirstOrDefault(v => v.ID == id);
                DtoTedarikciler veri = new DtoTedarikciler();
                if (bayi.Unvan != null) veri.Unvan = bayi.Unvan; else veri.Unvan = "Tanımlı Değil...";
                if (bayi.VergiDairesi != null) veri.VergiDairesi = bayi.VergiDairesi; else veri.VergiDairesi = "Tanımlı Değil...";
                if (bayi.VergiNo != null) veri.VergiNo = bayi.VergiNo; else veri.VergiNo = "Tanımlı Değil...";
                if (bayi.Adres != null) veri.Adres = bayi.Adres; else veri.Adres = "Tanımlı Değil...";
                if (bayi.Telefon != null) veri.Telefon = bayi.Telefon; else veri.Telefon = "Tanımlı Değil...";
                if (bayi.EPosta != null) veri.EPosta = bayi.EPosta; else veri.EPosta = "Tanımlı Değil...";
                if (bayi.Yetkili != null) veri.Yetkili = bayi.Yetkili; else veri.Yetkili = "Tanımlı Değil...";
                if (bayi.TedarikciKodu != null) veri.TedarikciKodu = bayi.TedarikciKodu; else veri.TedarikciKodu = "Tanımlı Değil...";
                if (bayi.FirmaAdi != null) veri.FirmaAdi = bayi.FirmaAdi; else veri.FirmaAdi = "Tanımlı Değil...";
                ViewBag.id = id;
                ViewBag.hata = TedarikciHata.Icerik;
                return View(veri);
            }
        }
    }
}
