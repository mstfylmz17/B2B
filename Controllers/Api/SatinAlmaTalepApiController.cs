using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class SatinAlmaTalepApiController : Controller
    {
        private readonly Context c;
        public SatinAlmaTalepApiController(Context context)
        {
            c = context;
        }

        //Satın Alma Talep İşlemleri
        [HttpPost]
        public IActionResult TalepList()
        {
            var veri = c.SatinAlmaTalepleri.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlmaTalepler> ham = new List<DtoSatinAlmaTalepler>();
            foreach (var x in veri)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                var grup = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                var birim = c.Birimlers.FirstOrDefault(v => v.ID == x.Birim);
                DtoSatinAlmaTalepler list = new DtoSatinAlmaTalepler();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = grup.Adi + " - " + urun.UrunKodu.ToString() + " - " + urun.UrunAdi.ToString();
                list.Birim = birim.BirimAdi.ToString();
                list.Tarih = Convert.ToDateTime(x.Tarih).ToString("d");
                list.Miktar = Convert.ToDecimal(x.Miktar).ToString("N2");
                list.TalepEden = x.TalepEden.ToString();
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult TalepEskiList()
        {
            var veri = c.SatinAlmaTalepleri.Where(v => v.Durum == false).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlmaTalepler> ham = new List<DtoSatinAlmaTalepler>();
            foreach (var x in veri)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                var birim = c.Birimlers.FirstOrDefault(v => v.ID == x.Birim);
                DtoSatinAlmaTalepler list = new DtoSatinAlmaTalepler();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = urun.UrunKodu.ToString() + " - " + urun.UrunAdi.ToString();
                list.Birim = birim.BirimAdi.ToString();
                list.Tarih = Convert.ToDateTime(x.Tarih).ToString("d");
                list.Miktar = Convert.ToDecimal(x.Miktar).ToString("N2");
                list.TalepEden = x.TalepEden.ToString();
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult TalepEkle(SatinAlmaTalepler d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.UrunID != null && d.TalepEden != null && d.Tarih != null && d.Miktar != null && d.Birim != null)
                {
                    SatinAlmaTalepler de = new SatinAlmaTalepler();
                    de.UrunID = d.UrunID;
                    de.Kaydeden = kulid;
                    de.Tarih = d.Tarih;
                    de.Miktar = d.Miktar;
                    de.Birim = d.Birim;
                    de.TalepEden = d.TalepEden;
                    de.Aciklama = d.Aciklama;
                    de.Durum = true;
                    c.SatinAlmaTalepleri.Add(de);
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
                else
                {
                    result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult TalepSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                SatinAlmaTalepler de = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == id);
                de.Durum = false;
                c.SaveChanges();
                result = new { status = "success", message = "Kayıt Silindi..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }

        //Satın Alma İşlemleri
        [HttpPost]
        public IActionResult SatinAlmaList()
        {
            var veri = c.SatinAlmas.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlma> ham = new List<DtoSatinAlma>();
            foreach (var x in veri)
            {
                var personel = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID);
                DtoSatinAlma list = new DtoSatinAlma();
                list.ID = Convert.ToInt32(x.ID);
                list.KullaniciID = personel.AdSoyad.ToString();
                list.SatinAlmaNo = x.SatinAlmaNo.ToString();

                ham.Add(list);
            }
            return Json(ham);
        }
    }
}
