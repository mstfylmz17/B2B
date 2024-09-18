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
            list.YuklemeyeHazirSip = c.SiparisIceriks.Where(v => v.Durum == true && v.YuklemeyeHazir == true && v.TeslimDurum != true && v.HazirAdet > 0).Select(v => v.SiparisID).Distinct().Count();

            list.GunlukSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today && v.OnayDurum == true).Count();
            list.GunlukSiparisTop = Convert.ToDecimal(c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.HaftalikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today && v.OnayDurum == true).Count();
            list.HaftalikSiparisTop = Convert.ToDecimal(c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.AylikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value.Month == DateTime.Now.Month && v.OnayDurum == true).Count();
            list.AylikSiparisTop = Convert.ToDecimal(c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value.Month == DateTime.Now.Month && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.YillikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value.Year == DateTime.Now.Year && v.OnayDurum == true).Count();
            list.YillikSiparisTop = Convert.ToDecimal(c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value.Year == DateTime.Now.Year && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";


            //Satın Alma
            list.OnayBekleyenSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum != true).Count();
            list.DevamEdenSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true).Count();
            list.KismiKabulSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true && v.ToplamTeslimEdilen > 0 && (v.ToplamTeslimEdilen != v.ToplamMiktar)).Count();
            list.GecmisSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true && v.ToplamTeslimEdilen > 0 && (v.ToplamTeslimEdilen == v.ToplamMiktar)).Count();
            list.SatinAlmaTalepleri = c.SatinAlmaTalepleri.Where(v => v.Durum == true).Count();

            list.GunlukSatinAlmas = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today && v.OnayDurum == true).Count();
            list.GunlukSatinAlmasTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.HaftalikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today && v.OnayDurum == true).Count();
            list.HaftalikSatinAlmaTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.AylikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value.Month == DateTime.Now.Month && v.OnayDurum == true).Count();
            list.AylikSatinAlmaTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value.Month == DateTime.Now.Month && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.YillikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value.Year == DateTime.Now.Year && v.OnayDurum == true).Count();
            list.YillikSatinAlmaTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.Tarih.Value.Year == DateTime.Now.Year && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";


            //Depo
            list.YeniDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Count();
            list.DevamDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true).Count();
            list.KismiDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0 && v.IslemdekiAdet > 0).Count();
            list.GecmisDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).Count();

            list.GunlukDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.HaftalikDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.AylikDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Month == DateTime.Now.Month).Count();
            list.YillikDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Year == DateTime.Now.Year).Count();


            //Boya
            list.YeniBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Count();
            list.DevamBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true).Count();
            list.KismiBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0 && v.IslemdekiAdet > 0).Count();
            list.GecmisBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).Count();

            list.GunlukBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.HaftalikBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.AylikBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Month == DateTime.Now.Month).Count();
            list.YillikBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Year == DateTime.Now.Year).Count();


            //Döşeme
            list.YeniDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Count();
            list.DevamDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true).Count();
            list.KismiDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0 && v.IslemdekiAdet > 0).Count();
            list.GecmisDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).Count();

            list.GunlukDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.HaftalikDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.AylikDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Month == DateTime.Now.Month).Count();
            list.YillikDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Year == DateTime.Now.Year).Count();


            //Ambalaj
            list.YeniAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Count();
            list.DevamAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true).Count();
            list.KismiAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && v.KalanAdet > 0 && v.IslemdekiAdet > 0).Count();
            list.GecmisAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true).Count();

            list.GunlukAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.HaftalikAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value > DateTime.Today).Count();
            list.AylikAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Month == DateTime.Now.Month).Count();
            list.YillikAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.OkunmaTarih.Value.Year == DateTime.Now.Year).Count();
            return Json(list);
        }
    }
}
