using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class BilgiApiController : Controller
    {
        private readonly Context c;
        public BilgiApiController(Context context)
        {
            c = context;
        }
        //Ana Sayfa Bilgileri

        [HttpPost]
        public IActionResult Bilgiler(int id)
        {
            DtoBilgiler list = new DtoBilgiler();
            //Sipariş
            list.OnayBekleyenSiparis = c.Siparis.Where(v => v.Durum == true && v.OnayDurum != true && v.BayiOnay == true).Count();
            list.DevamEdenSiparis = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true).Count();
            list.KismiTeslimSiparis = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true && v.ToplamTeslimEdilen > 0).Count();
            list.GecmisSiparis = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum == true).Count();

            list.GunlukSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today).Count();
            list.HaftalikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today).Count();
            list.AylikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value.Month == DateTime.Now.Month).Count();
            list.YillikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value.Year == DateTime.Now.Year).Count();


            //Satın Alma
            list.OnayBekleyenSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum != true).Count();
            list.DevamEdenSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true).Count();
            list.KismiKabulSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true && v.ToplamTeslimEdilen > 0).Count();
            list.GecmisSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true).Count();

            list.GunlukSatinAlmas = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today).Count();
            list.HaftalikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today).Count();
            list.AylikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value.Month == DateTime.Now.Month).Count();
            list.YillikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value.Year == DateTime.Now.Year).Count();
            return Json(list);
        }
    }
}
