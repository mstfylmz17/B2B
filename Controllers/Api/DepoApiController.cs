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
            var sipadimleri = c.SiparisAdimlaris.Where(v => v.SiparisAdimTurlariID == 1 && v.Durum == true && v.GorulduMu != true).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            if (sipadimleri != null)
            {
                foreach (var a in sipadimleri)
                {
                    var x = c.Siparis.FirstOrDefault(v => v.ID == a.SiparisID);
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == x.BayiID);
                    string para = "";
                    if (bayi.ParaBirimi == 2) para = " $"; else para = " ₺";
                    DtoSiparis list = new DtoSiparis();
                    list.ID = Convert.ToInt32(x.ID);
                    list.BayiID = bayi.Unvan.ToString();
                    if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                    if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                    var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                    if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                    list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                    list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                    list.SiparisNo = x.SiparisNo.ToString();
                    if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                    list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
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
        public IActionResult DevamList()
        {
            var depois = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false).ToList();
            List<DtoDepoIsEmirleri> i = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = x.ID;
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                list.BayiID = bayi.BayiKodu.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("d");
                list.UrunKodu = urun.UrunKodu.ToString() + " - " + urun.UrunAdi.ToString();
                list.UrunID = x.UrunID.ToString();
                list.GelenAdet = x.GelenAdet.ToString();
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in ozellik)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    if (o.UrunOzellikTurlariID == 6)
                        list.Ozellikleri = o.OzellikAdi.ToString();
                    else if (o.UrunOzellikTurlariID == 7)
                    {
                        if (o != null) list.Ozellikleri = o.OzellikAdi.ToString();
                        else list.Ozellikleri = "";
                    }
                }
                if (list.Ozellikleri != null) list.Ozellikleri += "Deri Rengi - " + list.Ozellikleri.ToString() + " / ";
                if (list.Ozellikleri != null) list.Ozellikleri += "Ahşap Rengi - " + list.Ozellikleri.ToString() + " / ";
                if (sipic.Aciklama != null) list.Ozellikleri += "Not (Açıklama) - " + sipic.Aciklama.ToString();
                i.Add(list);
            }
            return Json(i.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var depois = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).ToList();
            List<DtoDepoIsEmirleri> i = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = x.ID;
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                list.BayiID = bayi.BayiKodu.ToString();
                list.SiparisNo = sip.SiparisNo.ToString();
                list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("d");
                list.UrunKodu = urun.UrunKodu.ToString() + " - " + urun.UrunAdi.ToString();
                list.UrunID = x.UrunID.ToString();
                list.GelenAdet = x.GelenAdet.ToString();
                var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.SiparisIcerikID && v.Durum == true).ToList();
                foreach (var v in ozellik)
                {
                    var o = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikID);
                    if (o.UrunOzellikTurlariID == 6)
                        list.Ozellikleri = o.OzellikAdi.ToString();
                    else if (o.UrunOzellikTurlariID == 7)
                    {
                        if (o != null) list.Ozellikleri = o.OzellikAdi.ToString();
                        else list.Ozellikleri = "";
                    }
                }
                if (list.Ozellikleri != null) list.Ozellikleri += "Deri Rengi - " + list.Ozellikleri.ToString() + " / ";
                if (list.Ozellikleri != null) list.Ozellikleri += "Ahşap Rengi - " + list.Ozellikleri.ToString() + " / ";
                if (sipic.Aciklama != null) list.Ozellikleri += "Not (Açıklama) - " + sipic.Aciklama.ToString();
                i.Add(list);
            }
            return Json(i.OrderBy(v => v.ID));
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
                        list.OkunmaTarih = Convert.ToDateTime(a.OkunmaTarih).ToString("d");
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
                if (isemri.GelenAdet >= miktar)
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
                    if (sipic.Miktar >= miktar)
                    {
                        Formuller formuller = new Formuller(c);
                        formuller.BoyaSevk(id, kulid, miktar);
                        isemri.GelenAdet -= miktar;
                        isemri.GidenAdet = miktar;
                        isemri.BitirmeDurum = true;
                        isemri.BitisTarihi = DateTime.Now;
                        isemri.KullaniciID = kulid;
                        c.SaveChanges();
                        DepoHata.Icerik = "Boyaya Sevk Başarılı...";
                        return Json(new { status = "success", message = "Boyaya Sevk Başarılı...", redirectUrl = Url.Action("DevamList") });
                    }
                    else
                    {
                        result = new { status = "error", message = "Sevk Edilen Mikar Toplam Sipariş Miktarından Fazla Olamaz...." };
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
            urun.StokMiktari -= Adet;
            c.SaveChanges();
            result = new { status = "success", message = "Hammadde & Hırdavat Çıkarma İşlemi Başarılı" };
            return Json(result);
        }
    }
}
