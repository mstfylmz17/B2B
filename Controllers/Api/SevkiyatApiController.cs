using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models;

namespace VNNB2B.Controllers.Api
{
    public class SevkiyatApiController : Controller
    {
        private readonly Context c;
        public SevkiyatApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult IsEmirleri()
        {
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - x.GidenAdet).ToString();

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
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.KalanAdet > 0).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(x.GelenAdet - sipic.TeslimAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - sipic.TeslimAdet).ToString();

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                string bayiadi = "";
                if (bayi != null)
                {
                    if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString();
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
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                if (sipic.Miktar == x.GelenAdet && x.KalanAdet == 0)
                {
                    list.ID = Convert.ToInt32(x.ID);
                    list.UrunID = x.UrunID.ToString();
                    list.SiparisNo = sip.SiparisNo.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                    list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                    list.GelenAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                    list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                    list.KalanAdet = Convert.ToInt32(x.SiparisAdet - x.GidenAdet).ToString();
                    if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
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
        public IActionResult KismiList()
        {
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == x.SiparisIcerikID);
                if (sipic.TeslimAdet != sipic.Miktar && sipic.TeslimAdet > 0)
                {

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
                    list.GelenAdet = Convert.ToInt32(x.GelenAdet - sipic.TeslimAdet).ToString();
                    list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                    list.KalanAdet = Convert.ToInt32(x.SiparisAdet - sipic.TeslimAdet).ToString();
                    if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
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
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult UygunList(int id)
        {
            var sevkiyat = c.SevkiyatIsEmirleris.FirstOrDefault(v => v.ID == id);
            var veri = c.SiparisIceriks.Where(v => v.Durum == true && v.UrunID == sevkiyat.UrunID && v.HazirAdet == null).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparisIcerik> icerikler = new List<DtoSiparisIcerik>();
            foreach (var x in veri)
            {
                var sip = c.Siparis.FirstOrDefault(v => v.ID == x.SiparisID);
                if (sip.OnayDurum == true)
                {
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                    var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                    list.ID = Convert.ToInt32(x.ID);
                    list.UrunID = x.UrunID.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    if (urun.UrunAdi != null) list.UrunAciklama = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAciklama += " <br/> " + urun.Boyut.ToString();
                    string bayiadi = "";
                    if (bayi != null)
                    {
                        if (bayi.KullaniciAdi != null) bayiadi = bayi.KullaniciAdi.ToString() + "<br />";
                        if (sip.SiparisBayiAciklama != null) bayiadi += " - " + sip.SiparisBayiAciklama.ToString();
                    }
                    list.BayiID = bayiadi;
                    list.SiparisID = sip.SiparisNo.ToString();
                    if (sip.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(sip.SiparisTarihi).ToString("dd/MM/yyyy"); else list.SiparisTarihi = "";
                    list.Miktar = Convert.ToInt32(x.Miktar).ToString();
                    list.HazirAdet = Convert.ToInt32(x.HazirAdet).ToString();
                    list.TeslimAdet = Convert.ToInt32(x.TeslimAdet).ToString();
                    list.KalanAdet = Convert.ToInt32(x.Miktar - x.TeslimAdet).ToString();
                    var ozellik = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == x.ID && v.Durum == true).ToList();
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
                    if (x.Aciklama != null) if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = ""; else list.Aciklama = "";
                    icerikler.Add(list);

                }
            }
            return Json(icerikler);
        }
        [HttpPost]
        public IActionResult Sevkiyat([FromBody] List<SelectedItem> selectedIds)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            foreach (var x in selectedIds)
            {
                var isemri = c.SevkiyatIsEmirleris.FirstOrDefault(v => v.ID == x.Id);
                var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
                isemri.OkunmaTarih = DateTime.Now;
                isemri.GorenPersonel = kulid;
                isemri.GorulduMu = true;
                if (sipic.HazirAdet != null) sipic.HazirAdet += isemri.GelenAdet - isemri.GidenAdet;
                else sipic.HazirAdet = isemri.GelenAdet - isemri.GidenAdet;
                sipic.YuklemeyeHazir = true;
                isemri.GidenAdet += isemri.GelenAdet - isemri.GidenAdet;
                c.SaveChanges();
            }
            result = new { status = "success", message = "İş Emirleri Okundu Olarak İşaretlendi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult Kaydir(int id, int miktar, int degisen)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };

            var isemri = c.SevkiyatIsEmirleris.FirstOrDefault(v => v.ID == degisen);
            var degisecekicerik = c.SiparisIceriks.FirstOrDefault(v => v.ID == isemri.SiparisIcerikID);
            var secilenisemri = c.SiparisIceriks.FirstOrDefault(v => v.ID == id);
            if (degisecekicerik.HazirAdet >= miktar && degisecekicerik.HazirAdet > 0)
            {
                try
                {
                    //Seçilen İş Emri İşlemleri
                    //Depo
                    var depo = c.DepoIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == id);
                    depo.GorulduMu = true;
                    depo.GorenKullanici = kulid;
                    Formuller formuller = new Formuller(c);
                    formuller.BoyaSevk(depo.ID, kulid, miktar);
                    depo.GidenAdet += miktar;
                    depo.KalanAdet -= miktar;
                    depo.IslemdekiAdet = depo.GelenAdet - depo.GidenAdet;
                    if (depo.GelenAdet == depo.GidenAdet)
                    {
                        depo.BitirmeDurum = true;
                        depo.BitisTarihi = DateTime.Now;
                        depo.KullaniciID = kulid;
                    }
                    depo.KullaniciID = kulid;
                    c.SaveChanges();

                    //Boya iş Emirleri
                    var boya = c.BoyaIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == id);
                    boya.GorulduMu = true;
                    boya.GorenKullanici = kulid;
                    c.SaveChanges();

                    boya.IslemdekiAdet += miktar;
                    boya.KalanAdet -= miktar;
                    if (boya.IslemdekiAdet > 0)
                    {
                        boya.BaslamaDurum = true;
                        boya.BaslangicTarihi = DateTime.Now;
                    }
                    boya.KullaniciID = kulid;
                    c.SaveChanges();

                    Formuller f = new Formuller(c);
                    f.AmbalajSevk(boya.ID, kulid, "Boya", miktar);

                    boya.IslemdekiAdet -= miktar;
                    boya.GidenAdet += miktar;
                    if (boya.GelenAdet == boya.GidenAdet)
                    {
                        boya.BitirmeDurum = true;
                        boya.BitisTarihi = DateTime.Now;
                        boya.KullaniciID = kulid;
                    }
                    c.SaveChanges();

                    //Ambalaj işlemleri
                    var ambalaj = c.AmbalajIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == id);
                    ambalaj.IslemdekiAdet += miktar;
                    ambalaj.KalanAdet -= miktar;
                    if (ambalaj.IslemdekiAdet > 0)
                    {
                        ambalaj.BaslamaDurum = true;
                        ambalaj.BaslangicTarihi = DateTime.Now;
                    }
                    ambalaj.KullaniciID = kulid;
                    c.SaveChanges();

                    f.SevkiyatSevk(ambalaj.ID, kulid, miktar);

                    ambalaj.IslemdekiAdet -= miktar;
                    ambalaj.GidenAdet += miktar;
                    if (ambalaj.GelenAdet == ambalaj.GidenAdet)
                    {
                        ambalaj.BitirmeDurum = true;
                        ambalaj.BitisTarihi = DateTime.Now;
                        ambalaj.KullaniciID = kulid;
                    }
                    c.SaveChanges();
                    //Bitti burda



                    //değişecek olanın düzenlemeleri
                    //depo
                    var depois = c.DepoIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == degisecekicerik.ID);
                    depois.GidenAdet = depois.GelenAdet - miktar;
                    depois.KalanAdet = miktar;
                    depois.BitirmeDurum = false;
                    depois.BaslamaDurum = false;
                    depois.GorulduMu = false;
                    depois.GelenTarih = DateTime.Now;
                    c.SaveChanges();
                    //Boya
                    var boyais = c.BoyaIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == degisecekicerik.ID);
                    depois.GidenAdet = depois.GelenAdet - miktar;
                    depois.KalanAdet = miktar;
                    boyais.BitirmeDurum = false;
                    boyais.BaslamaDurum = false;
                    boyais.GorulduMu = false;
                    boyais.GelenTarih = DateTime.Now;
                    c.SaveChanges();
                    //Döşeme
                    var dosemeis = c.DosemeIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == degisecekicerik.ID);
                    if (dosemeis != null)
                    {
                        dosemeis.GidenAdet -= miktar;
                        dosemeis.KalanAdet += miktar;
                        dosemeis.BitirmeDurum = false;
                        dosemeis.BaslamaDurum = false;
                        dosemeis.GorulduMu = false;
                        dosemeis.GelenTarih = DateTime.Now;
                        c.SaveChanges();
                    }
                    //Ambalaj
                    var ambalajis = c.AmbalajIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == degisecekicerik.ID);
                    depois.GidenAdet = depois.GelenAdet - miktar;
                    depois.KalanAdet = miktar;
                    ambalajis.BitirmeDurum = false;
                    ambalajis.BaslamaDurum = false;
                    ambalajis.GorulduMu = false;
                    ambalajis.GelenTarih = DateTime.Now;
                    c.SaveChanges();
                    //sevkiyat
                    var sevkiyatis = c.SevkiyatIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == degisecekicerik.ID);
                    depois.GidenAdet = depois.GelenAdet - miktar;
                    depois.KalanAdet = miktar;
                    sevkiyatis.BitirmeDurum = false;
                    sevkiyatis.BaslamaDurum = false;
                    sevkiyatis.GorulduMu = false;
                    sevkiyatis.GelenTarih = DateTime.Now;
                    c.SaveChanges();

                    degisecekicerik.HazirAdet = 0;
                    degisecekicerik.YuklemeyeHazir = false;
                    c.SaveChanges();

                    result = new { status = "success", message = "Sipariş Kaydırma İşlemi Başarılı... Siparişi Sevkiyat İş Emirleri Kısmından Kabul Edebilirsiniz..." };
                }
                catch (Exception? ex)
                {
                    result = new { status = "error", message = "İşlem Sırasında Problem Oluştu Lütfen Tekrar Deneyiniz..." + ex.ToString() };
                }
            }
            else
            {
                result = new { status = "error", message = "Aktarılmak İstenen Miktar Siparişten Fazla Olamaz!!!" };
            }
            return Json(result);
        }
        public class SelectedItem
        {
            public int Id { get; set; }
        }
        [HttpPost]
        public IActionResult GunList()
        {
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.BaslangicTarihi.Value > DateTime.Today).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - x.GidenAdet).ToString();
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
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult HaftaList()
        {
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.BaslangicTarihi.Value > DateTime.Now.AddDays(-7)).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - x.GidenAdet).ToString();
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
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult AyList()
        {
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - x.GidenAdet).ToString();
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
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult YilList()
        {
            var veri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).OrderByDescending(v => v.ID).ToList();
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
                list.GelenAdet = Convert.ToInt32(x.GelenAdet - x.GidenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - x.GidenAdet).ToString();
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
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham);
        }
    }
}
