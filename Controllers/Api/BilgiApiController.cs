using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            list.DevamEdenSiparis = Convert.ToInt32(c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true).Count());
            list.KismiTeslimSiparis = Convert.ToInt32(c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.ToplamTeslimEdilen > 0 && v.ToplamTeslimEdilen != v.ToplamAdet).Count());
            list.GecmisSiparis = c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum == true).Count();
            list.YuklemeyeHazirSip = c.SiparisIceriks.Where(v => v.Durum == true && v.YuklemeyeHazir == true && v.Miktar != v.TeslimAdet && v.TeslimDurum != true).Sum(v => v.HazirAdet);

            list.GunlukSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value >= DateTime.Today && v.OnayDurum == true).Count();
            list.GunlukSiparisTop = Convert.ToDecimal(c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value >= DateTime.Today && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.HaftalikSiparis = c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today.AddDays(-7) && v.OnayDurum == true).Count();
            list.HaftalikSiparisTop = Convert.ToDecimal(c.Siparis.Where(v => v.Durum == true && v.SiparisTarihi.Value > DateTime.Today.AddDays(-7) && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
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

            list.GunlukSatinAlmas = c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value > DateTime.Today && v.OnayDurum == true).Count();
            list.GunlukSatinAlmasTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value > DateTime.Today).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.HaftalikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value > DateTime.Today.AddDays(-7) && v.OnayDurum == true).Count();
            list.HaftalikSatinAlmaTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value > DateTime.Today.AddDays(-7) && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.AylikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value.Month == DateTime.Now.Month && v.OnayDurum == true).Count();
            list.AylikSatinAlmaTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value.Month == DateTime.Now.Month && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";
            list.YillikSatinAlma = c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value.Year == DateTime.Now.Year && v.OnayDurum == true).Count();
            list.YillikSatinAlmaTop = Convert.ToDecimal(c.SatinAlmas.Where(v => v.Durum == true && v.OnayTarihi.Value.Year == DateTime.Now.Year && v.OnayDurum == true).Sum(v => v.ToplamTutar)).ToString("N2") + " ₺";


            //Depo
            list.YeniDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Sum(v => v.GelenAdet);
            list.DevamDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == false && v.GorulduMu == true).Sum(v => v.GelenAdet);
            list.KismiDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && v.GidenAdet > 0).Sum(v => v.GelenAdet - v.GidenAdet);
            list.GecmisDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BaslamaDurum == true && (v.GelenAdet == c.SiparisIceriks.FirstOrDefault(a => a.ID == v.SiparisIcerikID).Miktar) && v.GelenAdet == v.GidenAdet).Sum(v => v.GelenAdet);

            list.GunlukDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today).Sum(v => v.GelenAdet);
            list.HaftalikDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GelenAdet);
            list.AylikDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Month == DateTime.Now.Month).Sum(v => v.GelenAdet);
            list.YillikDepoIsEmri = c.DepoIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Year == DateTime.Now.Year).Sum(v => v.GelenAdet);

            list.GunlukDepoIsEmriCik = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet);
            list.HaftalikDepoIsEmriCik = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet);
            list.AylikDepoIsEmriCik = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet);
            list.YillikDepoIsEmriCik = c.DepoIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet);

            list.GunlukDepoIsEmriGelen = Convert.ToInt32(c.UrunCikislaris.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today).Sum(v => v.Miktar));
            list.HaftalikDepoIsEmriGelen = Convert.ToInt32(c.UrunCikislaris.Where(v => v.Durum == true && v.Tarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.Miktar));
            list.AylikDepoIsEmriGelen = Convert.ToInt32(c.UrunCikislaris.Where(v => v.Durum == true && v.Tarih.Value.Month == DateTime.Now.Month).Sum(v => v.Miktar));
            list.YillikDepoIsEmriGelen = Convert.ToInt32(c.UrunCikislaris.Where(v => v.Durum == true && v.Tarih.Value.Year == DateTime.Now.Year).Sum(v => v.Miktar));


            //Boya
            list.YeniBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Sum(v => v.GelenAdet - v.GidenAdet);
            list.DevamBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && (v.BitirmeDurum != true || (v.BitirmeDurum == true && v.GelenAdet != v.SiparisAdet))).Sum(v => v.GelenAdet);
            list.KismiBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && v.GidenAdet > 0).Sum(v => v.GelenAdet - v.GidenAdet);
            list.GecmisBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true && (v.GelenAdet == c.SiparisIceriks.FirstOrDefault(a => a.ID == v.SiparisIcerikID).Miktar)).Sum(v => v.GelenAdet);

            list.GunlukBoyaIsEmriGelen = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today).Sum(v => v.GelenAdet);
            list.HaftalikBoyaIsEmriGelen = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GelenAdet);
            list.AylikBoyaIsEmriGelen = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Month == DateTime.Now.Month).Sum(v => v.GelenAdet);
            list.YillikBoyaIsEmriGelen = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Year == DateTime.Now.Year).Sum(v => v.GelenAdet);

            list.GunlukBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.HaftalikBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.AylikBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.YillikBoyaIsEmri = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet + v.IslemdekiAdet);

            list.GunlukBoyaIsEmriCik = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet);
            list.HaftalikBoyaIsEmriCik = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet);
            list.AylikBoyaIsEmriCik = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet);
            list.YillikBoyaIsEmriCik = c.BoyaIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet);


            //Üretim
            list.YeniUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Sum(v => v.GelenAdet - v.GidenAdet);
            list.DevamUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && (v.BitirmeDurum != true || (v.BitirmeDurum == true && v.GelenAdet != v.SiparisAdet))).Sum(v => v.GelenAdet);
            list.KismiUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0)).Sum(v => v.GelenAdet - v.GidenAdet);
            list.GecmisUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true && (v.GelenAdet == c.SatinAlmaTalepleri.FirstOrDefault(a => a.ID == v.SatinAlmaIcerikID).Miktar)).Sum(v => v.GelenAdet);

            list.GunlukUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.HaftalikUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.AylikUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.YillikUretimIsEmri = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet + v.IslemdekiAdet);

            list.GunlukUretimIsEmriCik = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet);
            list.HaftalikUretimIsEmriCik = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet);
            list.AylikUretimIsEmriCik = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet);
            list.YillikUretimIsEmriCik = c.UretimIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet);

            list.GunlukUretimIsEmriGelen = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today).Sum(v => v.GelenAdet);
            list.HaftalikUretimIsEmriGelen = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GelenAdet);
            list.AylikUretimIsEmriGelen = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Month == DateTime.Now.Month).Sum(v => v.GelenAdet);
            list.YillikUretimIsEmriGelen = c.UretimIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Year == DateTime.Now.Year).Sum(v => v.GelenAdet);


            //Döşeme
            list.YeniDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Sum(v => v.GelenAdet - v.GidenAdet);
            list.DevamDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && (v.BitirmeDurum != true || (v.BitirmeDurum == true && v.GelenAdet != v.SiparisAdet))).Sum(v => v.GelenAdet);
            list.KismiDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && v.GidenAdet > 0).Sum(v => v.GelenAdet - v.GidenAdet);
            list.GecmisDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true && (v.GelenAdet == c.SiparisIceriks.FirstOrDefault(a => a.ID == v.SiparisIcerikID).Miktar)).Sum(v => v.GelenAdet);

            list.GunlukDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.HaftalikDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.AylikDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.YillikDosemeIsEmri = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet + v.IslemdekiAdet);

            list.GunlukDosemeIsEmriCik = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet);
            list.HaftalikDosemeIsEmriCik = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet);
            list.AylikDosemeIsEmriCik = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet);
            list.YillikDosemeIsEmriCik = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet);

            list.GunlukDosemeIsEmriGelen = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today).Sum(v => v.GelenAdet);
            list.HaftalikDosemeIsEmriGelen = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GelenAdet);
            list.AylikDosemeIsEmriGelen = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Month == DateTime.Now.Month).Sum(v => v.GelenAdet);
            list.YillikDosemeIsEmriGelen = c.DosemeIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Year == DateTime.Now.Year).Sum(v => v.GelenAdet);


            //Ambalaj
            list.YeniAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Sum(v => v.GelenAdet - v.GidenAdet);
            list.DevamAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && (v.BitirmeDurum != true || (v.BitirmeDurum == true && v.GelenAdet != v.SiparisAdet))).Sum(v => v.GelenAdet);
            list.KismiAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.BitirmeDurum == false && (v.KalanAdet > 0 || v.IslemdekiAdet > 0) && v.GidenAdet > 0).Sum(v => v.GelenAdet - v.GidenAdet);
            list.GecmisAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true && (v.GelenAdet == c.SiparisIceriks.FirstOrDefault(a => a.ID == v.SiparisIcerikID).Miktar)).Sum(v => v.GelenAdet);

            list.GunlukAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.HaftalikAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.AylikAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet + v.IslemdekiAdet);
            list.YillikAmbalajIsEmri = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BaslangicTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet + v.IslemdekiAdet);

            list.GunlukAmbalajIsEmriCik = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value > DateTime.Today).Sum(v => v.GidenAdet);
            list.HaftalikAmbalajIsEmriCik = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GidenAdet);
            list.AylikAmbalajIsEmriCik = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value.Month == DateTime.Now.Month).Sum(v => v.GidenAdet);
            list.YillikAmbalajIsEmriCik = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.BitisTarihi.Value.Year == DateTime.Now.Year).Sum(v => v.GidenAdet);

            list.GunlukAmbalajIsEmriGelen = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today).Sum(v => v.GelenAdet);
            list.HaftalikAmbalajIsEmriGelen = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GelenAdet);
            list.AylikAmbalajIsEmriGelen = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Month == DateTime.Now.Month).Sum(v => v.GelenAdet);
            list.YillikAmbalajIsEmriGelen = c.AmbalajIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Year == DateTime.Now.Year).Sum(v => v.GelenAdet);


            //Sevkiyat 
            list.YeniSevkiyatIsEmri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GorulduMu != true).Sum(v => v.GelenAdet - v.GidenAdet);
            list.DevamSevkiyatIsEmri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GorulduMu == true && v.KalanAdet > 0).Sum(v => v.GelenAdet);
            list.KismiSevkiyatIsEmri = Convert.ToInt32(c.Siparis.Where(v => v.Durum == true && v.OnayDurum == true && v.TeslimDurum != true && v.ToplamAdet != (c.TeslimatIceriks.Where(a => a.SiparisID == v.ID && a.Durum == true).Sum(a => a.Miktar)) && v.ToplamTeslimEdilen > 0).Count());
            list.GecmisSevkiyatIsEmri = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.BitirmeDurum == true && (v.GelenAdet == c.SiparisIceriks.FirstOrDefault(a => a.ID == v.SiparisIcerikID).Miktar)).Sum(v => v.GelenAdet);

            list.GunlukSevkiyatIsEmriGelen = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today).Sum(v => v.GelenAdet);
            list.HaftalikSevkiyatIsEmriGelen = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value > DateTime.Today.AddDays(-7)).Sum(v => v.GelenAdet);
            list.AylikSevkiyatIsEmriGelen = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Month == DateTime.Now.Month).Sum(v => v.GelenAdet);
            list.YillikSevkiyatIsEmriGelen = c.SevkiyatIsEmirleris.Where(v => v.Durum == true && v.GelenTarih.Value.Year == DateTime.Now.Year).Sum(v => v.GelenAdet);

            list.GunlukSevkiyatIsEmri = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.Durum == true && (c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID).TeslimatTarihi.Value) >= DateTime.Today).Sum(v => v.Miktar));
            list.HaftalikSevkiyatIsEmri = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.Durum == true && (c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID).TeslimatTarihi.Value) >= DateTime.Today.AddDays(-7)).Sum(v => v.Miktar));
            list.AylikSevkiyatIsEmri = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.Durum == true && (c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID).TeslimatTarihi.Value.Month) == DateTime.Now.Month).Sum(v => v.Miktar));
            list.YillikSevkiyatIsEmri = Convert.ToInt32(c.TeslimatIceriks.Where(v => v.Durum == true && (c.Teslimats.FirstOrDefault(a => a.ID == v.TeslimatID).TeslimatTarihi.Value.Year) == DateTime.Now.Year).Sum(v => v.Miktar));


            return Json(list);
        }

        //Yetki İşlemleri
        [HttpPost]
        public IActionResult PanelYetkileri()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var tanimlipaneller = c.PanelTanimlaris.Where(v => v.KullaniciID == kulid && v.Durum == true).ToList();
            List<DtoPanelTanimlari> tanimli = new List<DtoPanelTanimlari>();
            foreach (var x in tanimlipaneller)
            {
                DtoPanelTanimlari p = new DtoPanelTanimlari();
                p.Departman = x.DepartmanID.ToString();
                tanimli.Add(p);
            }
            return Json(tanimli);
        }
    }
}
