using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using VNNB2B.Models;

namespace VNNB2B.Controllers.Api
{
    public class UretimApiController : Controller
    {
        private readonly Context c;
        public UretimApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult IsEmirleri()
        {
            var veri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.SiparisTarihi = Convert.ToDateTime(sipic.Tarih).ToString("dd/MM/yyyy");
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (sipic.Aciklama != null) list.Aciklama = sipic.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult Okundu([FromBody] List<int> selectedIds)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            foreach (var id in selectedIds)
            {
                var isemri = c.UretimIsEmirleris.FirstOrDefault(v => v.ID == id);
                isemri.OkunmaTarih = DateTime.Now;
                isemri.GorenPersonel = kulid;
                isemri.GorulduMu = true;
                isemri.BaslamaDurum = true;
                isemri.BaslangicTarihi = DateTime.Now;
                c.SaveChanges();
            }
            result = new { status = "success", message = "İş Emirleri Okundu Olarak İşaretlendi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && (v.KalanAdet > 0 ||v.IslemdekiAdet>0)&& (v.BitirmeDurum != true || (v.BitirmeDurum == true && v.GelenAdet != v.SiparisAdet))).ToList();
            List<DtoDepoIsEmirleri> i = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.SiparisTarihi = Convert.ToDateTime(sipic.Tarih).ToString("dd/MM/yyyy");
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                i.Add(list);
            }
            return Json(i.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult AlList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true && v.KalanAdet > 0).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.SiparisTarihi = Convert.ToDateTime(sipic.Tarih).ToString("dd/MM/yyyy");
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult CikarList()
        {
            var veri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.BaslamaDurum == true && v.IslemdekiAdet > 0).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                ham.Add(list);
            }
            return Json(ham.OrderByDescending(v => v.SiparisNo));
        }
        [HttpPost]
        public IActionResult KismiList()
        {
            var veri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0)).OrderByDescending(v => v.ID).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in veri)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.SiparisTarihi = Convert.ToDateTime(sipic.Tarih).ToString("dd/MM/yyyy");
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BaslamaDurum == true && (v.GelenAdet == c.SatinAlmaTalepleri.FirstOrDefault(a => a.ID == v.SatinAlmaIcerikID).Miktar) && v.GelenAdet == v.GidenAdet).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                if (sipic.Miktar == x.GelenAdet && x.KalanAdet == 0)
                {
                    DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                    list.ID = Convert.ToInt32(x.ID);
                    list.SiparisTarihi = Convert.ToDateTime(sipic.Tarih).ToString("dd/MM/yyyy");
                    list.UrunID = x.UrunID.ToString();
                    list.UrunKodu = urun.UrunKodu.ToString();
                    var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                    if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                    if (urun.Boyut != null)
                        list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                    list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                    list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                    list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                    list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                    if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                    list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                    list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                    list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                    ham.Add(list);
                }
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult UretimeAl([FromBody] List<SelectedItem> selectedItems)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (selectedItems != null)
            {
                foreach (var x in selectedItems)
                {
                    int miktar = Convert.ToInt32(x.ExtraInfo);
                    var isemri = c.UretimIsEmirleris.FirstOrDefault(v => v.ID == x.Id);
                    if (isemri.GelenAdet >= miktar && (isemri.KalanAdet >= miktar))
                    {
                        var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == isemri.SatinAlmaIcerikID);
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
                            result = new { status = "success", message = "Üretime Alma Başarılı..." };
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
        public IActionResult UretimdenCikar([FromBody] List<SelectedItem> selectedItems)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            if (selectedItems != null)
            {
                foreach (var x in selectedItems)
                {
                    int miktar = Convert.ToInt32(x.ExtraInfo);
                    var isemri = c.UretimIsEmirleris.FirstOrDefault(v => v.ID == x.Id);
                    if (isemri.GelenAdet >= miktar && ((isemri.IslemdekiAdet) >= miktar))
                    {
                        var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == isemri.SatinAlmaIcerikID);
                        isemri.IslemdekiAdet -= miktar;
                        isemri.GidenAdet += miktar;
                        if (isemri.GelenAdet == isemri.GidenAdet)
                        {
                            isemri.BitirmeDurum = true;
                            isemri.BitisTarihi = DateTime.Now;
                            isemri.KullaniciID = kulid;
                        }
                        c.SaveChanges();
                        var urun = c.Urunlers.FirstOrDefault(v => v.ID == sipic.UrunID);
                        urun.StokMiktari += miktar;
                        c.SaveChanges();
                        result = new { status = "success", message = "Üretimden Çıkarma Başarılı... Ürünler Stoklara Aktarıldı..." };
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
        public class SelectedItem
        {
            public int Id { get; set; }
            public string ExtraInfo { get; set; }
        }

        [HttpPost]
        public IActionResult GunBaslaList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value > DateTime.Today).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult GunBitList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value > DateTime.Today).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }

        [HttpPost]
        public IActionResult HaftaBaslaList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value > DateTime.Now.AddDays(-7)).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult HaftaBitList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value > DateTime.Now.AddDays(-7)).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }

        [HttpPost]
        public IActionResult AyBaslaList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value.Month == DateTime.Now.Month).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult AyBitList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.GelenTarih.Value.Month == DateTime.Now.Month).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }

        [HttpPost]
        public IActionResult YilBaslaList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenAdet > 0 && v.GelenTarih.Value.Year == DateTime.Now.Year).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");

                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult YilBitList()
        {
            var depois = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GidenAdet > 0 && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).ToList();
            List<DtoDepoIsEmirleri> ham = new List<DtoDepoIsEmirleri>();
            foreach (var x in depois)
            {
                var sipic = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.SatinAlmaIcerikID);
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                DtoDepoIsEmirleri list = new DtoDepoIsEmirleri();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = x.UrunID.ToString();
                list.UrunKodu = urun.UrunKodu.ToString();
                var kat = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                if (urun.UrunAdi != null) list.UrunAdi = kat.Adi + " / " + urun.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (urun.Boyut != null)
                    list.UrunAdi += " <br/> " + urun.Boyut.ToString();
                list.SiparisAdet = Convert.ToInt32(sipic.Miktar).ToString();
                list.GelenAdet = Convert.ToInt32(x.GelenAdet).ToString();
                list.GidenAdet = Convert.ToInt32(x.GidenAdet).ToString();
                list.KalanAdet = Convert.ToInt32(x.SiparisAdet - (x.GidenAdet)).ToString();
                if (x.GorenPersonel != null && x.GorenPersonel > 0) list.GorenKullanici = c.Kullanicis.FirstOrDefault(v => v.ID == x.GorenPersonel).KullaniciAdi.ToString(); else list.GorenKullanici = "";
                list.OkunmaTarih = Convert.ToDateTime(x.OkunmaTarih).ToString("dd/MM/yyyy");
                list.BaslangicTarihi = Convert.ToDateTime(x.BaslangicTarihi).ToString("dd/MM/yyyy");
                list.BitisTarihi = Convert.ToDateTime(x.BitisTarihi).ToString("dd/MM/yyyy");
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
    }
}
