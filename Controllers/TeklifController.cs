using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class TeklifController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Context c;
        public TeklifController(Context context, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
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
                ViewBag.hata = TeklifHata.Icerik;
                return View();
            }
        }
        [HttpPost]
        public IActionResult YeniTeklif(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == id);
                Teklif s = new Teklif();
                s.BayiID = id;
                s.SiparisBayiAciklama = "";
                s.SiparisTarihi = DateTime.Now;
                s.ToplamAdet = 0;
                s.ToplamTeslimEdilen = 0;
                s.ToplamTutar = 0;
                s.IskontoOran = 0;
                s.IstoktoToplam = 0;
                s.AraToplam = 0;
                s.KDVToplam = 0;
                s.OnayDurum = false;
                s.OnayAciklama = "";
                s.Durum = false;
                s.SiparisDurum = "Teklif Onay Bekliyor...";
                s.SiparisNo = bayi.BayiKodu + " " + (c.Siparis.Count() + 1).ToString();
                s.BayiOnay = true;
                c.Teklifs.Add(s);
                c.SaveChanges();
                var sonsip = c.Siparis.OrderByDescending(v => v.ID).FirstOrDefault(v => v.BayiID == id);
                TeklifHata.Icerik = "Teklif Kaydı Oluşturuldu...";
                return Json(new { status = "success", message = "Teklif Kaydı Oluşturuldu", redirectUrl = Url.Action("YeniDetay", new { id = sonsip.ID }) });
                //return RedirectToAction("YeniDetay", new { id = sonsip.ID });
            }
        }
        public IActionResult YeniDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var sip = c.Teklifs.FirstOrDefault(v => v.ID == id);
                int bayiid = Convert.ToInt32(sip.BayiID);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                if (bayi.KDVDurum == true) ViewBag.kdv = "10"; else ViewBag.kdv = "0";
                string kdvbilgi = "";
                if (bayi.KDVBilgi != null) kdvbilgi = bayi.KDVBilgi.ToString();
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString() + " (" + kdvbilgi + ")";
                ViewBag.id = id;
                ViewBag.hata = TeklifHata.Icerik;
                return View();
            }
        }
    }
}
