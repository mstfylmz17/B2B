using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace VNNB2B.Controllers.Api
{
    public class SiparisApiController : Controller
    {
        private readonly Context c;
        public SiparisApiController(Context context)
        {
            c = context;
        }

        //Bayi Panel İşlemleri
        [HttpPost]
        public IActionResult SepetList(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.Durum == true && v.BayiID == id && v.BayiOnay == false);
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
                    DtoSiparisIcerik list = new DtoSiparisIcerik();
                    list.ID = Convert.ToInt32(x.ID);
                    if (urun.UrunKodu != null) list.UrunKodu = urun.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                    if (urun.UrunAdi != null) list.UrunAciklama = urun.UrunAdi.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                    if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = x.Aciklama.ToString();
                    if (x.Miktar != null) list.Miktar = Convert.ToInt32(x.Miktar).ToString(); else list.Miktar = "1";
                    if (x.SatirToplam != null && x.SatirToplam > 0) list.SatirToplam = Convert.ToDecimal(x.SatirToplam).ToString("N2") + para; else list.SatirToplam = "0,00" + para;

                    ham.Add(list);
                }
                return Json(ham);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult SepetSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == id);
            de.Durum = false;
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SepetGuncelle(SiparisIcerik s)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            SiparisIcerik de = c.SiparisIceriks.FirstOrDefault(v => v.ID == s.ID);
            if (s.Miktar != null)
            {
                de.Miktar = s.Miktar;
                de.SatirToplam = de.BirimFiyat * s.Miktar;
                c.SaveChanges();
                siphesapla(Convert.ToInt32(de.SiparisID));
                result = new { status = "success", message = "Miktar Güncellendi..." };
            }
            else
                result = new { status = "errror", message = "Miktar Boş Geçilemez..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SiparisOnay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.Siparis.FirstOrDefault(v => v.BayiID == id && v.BayiOnay == false);
            sip.BayiOnay = true;
            sip.SiparisDurum = "Onay Bekliyor...";
            c.SaveChanges();
            result = new { status = "success", message = "Sepet Onaylandı... Sipariş Durumunu Siparişlerim Sekmesinden Kontrol Edebilirsiniz..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult SiparisList(int id)
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.BayiID == id && v.BayiOnay == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamAdet = Convert.ToInt32(c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult SepetIcerik(int id)
        {
            var x = c.Siparis.FirstOrDefault(v => v.Durum == true && v.BayiID == id && v.BayiOnay == false);
            if (x != null)
            {
                string para = "";
                if (x.ParaBirimiID == 2) para = " $"; else para = " ₺";
                DtoSiparis list = new DtoSiparis();
                list.ID = Convert.ToInt32(x.ID);
                if (x.SiparisTarihi != null) list.SiparisTarihi = Convert.ToDateTime(x.SiparisTarihi).ToString("d");
                if (x.SiparisBayiAciklama != null) list.SiparisBayiAciklama = x.SiparisBayiAciklama; else list.SiparisBayiAciklama = "";
                var kismivarmi = c.Teslimats.FirstOrDefault(v => v.SiparisID == x.ID && v.Durum == true);
                if (x.TeslimTarihi != null && kismivarmi != null) list.TeslimTarihi = Convert.ToDateTime(x.TeslimTarihi).ToString("d"); else if (x.TeslimTarihi == null && kismivarmi == null) list.TeslimTarihi = "Teslim Edilmedi..."; else list.TeslimTarihi = "Kısmi Teslimatlar Var...";
                list.ToplamTeslimEdilen = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true).Sum(v => v.Miktar)).ToString();
                list.SiparisNo = x.SiparisNo.ToString();
                if (x.SiparisDurum != null) list.SiparisDurum = x.SiparisDurum.ToString(); else list.SiparisDurum = "";
                list.ToplamAdet = Convert.ToInt32(x.ToplamAdet).ToString();
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + para;
                list.AraToplam = Convert.ToDecimal(x.AraToplam).ToString("N2") + para;
                list.IstoktoToplam = Convert.ToDecimal(x.IstoktoToplam).ToString("N2") + para;
                list.KDVToplam = Convert.ToDecimal(x.KDVToplam).ToString("N2") + para;
                return Json(list);
            }
            else
            {
                return Json(2);
            }
        }
        [HttpPost]
        public IActionResult SepetBilgi(int id)
        {
            var sepet = c.Siparis.FirstOrDefault(v => v.BayiID == id && v.BayiOnay == false && v.Durum == true);
            int x = c.SiparisIceriks.Where(v => v.SiparisID == sepet.ID && v.Durum == true).Count();
            DtoSiparis list = new DtoSiparis();
            list.SepetAdet = x.ToString();
            return Json(list);
        }
        [HttpPost]
        public IActionResult SiparisSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            Siparis de = c.Siparis.FirstOrDefault(v => v.ID == id);
            de.Durum = false;
            c.SaveChanges();
            result = new { status = "success", message = "Kayıt Silindi..." };
            return Json(result);
        }

        public void siphesapla(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
            decimal? bayioran = sip.IskontoOran;
            decimal toplamtutar = 0;
            decimal toplamadet = 0;
            decimal iskontotoplam = 0;

            toplamtutar = Convert.ToDecimal(c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).Sum(v => v.SatirToplam));
            toplamadet = Convert.ToDecimal(c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).Sum(v => v.Miktar));
            iskontotoplam = Convert.ToDecimal((toplamtutar * bayioran) / 100);

            sip.ToplamAdet = toplamadet;
            sip.IstoktoToplam = iskontotoplam;
            sip.ToplamTutar = toplamtutar - iskontotoplam;
            sip.AraToplam = toplamtutar;
            if (bayi.KDVDurum == true)
            {
                decimal kdv = (toplamtutar * 10) / 100;
                sip.KDVToplam = kdv;
                sip.ToplamTutar = sip.ToplamTutar + kdv;
            }
            else
            {
                sip.KDVToplam = 0;
            }
            c.SaveChanges();
        }

        //Admin Panel İşlemleri
        [HttpPost]
        public IActionResult OnayBekleyenList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum != true && v.BayiOnay == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
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
            return Json(ham);
        }
        [HttpPost]
        public IActionResult SiparisOnayla(int id, string OnayAciklama)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            if (sip.BayiOnay != true)
            {
                result = new { status = "error", message = "Bayi Henüz Siparişi Onaylamamış..." };
                return Json(result);
            }
            sip.OnayDurum = true;
            sip.OnaylayanID = kulid;
            sip.OnayTarihi = DateTime.Now;
            sip.OnayAciklama = OnayAciklama;

            sip.SiparisDurum = "Sipariş Onaylandı...";
            c.SaveChanges();
            result = new { status = "success", message = "Sipariş Onaylandı..." };
            return Json(result);
        }
        [HttpPost]
        public IActionResult DevamList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
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
            return Json(ham);
        }
        [HttpPost]
        public IActionResult KTList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true && v.ToplamTeslimEdilen > 0).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
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
            return Json(ham);
        }
        [HttpPost]
        public IActionResult YHList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true && v.SiparisDurum == "Hazır...").OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
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
            return Json(ham);
        }
        [HttpPost]
        public IActionResult GecmisList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            foreach (var x in veri)
            {
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
            return Json(ham);
        }
        [HttpPost]
        public IActionResult TeslimEdilebilirList()
        {
            var veri = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum == false && v.SiparisDurum == "Sipariş İşelmde...").OrderByDescending(v => v.ID).ToList();
            List<DtoSiparis> ham = new List<DtoSiparis>();
            List<DtoSiparisIcerik> icerik = new List<DtoSiparisIcerik>();
            foreach (var x in veri)
            {
                var i = c.SiparisIceriks.Where(v => v.SiparisID == x.ID && v.Durum == true);
                foreach (var v in i)
                {
                    if (v.Miktar - v.TeslimAdet > 0)
                    {

                    }
                }
            }
            return Json(ham);
        }
    }
}