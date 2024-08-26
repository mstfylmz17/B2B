using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models;

namespace VNNB2B.Controllers.Api
{
    public class DepoApiController : Controller
    {
        private readonly Context c;
        public DepoApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult IsEmirleri()
        {
            var veri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var varmi = ham.FirstOrDefault(v => v.ID == x.SiparisID);
                if (varmi == null)
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                    list.ID = Convert.ToInt32(x.ID);
                    list.SiparisNo = sip.SiparisNo.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    list.UrunAdi = urun.UrunAdi.ToString();
                    if (urun.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();
                    string stozellik = "";
                    var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                    foreach (var v in oz)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                        stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                    }
                    list.Ozellikleri = stozellik;
                    ham.Add(list);
                }
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult GecmisIsEmirleri()
        {
            var veri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var varmi = ham.FirstOrDefault(v => v.ID == x.SiparisID);
                if (varmi == null)
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                    list.ID = Convert.ToInt32(x.ID);
                    list.SiparisNo = sip.SiparisNo.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    list.UrunAdi = urun.UrunAdi.ToString();
                    if (urun.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();
                    string stozellik = "";
                    var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                    foreach (var v in oz)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                        stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                    }
                    list.Ozellikleri = stozellik;
                    if (x.BaslangicTarihi != null) list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("g"); else list.BaslangicTarihi = "Veri Bulunamaduı...";
                    if (x.BitisTarihi != null) list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("g"); else list.BitisTarihi = "Veri Bulunamaduı...";
                    if (x.KullaniciID != null) list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).AdSoyad.ToString(); else list.Kullanici = "Veri Bulunamadı...";
                    ham.Add(list);
                }
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult Okundu(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            DepoIsEmirleri de = c.DepoIsEmirleris.FirstOrDefault(v => v.ID == id);
            de.GorulduMu = true;
            de.GorenKullanici = kulid;
            de.OkunmaTarih = DateTime.Now;
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Başarılı..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var veri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum != true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var varmi = ham.FirstOrDefault(v => v.ID == x.SiparisID);
                if (varmi == null)
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var goren = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici);
                    DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                    list.ID = Convert.ToInt32(x.ID);
                    list.GorenKullanici = goren.AdSoyad.ToString();
                    list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("d");
                    list.SiparisNo = sip.SiparisNo.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    list.UrunAdi = urun.UrunAdi.ToString();
                    if (urun.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();
                    string stozellik = "";
                    var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                    foreach (var v in oz)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                        stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                    }
                    list.Ozellikleri = stozellik;
                    ham.Add(list);
                }
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult BoyaSevk(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            Formuller formuller = new Formuller(c);
            formuller.BoyaSevk(id, kulid);
            result = new { status = "success", message = "Boyaya Sevk Başarılı..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult UrunSevk(int id, decimal Adet, string? Aciklama)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var urun = c.Urunlers.FirstOrDefault(v => v.ID == id);
            UrunCikislari cik = new UrunCikislari();
            cik.UrunID = id;
            cik.Aciklama = Aciklama;
            cik.Miktar = Adet;
            cik.Durum = true;
            cik.KullaniciID = kulid;
            cik.Tarih = DateTime.Now;
            c.UrunCikislaris.Add(cik);
            urun.StokMiktari -= Adet;
            c.SaveChanges();
            result = new { status = "success", message = "Hammadde & Hırdavat Çıkarma İşlemi Başarılı" };
            return Json(result);
        }
    }
}
