using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using FastReport.Export.PdfSimple;
using FastReport.Web;
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
                s.SiparisNo = bayi.BayiKodu + " - T - " + (c.Teklifs.Count() + 1).ToString();
                s.BayiOnay = true;
                c.Teklifs.Add(s);
                c.SaveChanges();
                var sonsip = c.Teklifs.OrderByDescending(v => v.ID).FirstOrDefault(v => v.BayiID == id);
                TeklifHata.Icerik = "Teklif Kaydı Oluşturuldu...";
                return Json(new { status = "success", message = "Teklif Kaydı Oluşturuldu", redirectUrl = Url.Action("YeniDetay", new { id = sonsip.ID }) });
                //return RedirectToAction("YeniDetay", new { id = sonsip.ID });
            }
        }
        public IActionResult YeniDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
        public JsonResult FormYazdir(long id)
        {
            var x = c.Teklifs.FirstOrDefault(v => v.ID == id);

            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
            string para = "";
            if (x.ParaBirimiID == 2) para = " $"; else para = " ₺";
            DtoSiparis rapor = new DtoSiparis();
            rapor.ID = Convert.ToInt32(x.ID);
            if (x.SiparisTarihi != null) rapor.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
            if (x.TeslimTarihi != null) rapor.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy");
            if (x.SiparisBayiAciklama != null) rapor.SiparisBayiAciklama = x.SiparisBayiAciklama; else rapor.SiparisBayiAciklama = "";
            var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
            if (x.TeslimTarihi != null && kismivarmi != null) rapor.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) rapor.TeslimTarihi = "Teslim Edilmedi..."; else rapor.TeslimTarihi = "Kısmi Teslimatlar Var...";
            rapor.ToplamTeslimEdilen = Convert.ToInt32(c.TeklifIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
            rapor.SiparisNo = x.SiparisNo.ToString();
            if (x.SiparisDurum != null) rapor.SiparisDurum = x.SiparisDurum.ToString(); else rapor.SiparisDurum = "";
            rapor.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
            rapor.SiparisBayiAciklama = x.SiparisBayiAciklama.ToString();
            if (x.TerminTarihi != null) rapor.TerminTarihi = Convert.ToDateTime(x.TerminTarihi).ToString("dd/MM/yyyy"); else rapor.TerminTarihi = "HENÜZ BELİRTİLMEDİ / NOT YET SPECIFIED";
            rapor.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
            rapor.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
            rapor.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
            rapor.DosyaYolu = x.DosyaYolu;
            rapor.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
            if (x.OnaylayanID != null && x.OnaylayanID > 0)
                rapor.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.OnaylayanID).AdSoyad.ToString();
            else
                rapor.Kullanici = "";
            rapor.indirimlifiyat = Convert.ToDecimal(x.AraToplam - x.IstoktoToplam).ToString("N2") + para;
            if (bayi.Yetkili != null) rapor.Yetkili = bayi.Yetkili.ToString(); else rapor.Yetkili = "";
            if (bayi.Unvan != null) rapor.Unvan = bayi.Unvan.ToString(); else rapor.Unvan = "";
            if (bayi.KullaniciAdi != null) rapor.FirmaAdi = bayi.KullaniciAdi.ToString(); else rapor.FirmaAdi = "";
            if (bayi.Telefon != null) rapor.Telefon = bayi.Telefon.ToString(); else rapor.Telefon = "";
            if (bayi.Adres != null) rapor.Adres = bayi.Adres.ToString(); else rapor.Adres = "";
            var icerik = c.TeklifIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
            decimal toplamm3 = 0;
            decimal toplamkg = 0;
            decimal toplampaketadet = 0;
            foreach (var v in icerik)
            {
                var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                if (urun.BirimKG != null) toplamkg += Convert.ToDecimal(urun.BirimKG * v.Miktar);
                if (urun.BirimM3 != null) toplamm3 += Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                if (urun.PaketAdet != null) toplampaketadet += Convert.ToDecimal(urun.PaketAdet * v.Miktar);
            }
            rapor.toplamm3 = toplamm3.ToString("N2");
            rapor.toplamkg = toplamkg.ToString("N2");
            rapor.toplamparcaadet = toplampaketadet.ToString("N2");

            var veri = c.TeklifIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
            List<DtoSiparisIcerik> ham = new List<DtoSiparisIcerik>();
            foreach (var a in veri)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == a.UrunID);
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                DtoSiparisIcerik list = new DtoSiparisIcerik();
                list.ID = Convert.ToInt32(x.ID);
                if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "";
                if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "";
                if (urun.Boyut != null)
                    list.UrunAciklama += " \n " + urun.Boyut.ToString();
                if (urun.Resim != null)
                {
                    list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    list.ResimBi = urun.Resim;
                }
                if (a.Aciklama != null) list.Aciklama = a.Aciklama.ToString(); else list.Aciklama = "Açıklama Yok!";
                list.Miktar = Convert.ToInt32(a.Miktar).ToString();
                list.KalanMiktar = Convert.ToInt32(a.Miktar - a.TeslimAdet).ToString();
                if (a.TeslimAdet != null) list.TeslimAdet = Convert.ToInt32(a.TeslimAdet).ToString(); else list.TeslimAdet = "0";
                if (bayi.IskontoOran > 0)
                {
                    if (a.SatirToplam != null && a.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal(((a.BirimFiyat * bayi.IskontoOran) / 100) * a.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                }
                else
                {
                    if (a.SatirToplam != null && a.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((a.BirimFiyat) * a.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                }
                if (a.BirimFiyat != null && a.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(a.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                if (a.BirimFiyat != null && a.BirimFiyat > 0) list.indirimFiyat = Convert.ToDecimal((a.BirimFiyat * bayi.IskontoOran) / 100).ToString("N2") + para; else list.indirimFiyat = "0,00" + para;
                var ozellik = c.TeklifIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == a.ID && v.Durum == true).ToList();
                foreach (var v in ozellik)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    if (o.UrunOzellikTurlariID == 6)
                    {
                        if (o != null) list.DeriRengi = o.OzellikAdi.ToString();
                        else list.DeriRengi = "";
                    }
                    else if (o.UrunOzellikTurlariID == 7)
                    {
                        list.DeriRengi = "";
                        if (o != null) list.AhsapRengi = o.OzellikAdi.ToString();
                        else list.AhsapRengi = "";
                    }
                    else
                    {
                        list.AhsapRengi = "";
                    }
                }
                ham.Add(list);
            }
            rapor.Icerik = ham;
            using (var stream = new MemoryStream())
            {
                string dosyayolu = "";
                string reportFileName = "teklif.frx";
                string reportFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Report");
                string reportFilePath = Path.Combine(reportFolderPath, reportFileName);

                if (System.IO.File.Exists(reportFilePath))
                {
                    dosyayolu = reportFilePath;
                }

                var webReport = new WebReport();
                webReport.Report.Load(dosyayolu);

                webReport.Report.RegisterData(new List<DtoSiparis> { rapor }, "Data");

                webReport.Report.RegisterData(icerik, "Icerik");

                webReport.Report.Prepare();
                var export = new PDFSimpleExport();
                webReport.Report.Export(export, stream);

                // PDF dosyasını sunucuda saklama
                string pdfFileName = "teklif.pdf";
                string pdfFilePath = Path.Combine(reportFolderPath, pdfFileName);
                System.IO.File.WriteAllBytes(pdfFilePath, stream.ToArray());

                string pdfFileUrl = Url.Content($"~/Report/{pdfFileName}");

                return Json(new { pdfUrl = pdfFileUrl, fileName = pdfFileName });
            }
        }
    }
}
