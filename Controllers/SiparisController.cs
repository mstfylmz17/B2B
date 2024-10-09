using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
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
                if (sip.OnayDurum == true) ViewBag.durum = "Aktif"; else ViewBag.durum = "Pasif";
                ViewBag.id = id;
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public IActionResult KisaDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
        public IActionResult Teslimatlar()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.hata = TeslimatHata.Icerik;
                return View();
            }
        }
        public IActionResult TeslimatDetay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
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
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
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
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
        public JsonResult FormYazdir(long id)
        {
            var x = c.Siparis.FirstOrDefault(v => v.ID == id);
            string siparisno = "Sipariş No Belitrilmemiş...";

            if (x.SiparisNo != null) siparisno = x.SiparisNo.ToString();

            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
            string para = "";
            if (x.ParaBirimiID == 2) para = " $"; else para = " ₺";
            DtoSiparis rapor = new DtoSiparis();
            rapor.ID = Convert.ToInt32(x.ID);
            rapor.IskontoOran = Convert.ToInt32(bayi.IskontoOran).ToString();
            if (x.SiparisTarihi != null) rapor.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("dd/MM/yyyy");
            if (x.TeslimTarihi != null) rapor.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy");
            if (x.SiparisBayiAciklama != null) rapor.SiparisBayiAciklama = x.SiparisBayiAciklama; else rapor.SiparisBayiAciklama = "";
            var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
            if (x.TeslimTarihi != null && kismivarmi != null) rapor.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("dd/MM/yyyy"); else if (x.TeslimTarihi == null && kismivarmi == null) rapor.TeslimTarihi = "Teslim Edilmedi..."; else rapor.TeslimTarihi = "Kısmi Teslimatlar Var...";
            rapor.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
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
            if (x.TeslimatSekli != null) rapor.TeslimatSekli = x.TeslimatSekli.ToString(); else rapor.TeslimatSekli = "";
            if (x.TerminHaftasi != null) rapor.TerminHaftasi = x.TerminHaftasi.ToString(); else rapor.TerminHaftasi = "";
            var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
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

            var veri = c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).ToList();
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
                    if (a.SatirToplam != null && a.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((a.BirimFiyat * a.Miktar) - (((a.BirimFiyat * bayi.IskontoOran) / 100) * a.Miktar)).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                }
                else
                {
                    if (a.SatirToplam != null && a.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal((a.BirimFiyat) * a.Miktar).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                }
                if (a.BirimFiyat != null && a.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(a.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                if (a.BirimFiyat != null && a.BirimFiyat > 0) list.indirimFiyat = Convert.ToDecimal(a.BirimFiyat - ((a.BirimFiyat * bayi.IskontoOran) / 100)).ToString("N2") + para; else list.indirimFiyat = "0,00" + para;
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == a.ID && v.Durum == true).ToList();
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
                string reportFileName = "siparis.frx";
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
                string pdfFileName = siparisno + ".pdf";
                string pdfFilePath = Path.Combine(reportFolderPath, pdfFileName);
                System.IO.File.WriteAllBytes(pdfFilePath, stream.ToArray());

                string pdfFileUrl = Url.Content($"~/Report/{pdfFileName}");

                return Json(new { pdfUrl = pdfFileUrl, fileName = pdfFileName });
            }
        }
        public JsonResult TeslimatYazdir(long id)
        {
            var tes = c.Teslimats.FirstOrDefault(v => v.ID == id);
            string testar = "Teslimat Tarihi Belirtilmemiş";
            if (tes.TeslimatTarihi != null) testar = Convert.ToDateTime(tes.TeslimatTarihi).ToString("dd/MM/yyyy");
            var sip = c.Siparis.FirstOrDefault(a => a.ID == tes.SiparisID);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == tes.BayiID);
            string para = "";
            if (sip.ParaBirimiID == 2) para = " $"; else para = " ₺";
            DtoTeslimat list = new DtoTeslimat();
            list.ID = Convert.ToInt32(tes.ID);
            if (tes.TeslimatTarihi != null) list.TeslimatTarihi = Convert.ToDateTime(tes.TeslimatTarihi).ToString("dd/MM/yyyy"); else list.TeslimatTarihi = "";
            if (tes.AracPlaka != null) list.AracPlaka = tes.AracPlaka.ToString(); else list.AracPlaka = "";
            if (tes.TeslimAlan != null) list.TeslimAlan = tes.TeslimAlan.ToString(); else list.TeslimAlan = "";
            if (tes.TeslimEden != null) list.TeslimEden = tes.TeslimEden.ToString(); else list.TeslimEden = "";
            if (tes.TeslimatNo != null) list.TeslimatNo = tes.TeslimatNo.ToString(); else list.TeslimatNo = "";
            if (tes.TeslimSekli != null) list.TeslimSekli = tes.TeslimSekli.ToString(); else list.TeslimSekli = "";
            if (tes.TeslimAdres != null) list.TeslimAdres = tes.TeslimAdres.ToString(); else list.TeslimAdres = "";
            if (tes.KullaniciID != null) list.KullaniciID = c.Kullanicis.FirstOrDefault(v => v.ID == tes.KullaniciID).AdSoyad.ToString(); else list.KullaniciID = "";

            if (bayi.Unvan != null) list.FaturaUnvan = bayi.Unvan.ToString(); else list.FaturaUnvan = "";
            if (bayi.KullaniciAdi != null) list.MusteriAdi = bayi.KullaniciAdi.ToString(); else list.MusteriAdi = "";
            if (bayi.Telefon != null) list.Telefon = bayi.Telefon.ToString(); else list.Telefon = "";
            if (bayi.Adres != null) list.Adres = bayi.Adres.ToString(); else list.Adres = "";

            var icerik = c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).ToList();
            decimal toplamm3 = 0;
            decimal toplamkg = 0;
            decimal toplampaketadet = 0;
            decimal indirimlifiyat = 0;
            decimal KDVToplam = 0;
            decimal ToplamTutar = 0;
            decimal AraToplam = 0;
            decimal IstoktoToplam = 0;
            decimal teslimmiktar = Convert.ToDecimal(c.TeslimatIceriks.Where(v => v.TeslimatID == id && v.Durum == true).Sum(v => v.Miktar));
            foreach (var v in icerik)
            {
                var fiyat = c.UrunFiyatlaris.FirstOrDefault(a => a.UrunID == v.UrunID && a.Durum == true);
                decimal? birimfiyat = 0;
                if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                else { birimfiyat = 0; sip.ParaBirimiID = 1; }


                var urun = c.Urunlers.FirstOrDefault(a => a.ID == v.UrunID);
                if (urun.BirimKG != null) toplamkg += Convert.ToDecimal(urun.BirimKG * v.Miktar);
                if (urun.BirimM3 != null) toplamm3 += Convert.ToDecimal(urun.BirimM3 * v.Miktar);
                if (urun.PaketAdet != null) toplampaketadet += Convert.ToDecimal(urun.PaketAdet * v.Miktar);


                IstoktoToplam += Convert.ToDecimal(((birimfiyat * v.Miktar) * sip.IskontoOran) / 100);
                AraToplam += Convert.ToDecimal(birimfiyat * v.Miktar);
            }
            if (bayi.KDVDurum == true)
            {
                decimal kdv = Convert.ToDecimal((((AraToplam + KDVToplam) - IstoktoToplam) * 10) / 100);
                KDVToplam += kdv;
            }
            else
            {
                KDVToplam = 0;
            }
            list.indirimlifiyat = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
            list.KDVToplam = KDVToplam.ToString("N2") + para;
            list.ToplamTutar = ((AraToplam + KDVToplam) - IstoktoToplam).ToString("N2") + para;
            list.AraToplam = AraToplam.ToString("N2") + para;
            list.IstoktoToplam = IstoktoToplam.ToString("N2") + para;

            list.ToplamM3 = toplamm3.ToString("N2");
            list.ToplamKG = toplamkg.ToString("N2");
            list.ToplamParca = toplampaketadet.ToString("N2");
            list.ToplamAdet = teslimmiktar.ToString();
            List<DtoTeslimatIcerik> ham = new List<DtoTeslimatIcerik>();
            foreach (var x in icerik)
            {
                var sippic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sipp = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                DtoTeslimatIcerik listt = new DtoTeslimatIcerik();
                list.ID = Convert.ToInt32(x.ID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (sip.SiparisBayiAciklama != null) bayiadi = sip.SiparisBayiAciklama.ToString();
                }
                listt.BayiID = bayiadi;
                listt.SiparisNo = sip.SiparisNo.ToString();
                listt.UrunID = x.UrunID.ToString();
                listt.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy");
                if (urun.UrunKodu != null) listt.UrunKodu = urun.UrunKodu.ToString(); else listt.UrunKodu = "";
                if (urun.UrunAdi != null) listt.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString() + " / " + urun.UrunAciklama.ToString(); else listt.UrunAciklama = "";
                if (urun.Boyut != null) listt.UrunAciklama += " \n " + urun.Boyut.ToString();
                if (sip != null) listt.SiparisID = sip.SiparisNo.ToString();
                if (urun.Resim != null) listt.Resimbi = urun.Resim;
                listt.SiparisMiktari = sipic.Miktar.ToString();
                listt.Miktar = x.Miktar.ToString();
                listt.HazirMiktar = sipic.HazirAdet.ToString();
                listt.KalanMiktar = (sipic.Miktar - sipic.TeslimAdet).ToString();
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                if (bayi.IskontoOran > 0)
                {
                    if (sippic.SatirToplam != null && sippic.SatirToplam > 0) listt.SatirToplam = Convert.ToDecimal(((sippic.BirimFiyat * bayi.IskontoOran) / 100) * x.Miktar).ToString("N2") + para; else listt.SatirToplam = "0,00" + para;
                }
                else
                {
                    if (sippic.SatirToplam != null && sippic.SatirToplam > 0) listt.SatirToplam = Convert.ToDecimal((sippic.BirimFiyat) * x.Miktar).ToString("N2") + para; else listt.SatirToplam = "0,00" + para;
                }
                if (sippic.BirimFiyat != null && sippic.BirimFiyat > 0) listt.BirimFiyat = Convert.ToDecimal(sippic.BirimFiyat).ToString("N2") + para; else listt.BirimFiyat = "0,00" + para;
                if (sippic.BirimFiyat != null && sippic.BirimFiyat > 0) listt.indirimFiyat = Convert.ToDecimal((sippic.BirimFiyat * bayi.IskontoOran) / 100).ToString("N2") + para; else listt.indirimFiyat = "0,00" + para;

                foreach (var v in ozellik)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    if (o.UrunOzellikTurlariID == 6)
                    {
                        if (o != null) listt.DeriRengi = o.OzellikAdi.ToString();
                        else listt.DeriRengi = "";
                    }
                    else if (o.UrunOzellikTurlariID == 7)
                    {
                        listt.DeriRengi = "";
                        if (o != null) listt.AhsapRengi = o.OzellikAdi.ToString();
                        else listt.AhsapRengi = "";
                    }
                    else
                    {
                        listt.AhsapRengi = "";
                    }
                }
                if (sipic.Aciklama != null) if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = ""; else list.Aciklama = "";
                ham.Add(listt);
            }
            list.Icerik = ham;

            using (var stream = new MemoryStream())
            {
                string dosyayolu = "";
                string reportFileName = "teslimat.frx";
                string reportFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Report");
                string reportFilePath = Path.Combine(reportFolderPath, reportFileName);

                if (System.IO.File.Exists(reportFilePath))
                {
                    dosyayolu = reportFilePath;
                }

                var webReport = new WebReport();
                webReport.Report.Load(dosyayolu);

                webReport.Report.RegisterData(new List<DtoTeslimat> { list }, "Data");

                webReport.Report.RegisterData(icerik, "Icerik");

                webReport.Report.Prepare();
                var export = new PDFSimpleExport();
                webReport.Report.Export(export, stream);

                // PDF dosyasını sunucuda saklama
                string pdfFileName = testar + ".pdf";
                string pdfFilePath = Path.Combine(reportFolderPath, pdfFileName);
                System.IO.File.WriteAllBytes(pdfFilePath, stream.ToArray());

                string pdfFileUrl = Url.Content($"~/Report/{pdfFileName}");

                return Json(new { pdfUrl = pdfFileUrl, fileName = pdfFileName });
            }
        }

        //SYS Sipariş Oluşturma
        public IActionResult Yeni()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
            if (Cerez == null && Cerez == "" && Cerez == "0")
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
                s.SiparisNo = "";
                s.BayiOnay = true;
                c.Siparis.Add(s);
                c.SaveChanges();
                var sonsip = c.Siparis.OrderByDescending(v => v.ID).FirstOrDefault(v => v.BayiID == id);
                SiparisHata.Icerik = "Sipariş Kaydı Oluşturuldu...";
                return Json(new { status = "success", message = "Sipariş Kaydı Oluşturuldu", redirectUrl = Url.Action("YeniDetay", new { id = sonsip.ID }) });
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
                var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
                int bayiid = Convert.ToInt32(sip.BayiID);
                var bayi = c.Bayilers.FirstOrDefault(b => b.ID == bayiid);
                string kdvbilgi = "";
                if (bayi.KDVBilgi != null) kdvbilgi = bayi.KDVBilgi.ToString();
                if (bayi.KDVDurum == true) ViewBag.kdv = "10" + " (" + kdvbilgi + ")"; else ViewBag.kdv = "0" + " (" + kdvbilgi + ")";
                ViewBag.iskonto = Convert.ToInt32(bayi.IskontoOran).ToString();
                if (sip.OnayDurum == true) ViewBag.durum = "Aktif"; else ViewBag.durum = "Pasif";
                ViewBag.id = id;
                ViewBag.hata = SiparisHata.Icerik;
                return View();
            }
        }
    }
}
