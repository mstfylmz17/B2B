using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models;
using VNNB2B.Models.Hata;

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
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                list.UrunAdi = urun.UrunAdi.ToString();
                list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                    if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                }
                list.BayiID = bayiadi;
                if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";

                string stozellik = "";
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
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
                list.Aciklama = sipic.Aciklama.ToString();
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.ID));
        }
        [HttpPost]
        public IActionResult Okundu(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var isemri = c.DepoIsEmirleris.FirstOrDefault(v => v.ID == id);
            isemri.OkunmaTarih = DateTime.Now;
            isemri.GorenKullanici = kulid;
            isemri.GorulduMu = true;
            isemri.BaslamaDurum = true;
            isemri.BaslangicTarihi = DateTime.Now;
            c.SaveChanges();
            result = new { status = "success", message = "İş Emri Okundu Olarak İşaretlendi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var depois = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true).ToList();
            List<DtoDepoIsEmirleri> i = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
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
                list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.KalanAdet = Convert.ToInt32(x.KalanAdet).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                    if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                }
                list.BayiID = bayiadi;
                if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";

                string stozellik = "";
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
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
                list.Aciklama = sipic.Aciklama.ToString();
                i.Add(list);
            }
            return Json(i.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult AlList()
        {
            var depois = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true && v.GelenAdet > 0).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
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
                list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.KalanAdet = Convert.ToInt32(x.KalanAdet).ToString();
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).KullaniciAdi.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                    if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                }
                list.BayiID = bayiadi;
                if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";

                string stozellik = "";
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
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
                list.Aciklama = sipic.Aciklama.ToString();
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult KismiList()
        {
            var veri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0 && v.IslemdekiAdet > 0).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.KalanAdet = Convert.ToInt32(x.KalanAdet).ToString();
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).KullaniciAdi.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                    if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                }
                list.BayiID = bayiadi;
                if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";

                string stozellik = "";
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
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
                list.Aciklama = sipic.Aciklama.ToString();
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var depois = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
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
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID).KullaniciAdi.ToString();
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.BayiKodu != null) { bayiadi += bayi.BayiKodu.ToString(); }
                    if (bayi.KullaniciAdi != null) bayiadi += " - " + bayi.KullaniciAdi.ToString();
                }
                list.BayiID = bayiadi;
                if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";

                string stozellik = "";
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
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
                list.Aciklama = sipic.Aciklama.ToString();
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult DevamIcList()
        {
            var sipad = c.SiparisAdimlaris.Where(v => v.Durum == true && v.GorulduMu == true && v.SiparisAdimTurlariID == 1).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var a in sipad)
            {
                var veri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.SiparisID == a.SiparisID && v.BitirmeDurum != true).OrderByDescending(v => v.ID).ToList();
                foreach (var x in veri)
                {
                    var varmi = ham.FirstOrDefault(v => v.ID == x.SiparisID);
                    if (varmi == null)
                    {
                        var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                        var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                        var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                        var goren = c.Kullanicis.FirstOrDefault(v => v.ID == a.GorenKullanici);
                        DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                        list.ID = Convert.ToInt32(x.ID);
                        list.GorenKullanici = goren.AdSoyad.ToString();
                        list.OkunmaTarih = Convert.ToDateTime(a.OkunmaTarih).ToString("dd/MM/yyyy");
                        list.SiparisNo = sip.SiparisNo.ToString();
                        list.UrunKodu = urun.UrunKodu.ToString();
                        list.UrunAdi = urun.UrunAdi.ToString();
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
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult BoyaSevk(int id, int miktar)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (miktar != null && id != null)
            {
                var isemri = c.DepoIsEmirleris.FirstOrDefault(v => v.ID == id);
                if (isemri.GelenAdet >= miktar && (isemri.KalanAdet >= miktar))
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
                    if (sipic.Miktar >= miktar)
                    {
                        Formuller formuller = new Formuller(c);
                        formuller.BoyaSevk(id, kulid, miktar);
                        isemri.GidenAdet += miktar;
                        isemri.KalanAdet -= miktar;
                        isemri.IslemdekiAdet = isemri.GelenAdet - isemri.GidenAdet;
                        if (isemri.GelenAdet == isemri.GidenAdet)
                        {
                            isemri.BitirmeDurum = true;
                            isemri.BitisTarihi = DateTime.Now;
                            isemri.KullaniciID = kulid;
                        }
                        isemri.KullaniciID = kulid;
                        c.SaveChanges();
                        return Json(new { status = "success", message = "Boyaya Sevk Başarılı...", redirectUrl = Url.Action("DevamList") });
                    }
                    else
                    {
                        result = new { status = "error", message = "Sevk Edilen Miktar Toplam Sipariş Miktarından Fazla Olamaz...." };
                    }
                }
                else
                {
                    result = new { status = "error", message = "Sevk Edilen Miktar Kabul Edilen Miktardan Fazla Olamaz..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
            }
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
            if (urun.StokMiktari == null) urun.StokMiktari = 0;
            urun.StokMiktari -= Adet;
            c.SaveChanges();
            result = new { status = "success", message = "Hammadde & Hırdavat Çıkarma İşlemi Başarılı" };
            return Json(result);
        }
    }
}
