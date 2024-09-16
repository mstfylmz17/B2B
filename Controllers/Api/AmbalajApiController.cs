using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;
using VNNB2B.Models;

namespace VNNB2B.Controllers.Api
{
    public class AmbalajApiController : Controller
    {
        private readonly Context c;
        public AmbalajApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult IsEmirleri()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
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
            return Json(ham);
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.IslemdekiAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                string stozellik = "";
                var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in oz)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                    stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                }
                list.Ozellikleri = stozellik;
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).AdSoyad.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("d");
                if (x.BaslamaDurum == true) list.BaslamaDurum = "Ambalaj Başladı..."; else list.BaslamaDurum = "Ambalaja Alınmayı Bekliyor...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult TumList()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.IslemdekiAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                string stozellik = "";
                var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in oz)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                    stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                }
                list.Ozellikleri = stozellik;
                list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).AdSoyad.ToString();
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("d");
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.IslemdekiAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                string stozellik = "";
                var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in oz)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                    stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                }
                list.Ozellikleri = stozellik;
                list.Kullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).AdSoyad.ToString();
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("d");
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult AlList()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.KalanAdet).ToString();
                string stozellik = "";
                var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in oz)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                    stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                }
                list.Ozellikleri = stozellik;
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).AdSoyad.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("d");
                if (x.BaslamaDurum == true) list.BaslamaDurum = "Ambalaj Başladı..."; else list.BaslamaDurum = "Ambalaja Alınmayı Bekliyor...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult CikarList()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.BaslamaDurum == true && v.IslemdekiAdet > 0).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.IslemdekiAdet = Convert.ToInt32(x.IslemdekiAdet).ToString();
                string stozellik = "";
                var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in oz)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                    stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                }
                list.Ozellikleri = stozellik;
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).AdSoyad.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("d");
                if (x.BaslamaDurum == true) list.BaslamaDurum = "Ambalaj Başladı..."; else list.BaslamaDurum = "Ambalaja Alınmayı Bekliyor...";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult KismiList()
        {
            var veri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.IslemdekiAdet > 0 && v.BitirmeDurum == false).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.KalanAdet + x.IslemdekiAdet).ToString();
                string stozellik = "";
                var oz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in oz)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    var tur = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == o.UrunOzellikTurlariID);
                    stozellik += tur.OzellikAdi.ToString() + " (" + o.OzellikAdi.ToString() + ") , ";
                }
                list.Ozellikleri = stozellik;
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).AdSoyad.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("d");
                list.GidenAdet = Convert.ToDecimal(x.GelenAdet).ToString();
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult Okundu(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var isemri = c.AmbalajIsEmirleris.FirstOrDefault(v => v.ID == id);
            isemri.OkunmaTarih = DateTime.Now;
            isemri.GorenPersonel = kulid;
            isemri.GorulduMu = true;
            c.SaveChanges();
            result = new { status = "success", message = "İş Emri Okundu Olarak İşaretlendi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult AmbalajAl(int id, int miktar)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (miktar != null && id != null)
            {
                var isemri = c.AmbalajIsEmirleris.FirstOrDefault(v => v.ID == id);
                if (isemri.GelenAdet >= miktar && (isemri.KalanAdet >= miktar))
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
                    if (sipic.Miktar >= miktar)
                    {
                        isemri.IslemdekiAdet += miktar;
                        isemri.KalanAdet -= miktar;
                        if (isemri.IslemdekiAdet > 0)
                        {
                            isemri.BaslamaDurum = true;
                            isemri.BaslangicTarihi = DateTime.Now;
                        }
                        isemri.KullaniciID = kulid;
                        c.SaveChanges();
                        return Json(new { status = "success", message = "Ambalaja Alma Başarılı...", redirectUrl = Url.Action("DevamList") });
                    }
                    else
                    {
                        result = new { status = "error", message = "Kalan Miktar Toplam Sipariş Miktarından Fazla Olamaz...." };
                    }
                }
                else
                {
                    result = new { status = "error", message = "Kalan Miktar Kabul Edilen Miktardan Fazla Olamaz..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult AmbalajCikar(int id, int miktar)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (miktar != null && id != null)
            {
                var isemri = c.AmbalajIsEmirleris.FirstOrDefault(v => v.ID == id);
                if (isemri.GelenAdet >= miktar && ((isemri.IslemdekiAdet + isemri.KalanAdet) >= miktar))
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
                    isemri.IslemdekiAdet -= miktar;
                    isemri.GidenAdet += miktar;
                    if (isemri.GelenAdet == isemri.GidenAdet)
                    {
                        isemri.BitirmeDurum = true;
                        isemri.BitisTarihi = DateTime.Now;
                        isemri.KullaniciID = kulid;
                    }
                    if (sipic.HazirAdet != null) sipic.HazirAdet += miktar;
                    else sipic.HazirAdet = miktar;
                    sipic.YuklemeyeHazir = true;
                    c.SaveChanges();
                    return Json(new { status = "success", message = "Ambalajdan Çıkarma Başarılı...", redirectUrl = Url.Action("DevamList") });
                }
                else
                {
                    result = new { status = "error", message = "Kalan Miktar Kabul Edilen Miktardan Fazla Olamaz..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
            }
            return Json(result);
        }
    }
}
