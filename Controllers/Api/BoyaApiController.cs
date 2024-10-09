using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models.Hata;
using VNNB2B.Models;
using System.Collections.Generic;

namespace VNNB2B.Controllers.Api
{
    public class BoyaApiController : Controller
    {
        private readonly Context c;
        public BoyaApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult IsEmirleri()
        {
            var veri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).OrderByDescending(v => v.ID).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var veri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && (v.BitirmeDurum != true || (v.BitirmeDurum == true && v.GelenAdet != v.SiparisAdet))).OrderByDescending(v => v.ID).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var veri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BaslamaDurum == true && (v.GelenAdet == v.SiparisAdet) && v.GelenAdet == v.GidenAdet).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                if (sipic.Miktar == x.GelenAdet && x.KalanAdet == 0)
                {
                    DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                    list.ID = Convert.ToInt32(x.ID);
                    list.UrunID = x.UrunID.ToString();
                    list.SiparisNo = sip.SiparisNo.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                    list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                    list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                    list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                    list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                    if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                    list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                    list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                    list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                    string bayiadi = "";
                    if (bayi != null)
                    {
                        if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                        if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                    if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                    ham.Add(list);
                }
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult AlList()
        {
            var veri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0).OrderByDescending(v => v.ID).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                list.IslemdekiAdet = Convert.ToInt32(x.KalanAdet).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult CikarList()
        {
            var veri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BaslamaDurum == true && v.IslemdekiAdet > 0).OrderByDescending(v => v.ID).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                list.IslemdekiAdet = Convert.ToInt32(x.IslemdekiAdet).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult KismiList()
        {
            var veri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && v.GidenAdet > 0).OrderByDescending(v => v.ID).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult Okundu([FromBody] List<int> selectedIds)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            foreach (var id in selectedIds)
            {
                var isemri = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == id);
                isemri.OkunmaTarih = DateTime.Now;
                isemri.GorenKullanici = kulid;
                isemri.GorulduMu = true;
                c.SaveChanges();
            }
            result = new { status = "success", message = "Seçilen İş Emirleri Okundu Olarak İşaretlendi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult BoyaAl([FromBody] List<SelectedItem> selectedItems)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };

            if (selectedItems != null)
            {
                foreach (var x in selectedItems)
                {
                    int miktar = Convert.ToInt32(x.ExtraInfo);
                    var isemri = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == x.Id);
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
                            result = new { status = "success", message = "Boyaya Alma Başarılı..." };
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
            }
            else
            {
                result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult BoyaCikar([FromBody] List<SelectedItem2> selectedItems)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (selectedItems != null)
            {
                foreach (var x in selectedItems)
                {
                    int miktar = Convert.ToInt32(x.ExtraInfo);
                    string Istasyon = x.selectedOption;
                    var isemri = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == x.Id);
                    if (isemri.GelenAdet >= miktar && ((isemri.IslemdekiAdet) >= miktar))
                    {
                        var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
                        if (Istasyon == "Döşeme")
                        {
                            Formuller f = new Formuller(c);
                            f.DosemeSevk(x.Id, miktar);
                        }
                        else
                        {
                            Formuller f = new Formuller(c);
                            f.AmbalajSevk(x.Id, kulid, "Boya", miktar);
                        }
                        isemri.IslemdekiAdet -= miktar;
                        isemri.GidenAdet += miktar;
                        if (isemri.GelenAdet == isemri.GidenAdet)
                        {
                            isemri.BitirmeDurum = true;
                            isemri.BitisTarihi = DateTime.Now;
                            isemri.KullaniciID = kulid;
                        }
                        c.SaveChanges();
                        result = new { status = "success", message = "Boyadan Çıkarma Başarılı..." };
                    }
                    else
                    {
                        result = new { status = "error", message = "Kalan Miktar Kabul Edilen Miktardan Fazla Olamaz..." };
                    }
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult GeriGetir([FromBody] List<SelectedItem2> selectedItems)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (selectedItems != null)
            {
                foreach (var x in selectedItems)
                {
                    int miktar = Convert.ToInt32(x.ExtraInfo);
                    string Istasyon = x.selectedOption;
                    var ise = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == x.Id);
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == ise.SiparisIcerikID);

                    if (Istasyon == "Döşeme")
                    {
                        var isemri = c.DosemeIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == ise.SiparisIcerikID && v.UrunID == ise.UrunID);
                        if (isemri != null)
                        {
                            if (isemri.GelenAdet >= miktar)
                            {
                                Formuller f = new Formuller(c);
                                f.DosemeSevkGeri(x.Id, miktar);

                                isemri.BitirmeDurum = false;

                                c.SaveChanges();
                                result = new { status = "success", message = "Boyaya Geri Alma Başarılı..." };
                            }
                            else
                            {
                                result = new { status = "error", message = "Geri Alınan Miktar Kabul Edilen Miktardan Fazla Olamaz..." };
                            }
                        }
                        else
                        {
                            result = new { status = "error", message = "Döşemede Seçilen Ürüne Ait Herhangi Bir Kayıt Bulunamadı..." };
                        }
                    }
                    else
                    {
                        var isemri = c.AmbalajIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == ise.SiparisIcerikID && v.UrunID == ise.UrunID);
                        if (isemri != null)
                        {
                            if (isemri.GelenAdet >= miktar)
                            {
                                Formuller f = new Formuller(c);
                                f.AmbalajSevkGeri(x.Id, miktar);

                                isemri.BitirmeDurum = false;
                                c.SaveChanges();
                                result = new { status = "success", message = "Boyaya Geri Alma Başarılı..." };
                            }
                            else
                            {
                                result = new { status = "error", message = "Geri Alınan Miktar Kabul Edilen Miktardan Fazla Olamaz..." };
                            }
                        }
                        else
                        {
                            result = new { status = "error", message = "Ambalajda Seçilen Ürüne Ait Herhangi Bir Kayıt Bulunamadı..." };
                        }
                    }
                }
            }
            else
            {
                result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
            }
            return Json(result);
        }
        public class SelectedItem
        {
            public int Id { get; set; }
            public string ExtraInfo { get; set; }
        }
        public class SelectedItem2
        {
            public int Id { get; set; }
            public string ExtraInfo { get; set; }
            public string selectedOption { get; set; }
        }


        [HttpPost]
        public IActionResult GunBaslaList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value > DateTime.Today).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult GunBitList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value > DateTime.Today).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }

        [HttpPost]
        public IActionResult HaftaBaslaList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value > DateTime.Now.AddDays(-7)).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult HaftaBitList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value > DateTime.Now.AddDays(-7)).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }

        [HttpPost]
        public IActionResult AyBaslaList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value.Month == DateTime.Now.Month).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult AyBitList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }

        [HttpPost]
        public IActionResult YilBaslaList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value.Year == DateTime.Now.Year).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult YilBitList()
        {
            var depois = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).ToList();
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
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenKullanici > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenKullanici).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                    if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
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
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
    }
}
