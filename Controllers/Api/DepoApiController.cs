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
            var sip = c.Siparis.FirstOrDefault(v => v.Durum == true && v.BayiOnay == false);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            string para = "";
            if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
            if (sip != null)
            {
                var veri = c.SiparisIceriks.Where(v => v.SiparisID == sip.ID && v.Durum == true).OrderByDescending(v => v.ID).ToList();
                List<DtoSiparisIcerik> ham = new List<DtoSiparisIcerik>();
                foreach (var x in veri)
                {
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    list.ID = Convert.ToInt32(x.ID);
                    if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    if (urun.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(urun.Resim);
                    if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "Açıklama Yok!";
                    if (x.Miktar != null) list.Miktar = Convert.ToInt32(x.Miktar).ToString(); else list.Miktar = "1";
                    if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal(x.SatirToplam).ToString("N2") + para; else list.SatirToplam = "0,00" + para;
                    if (x.BirimFiyat != null && x.BirimFiyat > 0) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2") + para; else list.BirimFiyat = "0,00" + para;
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
                    foreach (var v in ozellik)
                    {
                        var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                        if (o.UrunOzellikTurlariID == 6) list.DeriRengi = o.OzellikAdi.ToString(); else if (o.UrunOzellikTurlariID == 7) { if (o != null) list.AhsapRengi = o.OzellikAdi.ToString(); else list.AhsapRengi = ""; }
                    }
                    ham.Add(list);
                }
                return Json(ham.OrderBy(v => v.ID));
            }
            else
            {
                return Json(2);
            }
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
