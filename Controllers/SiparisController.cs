using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class SiparisController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Context c;
        public SiparisController(Context context, IWebHostEnvironment hostingEnvironment)
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
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult Kaydedilmemis()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
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
                var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
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
        public IActionResult KisaDetay(int id)
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
        public IActionResult KT()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult YH()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult Teslimat()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult Teslimatlar()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult TeslimatDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.id = id;
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public JsonResult FormYazdir(long id)
        {
            DtoSiparis list = new DtoSiparis();
            List<DtoSiparisIcerik> icerik = new List<DtoSiparisIcerik>();

            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            var sipicerik = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
            string para = "";
            if (sip.ParaBirimiID == 1) para = "₺"; else para = "$";
            list.BayiID = bayi.BayiKodu.ToString() + " " + bayi.Unvan.ToString();
            list.Telefon = bayi.Telefon.ToString();
            list.Adres = bayi.Adres.ToString();
            list.ToplamAdet = Convert.ToInt32(sip.ToplamAdet).ToString();
            list.KDVToplam = Convert.ToDecimal(sip.KDVToplam).ToString("N2") + para;
            list.AraToplam = Convert.ToDecimal(sip.AraToplam).ToString("N2") + para;
            list.ToplamTutar = Convert.ToDecimal(sip.ToplamTutar).ToString("N2") + para;
            list.IskontoOran = Convert.ToInt32(bayi.IskontoOran).ToString();
            list.IstoktoToplam = Convert.ToDecimal(sip.IstoktoToplam).ToString("N2") + para;
            list.TeslimTarihi = Convert.ToDateTime(sip.TeslimTarihi).ToString("dd/MM/yyyy");
            list.SiparisNo = sip.SiparisNo.ToString();
            list.Yetkili = bayi.Yetkili.ToString() + " - " + bayi.Telefon.ToString();
            list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy");
            if (c.Teslimats.FirstOrDefault(v => v.SiparisID == id) != null) list.TeslimDurum = "Parçalı Teslimat."; else if (sip.TeslimDurum == true) list.TeslimDurum = "Tam Teslim"; else list.TeslimDurum = "Henüz Teslim Edilmedi...";

            //İçerik

            foreach (var x in sipicerik)
            {
                DtoSiparisIcerik i = new DtoSiparisIcerik();
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID).ToList();
            }

            list.Icerik = icerik;

            using (var stream = new MemoryStream())
            {
                string dosyayolu = "";
                string reportFileName = "siparis.frx";
                string reportFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Report");
                string reportFilePath = Path.Combine(reportFolderPath, reportFileName);

                if (System.IO.File.Exists(reportFilePath))
                {
                    dosyayolu = reportFilePath;
                }

                var webReport = new WebReport();
                webReport.Report.Load(dosyayolu);

                webReport.Report.RegisterData(new List<DtoSiparis> { list }, "Data");

                webReport.Report.RegisterData(icerik, "Icerik");

                webReport.Report.Prepare();
                var export = new PDFSimpleExport();
                webReport.Report.Export(export, stream);

                return Json(new
                {
                    pdfArray = Convert.ToBase64String(stream.ToArray()),
                    fileName = "siparis.pdf"
                });
            }
        }

        //SYS Sipariş Oluşturma
        public IActionResult Yeni()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        [HttpPost]
        public IActionResult YeniSiparis(int id)
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
                Siparis s = new Siparis();
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
                s.SiparisDurum = "Sipariş Onay Bekliyor...";
                s.SiparisNo = bayi.BayiKodu + " " + (c.Siparis.Count() + 1).ToString();
                s.BayiOnay = true;
                c.Siparis.Add(s);
                c.SaveChanges();
                var sonsip = c.Siparis.OrderByDescending(v => v.ID).FirstOrDefault(v =>v.BayiID == id);
                SiparisHata.Icerik = "Sipariş Kaydı Oluşturuldu...";
                return Json(new { status = "success", message = "Sipariş Kaydı Oluşturuldu", redirectUrl = Url.Action("YeniDetay", new { id = sonsip.ID }) });
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
                var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
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
    }
}
