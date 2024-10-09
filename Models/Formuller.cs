using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace VNNB2B.Models
{
    public class Formuller
    {
        private readonly Context c;
        public Formuller(Context context)
        {
            c = context;
        }
        public static bool durum = false;
        public static string mesaj = "";
        public static int stokid = 0;

        public void SiparisSonMu(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();
            if (icerik.Where(v => v.Miktar == v.TeslimAdet).Count() == icerik.Count())
            {
                sip.TeslimDurum = true;
                sip.SiparisDurum = "Tamamı Teslim Edildi...";
                c.SaveChanges();
            }
            else
            {
                sip.SiparisDurum = "Kısmi Teslim Edildi...";
                c.SaveChanges();
            }
        }
        public void stokozellikleri(List<UrunOzellikleri> o, int id)
        {
            foreach (var x in o)
            {
                UrunOzellikleri of = new UrunOzellikleri();
                of.UrunAltOzellikleriID = x.UrunAltOzellikleriID;
                of.UrunStokID = id;
                of.Durum = true;
                c.UrunOzellikleris.Add(of);
                //c.SaveChanges();
            }

        }

        public void SatinAlmaHesapla(int id)
        {
            DolarKurFormul formuller = new DolarKurFormul();
            decimal kur = formuller.GetDolarKuru(1);
            decimal kureuro = formuller.GetEuroKuru(1);

            var satinalma = c.SatinAlmas.FirstOrDefault(v => v.ID == id);

            satinalma.ToplamMiktar = Convert.ToDecimal(c.SatinAlmaIceriks.Where(v => v.SatinAlmaID == id && v.Durum == true).Sum(v => v.Miktar));
            satinalma.ToplamTutar = 0;

            var icerik = c.SatinAlmaIceriks.Where(v => v.SatinAlmaID == id && v.Durum == true).ToList();
            foreach (var x in icerik)
            {
                x.SatirToplam = 0;
                if (x.ParaBirimiID == 1)
                {
                    satinalma.ToplamTutar += x.BirimFiyat * x.Miktar;
                    x.SatirToplam += x.BirimFiyat * x.Miktar;
                }
                else if (x.ParaBirimiID == 2)
                {
                    satinalma.ToplamTutar += (x.BirimFiyat * kur) * x.Miktar;
                    x.SatirToplam += (x.BirimFiyat * kur) * x.Miktar;
                }
                else if (x.ParaBirimiID == 3)
                {
                    satinalma.ToplamTutar += (x.BirimFiyat * kureuro) * x.Miktar;
                    x.SatirToplam += (x.BirimFiyat * kureuro) * x.Miktar;
                }
                satinalma.KdvTutari += x.BirimFiyat / (1 + (x.KDV / 100));
            }
            c.SaveChanges();
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
                decimal kdv = ((toplamtutar - iskontotoplam) * 10) / 100;
                sip.KDVToplam = kdv;
                sip.ToplamTutar = sip.ToplamTutar + kdv;
            }
            else
            {
                sip.KDVToplam = 0;
            }
            c.SaveChanges();
        }

        public void SiparisOnay(int id, int idd)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id && v.Durum == true).ToList();

            foreach (var x in icerik)
            {
                if (c.DepoIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == x.ID && v.Durum == true) == null)
                {
                    DepoIsEmirleri i = new DepoIsEmirleri();
                    i.SiparisID = x.SiparisID;
                    i.KullaniciID = null;
                    i.SiparisIcerikID = x.ID;
                    i.UrunID = x.UrunID;
                    i.BaslangicTarihi = null;
                    i.BitisTarihi = null;
                    i.BaslamaDurum = false;
                    i.BitirmeDurum = false;
                    i.SiparisAdet = Convert.ToInt32(x.Miktar);
                    i.GelenAdet = Convert.ToInt32(x.Miktar);
                    i.KalanAdet = Convert.ToInt32(x.Miktar);
                    i.GelenTarih = DateTime.Now;
                    i.IslemdekiAdet = 0;
                    i.GidenAdet = 0;
                    i.GorenKullanici = null;
                    i.GorulduMu = false;
                    i.Durum = true;
                    c.DepoIsEmirleris.Add(i);
                    c.SaveChanges();
                }
            }
            mesaj = "Tüm Ürünler Depoya Sevk Edildi.";
        }
        public void BoyaSevk(int id, int kulid, int miktar)
        {
            var depois = c.DepoIsEmirleris.FirstOrDefault(v => v.ID == id);
            var varmi = c.BoyaIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);
            if (varmi == null)
            {
                BoyaIsEmirleri i = new BoyaIsEmirleri();
                i.SiparisID = depois.SiparisID;
                i.KullaniciID = null;
                i.SiparisIcerikID = depois.SiparisIcerikID;
                i.UrunID = depois.UrunID;
                i.BaslangicTarihi = null;
                i.BitisTarihi = null;
                i.BaslamaDurum = false;
                i.BitirmeDurum = false;
                i.SiparisAdet = depois.SiparisAdet;
                i.GelenAdet = miktar;
                i.KalanAdet = miktar;
                i.IslemdekiAdet = 0;
                i.GelenTarih = DateTime.Now;
                i.GidenAdet = 0;
                i.GorenKullanici = null;
                i.GorulduMu = false;
                i.Durum = true;
                c.BoyaIsEmirleris.Add(i);
                c.SaveChanges();
            }
            else
            {
                varmi.KalanAdet += miktar;
                varmi.GelenAdet += miktar;
                varmi.BitirmeDurum = false;
                varmi.BaslamaDurum = false;
                varmi.GorulduMu = false;
                c.SaveChanges();
            }
            mesaj = "Tüm Ürünler Depoya Sevk Edildi.";
        }
        public void DosemeSevk(int id, int miktar)
        {
            var depois = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == id);
            var varmi = c.DosemeIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);
            if (varmi == null)
            {
                DosemeIsEmirleri i = new DosemeIsEmirleri();
                i.SiparisID = depois.SiparisID;
                i.KullaniciID = null;
                i.SiparisIcerikID = depois.SiparisIcerikID;
                i.UrunID = depois.UrunID;
                i.BaslangicTarihi = null;
                i.BitisTarihi = null;
                i.BaslamaDurum = false;
                i.BitirmeDurum = false;
                i.SiparisAdet = depois.SiparisAdet;
                i.GelenAdet = miktar;
                i.KalanAdet = miktar;
                i.IslemdekiAdet = 0;
                i.GelenTarih = DateTime.Now;
                i.GidenAdet = 0;
                i.GorenPersonel = null;
                i.GorulduMu = false;
                i.Durum = true;
                c.DosemeIsEmirleris.Add(i);
                c.SaveChanges();
            }
            else
            {
                varmi.KalanAdet += miktar;
                varmi.GelenAdet += miktar;
                varmi.BitirmeDurum = false;
                varmi.BaslamaDurum = false;
                varmi.GorulduMu = false;
                c.SaveChanges();
            }
            mesaj = "Tüm Ürünler Döşemeye Sevk Edildi.";
        }
        public void DosemeSevkGeri(int id, int miktar)
        {
            var depois = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == id);
            var varmi = c.DosemeIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);

            varmi.KalanAdet -= miktar;
            varmi.GelenAdet -= miktar;
            varmi.BitirmeDurum = false;

            depois.KalanAdet += miktar;
            depois.GidenAdet -= miktar;
            depois.BitirmeDurum = false;

            if (varmi.GelenAdet == 0)
            {
                c.DosemeIsEmirleris.Remove(varmi);
            }
            c.SaveChanges();

            mesaj = "Tüm Ürünler Döşemeye Sevk Edildi.";
        }
        public void AmbalajSevk(int id, int kulid, string tur, int miktar)
        {
            if (tur == "Boya")
            {
                var depois = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == id);
                var varmi = c.AmbalajIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);
                if (varmi == null)
                {
                    AmbalajIsEmirleri i = new AmbalajIsEmirleri();
                    i.SiparisID = depois.SiparisID;
                    i.KullaniciID = null;
                    i.SiparisIcerikID = depois.SiparisIcerikID;
                    i.UrunID = depois.UrunID;
                    i.BaslangicTarihi = null;
                    i.BitisTarihi = null;
                    i.BaslamaDurum = false;
                    i.BitirmeDurum = false;
                    i.SiparisAdet = depois.SiparisAdet;
                    i.GelenAdet = miktar;
                    i.KalanAdet = miktar;
                    i.GelenTarih = DateTime.Now;
                    i.IslemdekiAdet = 0;
                    i.GidenAdet = 0;
                    i.GorenPersonel = null;
                    i.GorulduMu = false;
                    i.Durum = true;
                    c.AmbalajIsEmirleris.Add(i);
                    c.SaveChanges();
                }
                else
                {
                    varmi.KalanAdet += miktar;
                    varmi.GelenAdet += miktar;
                    varmi.BitirmeDurum = false;
                    varmi.BaslamaDurum = false;
                    varmi.GorulduMu = false;
                    c.SaveChanges();
                }
            }
            if (tur == "Döşeme")
            {
                var depois = c.DosemeIsEmirleris.FirstOrDefault(v => v.ID == id);
                var varmi = c.AmbalajIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);
                if (varmi == null)
                {
                    AmbalajIsEmirleri i = new AmbalajIsEmirleri();
                    i.SiparisID = depois.SiparisID;
                    i.KullaniciID = null;
                    i.SiparisIcerikID = depois.SiparisIcerikID;
                    i.UrunID = depois.UrunID;
                    i.BaslangicTarihi = null;
                    i.BitisTarihi = null;
                    i.BaslamaDurum = false;
                    i.BitirmeDurum = false;
                    i.GelenAdet = miktar;
                    i.KalanAdet = miktar;
                    i.GelenTarih = DateTime.Now;
                    i.IslemdekiAdet = 0;
                    i.GidenAdet = 0;
                    i.GorenPersonel = null;
                    i.GorulduMu = false;
                    i.Durum = true;
                    c.AmbalajIsEmirleris.Add(i);
                    c.SaveChanges();
                }
                else
                {
                    varmi.KalanAdet += miktar;
                    varmi.GelenAdet += miktar;
                    varmi.BitirmeDurum = false;
                    varmi.BaslamaDurum = false;
                    varmi.GorulduMu = false;
                    c.SaveChanges();
                }
            }
            mesaj = "Tüm Ürünler Ambalaja Sevk Edildi.";
        }
        public void AmbalajSevkGeri(int id, int miktar)
        {
            var depois = c.BoyaIsEmirleris.FirstOrDefault(v => v.ID == id);
            var varmi = c.AmbalajIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);

            varmi.KalanAdet -= miktar;
            varmi.GelenAdet -= miktar;
            varmi.BitirmeDurum = false;

            depois.KalanAdet += miktar;
            depois.GidenAdet -= miktar;
            depois.BitirmeDurum = false;

            if (varmi.GelenAdet == 0)
            {
                c.AmbalajIsEmirleris.Remove(varmi);
            }
            c.SaveChanges();
            mesaj = "Tüm Ürünler Ambalaja Sevk Edildi.";
        }
        public void SevkiyatSevk(int id, int kulid, int miktar)
        {
            var depois = c.AmbalajIsEmirleris.FirstOrDefault(v => v.ID == id);
            var varmi = c.SevkiyatIsEmirleris.FirstOrDefault(v => v.SiparisIcerikID == depois.SiparisIcerikID && v.Durum == true);
            if (varmi == null)
            {
                SevkiyatIsEmirleri i = new SevkiyatIsEmirleri();
                i.SiparisID = depois.SiparisID;
                i.KullaniciID = null;
                i.SiparisIcerikID = depois.SiparisIcerikID;
                i.UrunID = depois.UrunID;
                i.BaslangicTarihi = null;
                i.BitisTarihi = null;
                i.BaslamaDurum = false;
                i.BitirmeDurum = false;
                i.SiparisAdet = depois.SiparisAdet;
                i.GelenAdet = miktar;
                i.KalanAdet = miktar;
                i.GelenTarih = DateTime.Now;
                i.IslemdekiAdet = 0;
                i.GidenAdet = 0;
                i.GorenPersonel = null;
                i.GorulduMu = false;
                i.Durum = true;
                c.SevkiyatIsEmirleris.Add(i);
                c.SaveChanges();
            }
            else
            {
                varmi.KalanAdet += miktar;
                varmi.GelenAdet += miktar;
                varmi.BitirmeDurum = false;
                varmi.BaslamaDurum = false;
                varmi.GorulduMu = false;
                c.SaveChanges();
            }
            mesaj = "Tüm Ürünler Sevkiyata Sevk Edildi.";
        }

        public void SipSil()
        {
            var sip = c.Siparis.Where(v => v.Durum == true && v.ToplamAdet <= 0).ToList();
            foreach (var x in sip)
            {
                x.Durum = false;
                c.SaveChanges();
            }
        }
        public void SiparisSil(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id).ToList();
            //İş Emirleri Temizle
            foreach (var x in icerik)
            {
                //Depo Temizle
                var depo = c.DepoIsEmirleris.Where(v => v.SiparisIcerikID == x.ID).ToList();
                foreach (var v in depo)
                {
                    v.Durum = false;
                    c.SaveChanges();
                }
                //Boya Temizle
                var boya = c.BoyaIsEmirleris.Where(v => v.SiparisIcerikID == x.ID).ToList();
                foreach (var v in boya)
                {
                    v.Durum = false;
                    c.SaveChanges();
                }
                //Döşeme Temizle
                var doseme = c.DosemeIsEmirleris.Where(v => v.SiparisIcerikID == x.ID).ToList();
                foreach (var v in doseme)
                {
                    v.Durum = false;
                    c.SaveChanges();
                }
                //Ambalaj Temizle
                var ambalaj = c.AmbalajIsEmirleris.Where(v => v.SiparisIcerikID == x.ID).ToList();
                foreach (var v in ambalaj)
                {
                    v.Durum = false;
                    c.SaveChanges();
                }
                //Sevkiyat Temizle
                var sevkiyat = c.SevkiyatIsEmirleris.Where(v => v.SiparisIcerikID == x.ID).ToList();
                foreach (var v in sevkiyat)
                {
                    v.Durum = false;
                    c.SaveChanges();
                }
                x.Durum = false;
                c.SaveChanges();
            }
            //Teslimat Temizle
            var teslimatlar = c.Teslimats.Where(v => v.SiparisID == id).ToList();
            foreach (var x in teslimatlar)
            {
                var tesicerik = c.TeslimatIceriks.Where(v => v.TeslimatID == x.ID).ToList();
                foreach (var v in tesicerik)
                {
                    v.Durum = false;
                    c.SaveChanges();
                }
                x.Durum = false;
                c.SaveChanges();
            }
            sip.Durum = false;
            sip.SiparisDurum = "Sipariş İptal Edildi...";
            c.SaveChanges();
        }
    }
}