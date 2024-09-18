using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.RegularExpressions;
using VNNB2B.Models;
using VNNB2B.Models.Hata;

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
                list.Urun = x.UrunID.ToString();
                list.Birim = birim.BirimAdi.ToString();
                list.Tarih = Convert.ToDateTime(x.Tarih).ToString("dd/MM/yyyy");
                list.Miktar = Convert.ToDecimal(x.Miktar).ToString("N2");
                list.TalepEden = x.TalepEden.ToString();
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                var endusuktl = c.SatinAlmaIcerikFiyat.Where(v => v.SatinAlmaTalepID == x.ID && v.Durum == true && v.ParaBirimiID == 1).OrderBy(V => V.BirimFiyat).FirstOrDefault();
                var endusukusd = c.SatinAlmaIcerikFiyat.Where(v => v.SatinAlmaTalepID == x.ID && v.Durum == true && v.ParaBirimiID == 2).OrderBy(V => V.BirimFiyat).FirstOrDefault();
                var endusukeuro = c.SatinAlmaIcerikFiyat.Where(v => v.SatinAlmaTalepID == x.ID && v.Durum == true && v.ParaBirimiID == 3).OrderBy(V => V.BirimFiyat).FirstOrDefault();
                if (endusuktl == null && endusukeuro == null && endusukusd == null) list.EnDusukTeklif = "Teklif Henüz Yok!";
                if (endusuktl != null) list.EnDusukTeklif = "En Düşük TL : " + Convert.ToDecimal(endusuktl.BirimFiyat).ToString() + "₺";
                if (endusukusd != null) list.EnDusukTeklif += " En Düşük USD : " + Convert.ToDecimal(endusukusd.BirimFiyat).ToString() + "$";
                if (endusukeuro != null) list.EnDusukTeklif += " En Düşük EURO : " + Convert.ToDecimal(endusukeuro.BirimFiyat).ToString() + "€";
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
                list.Tarih = Convert.ToDateTime(x.Tarih).ToString("dd/MM/yyyy");
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
        [HttpPost]
        public IActionResult TalepFiyatlari(int id)
        {
            var veri = c.SatinAlmaIcerikFiyat.Where(v => v.Durum == true && v.SatinAlmaTalepID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlmaIcerikFiyat> ham = new List<DtoSatinAlmaIcerikFiyat>();
            foreach (var x in veri)
            {
                var parabirimi = c.ParaBirimleris.FirstOrDefault(v => v.ID == x.ParaBirimiID);
                var tedarikci = c.Tedarikcilers.FirstOrDefault(v => v.ID == x.TedarikciID);
                string ted = "";
                string kisa = "";
                if (tedarikci != null)
                {
                    if (tedarikci.TedarikciKodu != null) ted = tedarikci.TedarikciKodu.ToString();
                    if (tedarikci.Unvan != null) ted += " - " + tedarikci.Unvan.ToString();
                    if (tedarikci.FirmaAdi != null) ted += " - " + tedarikci.FirmaAdi.ToString();
                }
                DtoSatinAlmaIcerikFiyat list = new DtoSatinAlmaIcerikFiyat();
                list.ID = Convert.ToInt32(x.ID);
                if (x.ParaBirimiID != null) { list.ParaBirimiID = parabirimi.ParaBirimAdi.ToString(); if (parabirimi.ParaBirimAdi == "TL") kisa = "₺"; else if (parabirimi.ParaBirimAdi == "USD") kisa = "$"; else if (parabirimi.ParaBirimAdi == "EURO") kisa = "€"; } else list.ParaBirimiID = "";
                if (ted.Length > 0) list.TedarikciID = ted; else list.TedarikciID = "Tanımlanmamış Yanlış Kayıt...";
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                if (x.BirimFiyat != null) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2") + kisa; else list.BirimFiyat = "0,00" + kisa;
                if (x.kdvoran != null) list.KDV = Convert.ToInt32(x.kdvoran).ToString("N2"); else list.KDV = "0";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult FiyatEkle(SatinAlmaIcerikFiyat d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.TedarikciID != null && d.BirimFiyat != null && d.ParaBirimiID != null && d.SatinAlmaTalepID != null && d.kdvoran != null)
                {
                    var talep = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == d.SatinAlmaTalepID);
                    SatinAlmaIcerikFiyat de = new SatinAlmaIcerikFiyat();
                    de.UrunID = talep.UrunID;
                    de.ParaBirimiID = d.ParaBirimiID;
                    de.TedarikciID = d.TedarikciID;
                    de.BirimFiyat = d.BirimFiyat;
                    de.SatinAlmaTalepID = d.SatinAlmaTalepID;
                    de.Aciklama = d.Aciklama;
                    de.Durum = true;
                    if (d.kdvoran > 0)
                    {
                        de.kdvoran = d.kdvoran;
                        de.BirimFiyat = Convert.ToDecimal((d.BirimFiyat * d.kdvoran) / 100) + de.BirimFiyat;
                    }
                    c.SatinAlmaIcerikFiyat.Add(de);
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
        public IActionResult FiyatSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var de = c.SatinAlmaIcerikFiyat.FirstOrDefault(v => v.ID == id);
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
        public IActionResult SatinAlmaOlustur([FromBody] List<int> selectedIds)
        {
            decimal kur = 0;
            decimal kureuro = 0;
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                try
                {
                    DolarKurFormul formuller = new DolarKurFormul();
                    kur = formuller.GetDolarKuru(1);
                    kureuro = formuller.GetEuroKuru(1);

                    List<SatinAlmaTalepler> secilenler = new List<SatinAlmaTalepler>();
                    foreach (var x in selectedIds)
                    {
                        var s = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x);
                        secilenler.Add(s);
                    }

                    List<SatinAlmaIcerikFiyat> endusukfiyatlar = new List<SatinAlmaIcerikFiyat>();
                    foreach (var x in secilenler)
                    {
                        var fiyatlar = c.SatinAlmaIcerikFiyat.Where(v => v.SatinAlmaTalepID == x.ID && v.Durum == true && v.UrunID == x.UrunID).ToList();
                        if (fiyatlar.Count > 0)
                        {
                            var talepbelge = c.SatinAlmaTalepleri.FirstOrDefault(v => v.ID == x.ID);
                            List<SatinAlmaIcerikFiyat> ftl = new List<SatinAlmaIcerikFiyat>();
                            foreach (var v in fiyatlar)
                            {
                                SatinAlmaIcerikFiyat f = new SatinAlmaIcerikFiyat();
                                decimal tlkarcilik = 0;
                                if (v.ParaBirimiID == 2)
                                {
                                    tlkarcilik = Convert.ToDecimal(kur * v.BirimFiyat);
                                }
                                else if (v.ParaBirimiID == 3)
                                {
                                    tlkarcilik = Convert.ToDecimal(kureuro * v.BirimFiyat);
                                }
                                else
                                {
                                    tlkarcilik = Convert.ToDecimal(v.BirimFiyat);
                                }
                                f.ID = v.ID;
                                f.BirimFiyat = tlkarcilik;
                                f.TedarikciID = v.TedarikciID;
                                f.ParaBirimiID = v.ParaBirimiID;
                                f.SatinAlmaTalepID = v.SatinAlmaTalepID;
                                f.UrunID = v.UrunID;
                                f.kdvoran = v.kdvoran;
                                f.Durum = v.Durum;
                                f.Aciklama = v.Aciklama;
                                ftl.Add(f);
                            }

                            var endusuk = ftl.OrderBy(v => v.BirimFiyat).FirstOrDefault();
                            var endusukasil = c.SatinAlmaIcerikFiyat.FirstOrDefault(v => v.ID == endusuk.ID);
                            var urunbirim = c.Urunlers.Where(v => v.ID == endusuk.UrunID).Select(v => v.BirimID).FirstOrDefault();
                            int satinalmano = 0;
                            var acikvarmi = c.SatinAlmas.Where(v => v.TedarikciID == endusuk.TedarikciID && v.Aktif == true && v.Durum == true).FirstOrDefault();
                            int id = 0;
                            if (acikvarmi != null)
                            {
                                satinalmano = acikvarmi.ID;
                            }
                            else
                            {
                                //Eğer Bu Tedarikçiye Ait Satın Alma Belgesi Yoksa Oluşturuyor
                                SatinAlma sa = new SatinAlma();
                                sa.KullaniciID = kulid;
                                sa.TedarikciID = endusuk.TedarikciID;
                                sa.SatinAlmaNo = c.Tedarikcilers.Where(v => v.ID == endusuk.TedarikciID).Select(v => v.TedarikciKodu).FirstOrDefault().ToString() + (c.SatinAlmas.Count() + 1).ToString();
                                sa.Aktif = true;
                                sa.Durum = true;
                                sa.Tarih = DateTime.Now;
                                sa.ToplamMiktar = 0;
                                sa.ToplamTeslimEdilen = 0;
                                sa.ToplamTutar = 0;
                                sa.KdvTutari = 0;
                                sa.TeslimEden = "";
                                sa.TeslimAlan = "";
                                sa.AracPlaka = "";
                                sa.FaturaDurum = false;
                                sa.FaturaNo = "";
                                sa.FaturaAciklama = "";
                                sa.OnayDurum = false;
                                sa.OnayAciklama = "";
                                c.SatinAlmas.Add(sa);
                                c.SaveChanges();

                                satinalmano = c.SatinAlmas.OrderByDescending(v => v.ID).FirstOrDefault().ID;
                            }

                            SatinAlmaIcerik si = new SatinAlmaIcerik();
                            si.Miktar = talepbelge.Miktar;
                            si.KullaniciID = kulid;
                            si.SatinAlmaID = satinalmano;
                            si.UrunID = talepbelge.UrunID;
                            si.Miktar = talepbelge.Miktar;
                            si.KDV = endusuk.kdvoran;
                            si.ParaBirimiID = endusuk.ParaBirimiID;
                            si.BirimFiyat = endusukasil.BirimFiyat;
                            si.BirimID = urunbirim;
                            si.SatirToplam = si.BirimFiyat * si.Miktar;
                            si.Durum = true;
                            c.SatinAlmaIceriks.Add(si);
                            c.SaveChanges();

                            talepbelge.Durum = false;
                            c.SaveChanges();
                            Formuller form = new Formuller(c);
                            form.SatinAlmaHesapla(satinalmano);
                            result = new { status = "success", message = "Seçilen Ürünler İçin Satın Alma Belgeleri Oluşturuldu. Daha Önce Oluşturulmuş Aktif Kayıtlar İçin İse Güncellemeler Yapıldı..." };
                        }
                        else
                        {
                            result = new { status = "error", message = "Seçilen Ürünler Arasında Fiyat Teklifi Girilmemiş Ürünler Vardı. Seçilen Ürünlerin Tamamı Belgelenmedi!!!" };
                        }
                    }
                }
                catch (Exception e)
                {
                    result = new { status = "error", message = e.Message };
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult SatinAlmaBekleyenList()
        {
            var veri = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum != true).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlma> ham = new List<DtoSatinAlma>();
            foreach (var x in veri)
            {
                var ted = c.Tedarikcilers.FirstOrDefault(v => v.ID == x.TedarikciID);
                var personel = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID);
                DtoSatinAlma list = new DtoSatinAlma();
                list.ID = Convert.ToInt32(x.ID);
                list.KullaniciID = personel.AdSoyad.ToString();
                list.SatinAlmaNo = x.SatinAlmaNo.ToString();
                list.Tarih = Convert.ToDateTime(x.Tarih).ToString("dd/MM/yyyy");
                if (ted.TedarikciKodu != null) list.TedarikciID = ted.TedarikciKodu.ToString();
                if (ted.Unvan != null) list.TedarikciID += " - " + ted.Unvan.ToString();
                if (ted.FirmaAdi != null) list.TedarikciID += " - " + ted.FirmaAdi.ToString();
                list.ToplamMiktar = Convert.ToDecimal(x.ToplamMiktar).ToString("N2");
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + "₺";
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult SatinAlmaIcerikList(int id)
        {
            var sip = c.SatinAlmas.FirstOrDefault(v => v.ID == id);
            var veri = c.SatinAlmaIceriks.Where(v => v.Durum == true && v.SatinAlmaID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlmaIcerik> ham = new List<DtoSatinAlmaIcerik>();
            foreach (var x in veri)
            {
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == x.UrunID);
                var grup = c.UrunKategoris.FirstOrDefault(v => v.ID == urun.UrunKategoriID);
                var birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID);
                var para = c.ParaBirimleris.FirstOrDefault(v => v.ID == x.ParaBirimiID);
                string parakisa = "";
                if (para != null)
                {
                    if (para.ParaBirimAdi == "TL") parakisa = "₺";
                    if (para.ParaBirimAdi == "USD") parakisa = "$";
                    if (para.ParaBirimAdi == "EURO") parakisa = "€";
                }
                DtoSatinAlmaIcerik list = new DtoSatinAlmaIcerik();
                list.ID = Convert.ToInt32(x.ID);
                list.UrunID = grup.Adi + " - " + urun.UrunKodu.ToString() + " - " + urun.UrunAdi.ToString();
                list.Urun = x.UrunID.ToString();
                if (x.Renk != null) list.Renk = x.Renk.ToString(); else list.Renk = "";
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "";
                if (x.BirimFiyat != null) list.BirimFiyat = Convert.ToDecimal(x.BirimFiyat).ToString("N2");
                if (x.SatirToplam != null) list.SatirToplam = Convert.ToDecimal(x.SatirToplam).ToString("N2") + parakisa;
                list.KabulMiktar = x.KabulMiktar.ToString();
                if (sip.OnayDurum == true)
                {
                    list.Miktar = x.Miktar.ToString() + " / " + birim.BirimAdi.ToString();
                    list.Durum = "Onaylandı";
                }
                else
                {
                    list.Miktar = x.Miktar.ToString();
                    list.Durum = "Onay Bekliyor";
                }
                ham.Add(list);
            }
            return Json(ham);
        }
        [HttpPost]
        public IActionResult SatinAlmaDevamList()
        {
            var veri = c.SatinAlmas.Where(v => v.Durum == true && v.OnayDurum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoSatinAlma> ham = new List<DtoSatinAlma>();
            foreach (var x in veri)
            {
                var ted = c.Tedarikcilers.FirstOrDefault(v => v.ID == x.TedarikciID);
                var personel = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID);
                var onaylayan = c.Kullanicis.FirstOrDefault(v => v.ID == x.OnaylayanID);
                DtoSatinAlma list = new DtoSatinAlma();
                list.ID = Convert.ToInt32(x.ID);
                list.KullaniciID = personel.AdSoyad.ToString();
                list.OnaylayanID = onaylayan.AdSoyad.ToString();
                list.SatinAlmaNo = x.SatinAlmaNo.ToString();
                list.Tarih = Convert.ToDateTime(x.Tarih).ToString("dd/MM/yyyy");
                list.OnayTarihi = Convert.ToDateTime(x.OnayTarihi).ToString("dd/MM/yyyy");
                if (ted.TedarikciKodu != null) list.TedarikciID = ted.TedarikciKodu.ToString();
                if (ted.Unvan != null) list.TedarikciID += " - " + ted.Unvan.ToString();
                if (ted.FirmaAdi != null) list.TedarikciID += " - " + ted.FirmaAdi.ToString();
                list.ToplamMiktar = Convert.ToDecimal(x.ToplamMiktar).ToString("N2");
                list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + "₺";
                list.ToplamTeslimEdilen = c.SatinAlmaIceriks.Where(v => v.SatinAlmaID == x.ID && v.Durum == true).Sum(v => v.KabulMiktar).ToString();
                ham.Add(list);
            }
            return Json(ham);
        }
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
        [HttpPost]
        public IActionResult SatinAlmaSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var de = c.SatinAlmas.FirstOrDefault(v => v.ID == id);
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
        [HttpPost]
        public IActionResult IcerikDuzenle(SatinAlmaIcerik i)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var de = c.SatinAlmaIceriks.FirstOrDefault(v => v.ID == i.ID);
                de.Renk = i.Renk;
                de.Aciklama = i.Aciklama;
                de.Miktar = i.Miktar;
                de.BirimFiyat = i.BirimFiyat;
                c.SaveChanges();
                Formuller f = new Formuller(c);
                f.SatinAlmaHesapla(Convert.ToInt32(de.SatinAlmaID));
                result = new { status = "success", message = "Kayıt Güncellendi..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult IcerikSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var de = c.SatinAlmaIceriks.FirstOrDefault(v => v.ID == id);
                de.Durum = false;
                c.SaveChanges();
                Formuller f = new Formuller(c);
                f.SatinAlmaHesapla(Convert.ToInt32(de.SatinAlmaID));
                result = new { status = "success", message = "Kayıt Silindi..." };
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult DetayBilgi(int id)
        {
            var x = c.SatinAlmas.Where(v => v.ID == id).FirstOrDefault();
            DtoSatinAlma list = new DtoSatinAlma();
            var ted = c.Tedarikcilers.FirstOrDefault(v => v.ID == x.TedarikciID);
            var personel = c.Kullanicis.FirstOrDefault(v => v.ID == x.KullaniciID);
            list.ID = Convert.ToInt32(x.ID);
            list.KullaniciID = personel.AdSoyad.ToString();
            list.SatinAlmaNo = x.SatinAlmaNo.ToString();
            list.Tarih = Convert.ToDateTime(x.Tarih).ToString("dd/MM/yyyy");
            if (ted.TedarikciKodu != null) list.TedarikciID = ted.TedarikciKodu.ToString();
            if (ted.Unvan != null) list.TedarikciID += " - " + ted.Unvan.ToString();
            if (ted.FirmaAdi != null) list.TedarikciID += " - " + ted.FirmaAdi.ToString();
            list.ToplamMiktar = Convert.ToDecimal(x.ToplamMiktar).ToString("N2");
            list.ToplamTutar = Convert.ToDecimal(x.ToplamTutar).ToString("N2") + "₺";
            list.KdvTutari = Convert.ToDecimal(x.KdvTutari).ToString("N2") + "₺";
            list.AraToplam = Convert.ToDecimal(x.ToplamTutar - x.KdvTutari).ToString("N2") + "₺";
            if (x.OnayDurum == true) list.OnayDurum = "Onaylandı"; else list.OnayDurum = "Onay Bekliyor";
            if (x.DosyaYolu != null) list.DosyaYolu = x.DosyaYolu; else list.DosyaYolu = "";
            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> SatinAlmaOnayla(int id, string OnayAciklama, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var sip = c.SatinAlmas.FirstOrDefault(v => v.ID == id);
            if (imagee != null)
            {
                var dosyaAdi = Path.GetFileName(imagee.FileName);

                var dosyaYolu = Path.Combine("wwwroot/Evraklar/SatinAlmaOnayEvraklar", dosyaAdi);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await imagee.CopyToAsync(stream);
                }

                sip.DosyaYolu = dosyaYolu.Substring(7);

                sip.OnayDurum = true;
                sip.OnaylayanID = kulid;
                sip.OnayTarihi = DateTime.Now;
                sip.OnayAciklama = OnayAciklama;
                sip.Aktif = false;

                c.SaveChanges();
                SatinAlmaHata.Icerik = "Satın Alma Belgesi Onaylandı...";
            }
            else
            {
                SatinAlmaHata.Icerik = "Onay Evrağını Eklemeden Siparişi Onaylayamazsınız...";
            }
            return RedirectToAction("Index", "Bekleyen");
        }
        [HttpPost]
        public IActionResult KabulOlustur(SatinAlmaIcerik i)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var de = c.SatinAlmaIceriks.FirstOrDefault(v => v.ID == i.ID);

                var malkab = c.SatinAlmaTeslimats.FirstOrDefault(v => v.SatinAlmaID == de.SatinAlmaID && v.Durum == true);
                if (malkab != null)
                {
                    var varmi = c.SatinAlmaTeslimatIceriks.FirstOrDefault(v => v.TeslimatID == malkab.ID);
                    if (varmi != null)
                    {
                        if (varmi.Miktar + i.KabulMiktar! > de.Miktar)
                        {
                            result = new { status = "error", message = "Sipariş Edilen Miktardan Fazla Kabul Yapılamaz..." };
                            return Json(result);
                        }
                        else
                        {
                            varmi.Miktar += i.KabulMiktar;
                        }
                    }
                    else
                    {
                        SatinAlmaTeslimatIcerik t = new SatinAlmaTeslimatIcerik();
                        t.TeslimatID = malkab.ID;
                        t.SatinAlmaIcerik = de.ID;
                        t.UrunID = de.UrunID;
                        t.BirimID = de.BirimID;
                        t.Miktar = i.KabulMiktar;
                        t.Durum = true;
                        c.SatinAlmaTeslimatIceriks.Add(t);
                        c.SaveChanges();

                    }
                    result = new { status = "success", message = "Kabul Kaydı Oluşturuldu..." };
                }
                else
                {
                    SatinAlmaTeslimat te = new SatinAlmaTeslimat();
                    te.SatinAlmaID = de.SatinAlmaID;
                    te.KullaniciID = kulid;
                    te.KayitTarihi = DateTime.Now;
                    te.Aciklama = "";
                    te.TeslimatNo = "TE" + (c.SatinAlmaTeslimats.Count() + 1).ToString();
                    te.TeslimEden = "";
                    te.AracPlaka = "";
                    te.Durum = true;
                    c.SatinAlmaTeslimats.Add(te);
                    c.SaveChanges();

                    var sontes = c.SatinAlmaTeslimats.OrderByDescending(v => v.ID).FirstOrDefault();

                    SatinAlmaTeslimatIcerik t = new SatinAlmaTeslimatIcerik();
                    t.TeslimatID = sontes.ID;
                    t.SatinAlmaIcerik = de.ID;
                    t.UrunID = de.UrunID;
                    t.BirimID = de.BirimID;
                    t.Miktar = i.Miktar;
                    t.Durum = true;
                    c.SatinAlmaTeslimatIceriks.Add(t);
                    c.SaveChanges();
                    result = new { status = "success", message = "Kabul Kaydı Oluşturuldu..." };
                }

                de.KabulMiktar += i.KabulMiktar;
                c.SaveChanges();
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
    }
}
