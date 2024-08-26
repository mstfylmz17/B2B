using DataAccessLayer.Concrate;
using EntityLayer.Concrate;

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

        public void SiparisOnay(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
            var icerik = c.SiparisIceriks.Where(v => v.SiparisID == id).ToList();

            foreach (var x in icerik)
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
                i.GelenAdet = 0;
                i.GidenAdet = 0;
                i.GorenKullanici = null;
                i.GorulduMu = false;
                i.Durum = true;
                c.DepoIsEmirleris.Add(i);
                c.SaveChanges();
            }
            mesaj = "Tüm Ürünler Depoya Sevk Edildi.";
        }
        public void BoyaSevk(int id, int kulid)
        {
            var depois = c.DepoIsEmirleris.FirstOrDefault(v => v.ID == id);
            var sip = c.Siparis.FirstOrDefault(v => v.ID == depois.SiparisID);
            BoyaIsEmirleri i = new BoyaIsEmirleri();
            i.SiparisID = depois.SiparisID;
            i.KullaniciID = null;
            i.SiparisIcerikID = depois.SiparisIcerikID;
            i.UrunID = depois.UrunID;
            i.BaslangicTarihi = null;
            i.BitisTarihi = null;
            i.BaslamaDurum = false;
            i.BitirmeDurum = false;
            i.GelenAdet = 0;
            i.GidenAdet = 0;
            i.GorenKullanici = null;
            i.GorulduMu = false;
            i.Durum = true;
            c.BoyaIsEmirleris.Add(i);
            depois.BitirmeDurum = true;
            depois.BitisTarihi = DateTime.Now;
            depois.KullaniciID = kulid;
            c.SaveChanges();
            mesaj = "Tüm Ürünler Depoya Sevk Edildi.";
        }
    }
}