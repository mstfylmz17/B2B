using DataAccessLayer.Concrate;
using EntityLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;

namespace VNNB2B.Controllers.Api
{
    public class UrunApiController : Controller
    {
        private readonly Context c;
        public UrunApiController(Context context)
        {
            c = context;
        }
        [HttpPost]
        public IActionResult UrunList()
        {
            var veri = c.Urunlers.Where(v => v.Durum == true).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunler> ham = new List<DtoUrunler>();
            foreach (var x in veri)
            {
                decimal guncel = 0;
                DtoUrunler list = new DtoUrunler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunKodu != null) list.UrunKodu = x.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                if (x.UrunAdi != null) list.UrunAdi = x.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (x.UrunAciklama != null) list.UrunAciklama = x.UrunAciklama.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.Birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.Birim = "Tanımlanmamış...";
                if (x.UrunKategoriID != null) list.UrunKategori = c.UrunKategoris.FirstOrDefault(v => v.ID == x.UrunKategoriID).Adi.ToString(); else list.UrunKategori = "Tanımlanmamış...";
                list.UrunTuru = c.UrunTurlaris.FirstOrDefault(v => v.ID == x.UrunTuruID).UrunGrubuAdi.ToString();
                list.StokMiktari = c.UrunStoklaris.Where(v => v.ID == x.ID && v.Durum == true && v.StokMiktari > 0).Sum(v => v.StokMiktari).ToString();
                if (guncel < x.KritikStokMiktari) list.Durum = "Red";
                if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> UrunEkle(Urunler d, IFormFile imagee)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var kodbosmu = c.Urunlers.FirstOrDefault(v => v.UrunKodu == d.UrunKodu);
                if (kodbosmu != null)
                {
                    result = new { status = "error", message = "Ürün Kodu Daha Önce Farklı Bir Ürüne Atanmış Lütfen Farklı Bir Ürün Kdu Tanımlayınız..." };
                }
                else
                {
                    if (d.UrunKodu != null && d.UrunAdi != null && d.UrunTuruID != null && d.BirimID != null)
                    {
                        Urunler kat = new Urunler();
                        kat.UrunKodu = d.UrunKodu;
                        kat.UrunKategoriID = d.UrunKategoriID;
                        kat.UrunAdi = d.UrunAdi;
                        kat.UrunAciklama = d.UrunAciklama;
                        kat.UrunTuruID = d.UrunTuruID;
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                        kat.BirimKG = d.BirimKG;
                        kat.BirimM3 = d.BirimM3;
                        kat.Boyut = d.Boyut;
                        kat.PaketAdet = d.PaketAdet;
                        kat.BirimID = d.BirimID;
                        if (imagee != null)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                await imagee.CopyToAsync(memoryStream);
                                byte[] bytes = memoryStream.ToArray();

                                kat.Resim = bytes;
                            }
                        }
                        kat.Durum = true;
                        c.Urunlers.Add(kat);
                        c.SaveChanges();
                        result = new { status = "success", message = "Kayıt Başarılı..." };
                    }
                    else
                    {
                        result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
                    }
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult UrunSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                Urunler de = c.Urunlers.FirstOrDefault(v => v.ID == id);
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
        public IActionResult UrunStokList(int id)
        {
            var veri = c.UrunStoklaris.Where(v => v.Durum == true && v.UrunID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunStoklari> ham = new List<DtoUrunStoklari>();
            foreach (var x in veri)
            {
                var sipno = c.SiparisIcerikUrunOzellikleris.FirstOrDefault(v => v.UrunStoklariID == x.ID);
                string ozellik = "";
                var ozellikleri = c.UrunOzellikleris.Where(v => v.UrunStokID == x.ID && v.Durum == true).ToList();
                foreach (var v in ozellikleri)
                {
                    var ozelliktanim = c.UrunAltOzellikleris.FirstOrDefault(a => a.ID == v.UrunAltOzellikleriID);
                    var ozellikturu = c.UrunOzelikTurlaris.FirstOrDefault(a => a.ID == ozelliktanim.UrunOzellikTurlariID);
                    ozellik += ozellikturu.OzellikAdi.ToString() + " - " + ozelliktanim.OzellikAdi.ToString();
                }
                DtoUrunStoklari list = new DtoUrunStoklari();
                var urun = c.Urunlers.FirstOrDefault(v => v.ID == id);
                list.ID = Convert.ToInt32(x.ID);
                list.StokTarihi = Convert.ToDateTime(x.StokTarihi).ToString("g");
                list.StokMiktari = x.StokMiktari.ToString();
                list.UrunOzellikleriID = ozellik.ToString();
                if (sipno != null)
                {
                    var sipic = c.SiparisIceriks.FirstOrDefault(v => v.ID == sipno.SiaprisIcerikID);
                    var sip = c.Siparis.FirstOrDefault(v => v.ID == sipic.SiparisID);
                    list.SiparisNo = sip.SiparisNo.ToString();
                }
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public async Task<IActionResult> UrunDuzenle(Urunler d, IFormFile imagee, string FiyatTL, string FiyatUSD)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var kodbosmu = c.Urunlers.FirstOrDefault(v => v.UrunKodu == d.UrunKodu);
                if (kodbosmu != null)
                {
                    result = new { status = "error", message = "Ürün Kodu Daha Önce Farklı Bir Ürüne Atanmış Lütfen Farklı Bir Ürün Kdu Tanımlayınız..." };
                }
                else
                {
                    Urunler kat = c.Urunlers.FirstOrDefault(v => v.ID == d.ID);
                    if (d.UrunKodu != null)
                        kat.UrunKodu = d.UrunKodu;
                    if (d.UrunAdi != null)
                        kat.UrunAdi = d.UrunAdi;
                    if (d.UrunAciklama != null)
                        kat.UrunAciklama = d.UrunAciklama;
                    if (d.UrunTuruID != null)
                        kat.UrunTuruID = d.UrunTuruID;
                    if (d.KritikStokMiktari != null)
                        kat.KritikStokMiktari = d.KritikStokMiktari;
                    if (d.BirimID != null)
                        kat.BirimID = d.BirimID;
                    if (d.UrunKategoriID != null)
                        kat.UrunKategoriID = d.UrunKategoriID;
                    if (d.PaketAdet != null)
                        kat.PaketAdet = d.PaketAdet;
                    if (d.Boyut != null)
                        kat.Boyut = d.Boyut;
                    if (d.BirimM3 != null)
                        kat.BirimM3 = d.BirimM3;
                    if (d.BirimKG != null)
                        kat.BirimKG = d.BirimKG;
                    if (imagee != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await imagee.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();

                            kat.Resim = bytes;
                        }
                    }
                    try
                    {
                        var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == d.ID && v.Durum == true);
                        decimal? tlfiat = Convert.ToDecimal(FiyatTL);
                        decimal? usdfiyat = Convert.ToDecimal(FiyatUSD);

                        UrunFiyatlari f = new UrunFiyatlari();
                        f.FiyatTL = tlfiat;
                        f.FiyatUSD = usdfiyat;
                        f.UrunID = d.ID;
                        f.FiyatTarihi = DateTime.Now;
                        f.Durum = true;
                        c.UrunFiyatlaris.Add(f);
                        c.SaveChanges();

                        fiyat.Durum = false;
                        c.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
            }
            else
            {
                result = new { status = "error", message = "Yetkiniz Yok Lütfen Yöneticinize Başvurunuz..." };
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult UrunOzellikList(int id)
        {
            var veri = c.UrunOzellikTanimlaris.Where(v => v.Durum == true && v.UrunID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunOzellikTanimlari> ham = new List<DtoUrunOzellikTanimlari>();
            foreach (var x in veri)
            {
                decimal guncel = 0;
                DtoUrunOzellikTanimlari list = new DtoUrunOzellikTanimlari();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunOzellikTurlariID != null) list.UrunOzellikTanimi = c.UrunOzelikTurlaris.FirstOrDefault(v => v.ID == x.UrunOzellikTurlariID).OzellikAdi.ToString(); else list.UrunOzellikTanimi = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult UrunOzellikEkle(UrunOzellikTanimlari d)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                if (d.UrunOzellikTurlariID != null)
                {
                    UrunOzellikTanimlari kat = new UrunOzellikTanimlari();
                    kat.UrunOzellikTurlariID = d.UrunOzellikTurlariID;
                    kat.UrunID = d.UrunID;
                    kat.Durum = true;
                    c.UrunOzellikTanimlaris.Add(kat);
                    c.SaveChanges();
                    result = new { status = "success", message = "Kayıt Başarılı..." };
                }
                else
                {
                    result = new { status = "error", message = "Lütfen Boş Alan Bırakmayınız..." };
                }
            }
            return Json(result);
        }
        [HttpPost]
        public IActionResult OzellikSil(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Kullanicis.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                UrunOzellikTanimlari de = c.UrunOzellikTanimlaris.FirstOrDefault(v => v.ID == id);
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
        public IActionResult UrunDetay(int id)
        {
            var x = c.Urunlers.Where(v => v.ID == id).FirstOrDefault();

            decimal guncel = 0;
            DtoUrunler list = new DtoUrunler();
            list.ID = Convert.ToInt32(x.ID);
            if (x.UrunKodu != null) list.UrunKodu = x.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
            if (x.UrunAdi != null) list.UrunAdi = x.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
            if (x.UrunAciklama != null) list.UrunAciklama = x.UrunAciklama.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
            if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
            if (x.BirimID != null) list.Birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.Birim = "Tanımlanmamış...";
            if (x.UrunKategoriID != null) list.UrunKategori = c.UrunKategoris.FirstOrDefault(v => v.ID == x.UrunKategoriID).Adi.ToString(); else list.UrunKategori = "Tanımlanmamış...";
            list.UrunTuru = c.UrunTurlaris.FirstOrDefault(v => v.ID == x.UrunTuruID).UrunGrubuAdi.ToString();
            list.StokMiktari = c.UrunStoklaris.Where(v => v.ID == x.ID && v.Durum == true && v.StokMiktari > 0).Sum(v => v.StokMiktari).ToString();
            if (guncel < x.KritikStokMiktari) list.Durum = "Red";
            if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
            var veri = c.UrunOzellikTanimlaris.Where(v => v.Durum == true && v.UrunID == id).OrderBy(v => v.ID).ToList();
            List<DtoUrunOzellikTanimlari> ham = new List<DtoUrunOzellikTanimlari>();
            foreach (var a in veri)
            {
                DtoUrunOzellikTanimlari l = new DtoUrunOzellikTanimlari();
                l.ID = Convert.ToInt32(a.UrunOzellikTurlariID);
                if (a.UrunOzellikTurlariID != null) l.UrunOzellikTanimi = c.UrunOzelikTurlaris.FirstOrDefault(v => v.ID == a.UrunOzellikTurlariID).OzellikAdi.ToString(); else l.UrunOzellikTanimi = "";
                ham.Add(l);
            }
            list.UrunOzellikleri = ham;
            return Json(list);
        }
        [HttpPost]
        public IActionResult KatUrunList(int id)
        {
            var veri = c.Urunlers.Where(v => v.Durum == true && v.UrunKategoriID == id && v.UrunTuruID == 3).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunler> ham = new List<DtoUrunler>();
            foreach (var x in veri)
            {
                decimal guncel = 0;
                DtoUrunler list = new DtoUrunler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunKodu != null) list.UrunKodu = x.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                if (x.UrunAdi != null) list.UrunAdi = x.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (x.UrunAciklama != null) list.UrunAciklama = x.UrunAciklama.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.Birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.Birim = "Tanımlanmamış...";
                if (x.UrunKategoriID != null) list.UrunKategori = c.UrunKategoris.FirstOrDefault(v => v.ID == x.UrunKategoriID).Adi.ToString(); else list.UrunKategori = "Tanımlanmamış...";
                list.UrunTuru = c.UrunTurlaris.FirstOrDefault(v => v.ID == x.UrunTuruID).UrunGrubuAdi.ToString();
                list.StokMiktari = c.UrunStoklaris.Where(v => v.ID == x.ID && v.Durum == true && v.StokMiktari > 0).Sum(v => v.StokMiktari).ToString();
                if (guncel < x.KritikStokMiktari) list.Durum = "Red";
                if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult UrunAltOzellikleri(int id)
        {
            var veri = c.UrunAltOzellikleris.Where(v => v.Durum == true && v.UrunOzellikTurlariID == id).OrderByDescending(v => v.ID).ToList();
            List<DtoUrunAltOzellikleri> ham = new List<DtoUrunAltOzellikleri>();
            foreach (var x in veri)
            {
                DtoUrunAltOzellikleri list = new DtoUrunAltOzellikleri();
                list.ID = Convert.ToInt32(x.ID);
                if (x.OzellikAdi != null) list.OzellikAdi = x.OzellikAdi.ToString(); else list.OzellikAdi = "";
                ham.Add(list);
            }
            return Json(ham.OrderBy(v => v.ID));
        }
        [HttpPost]
        public IActionResult SepetEkle([FromBody] SepetEkleModel model)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNBayiCerez", out var Cerez);
            int kulid = Convert.ToInt32(Cerez);
            var result = new { status = "error", message = "İşlem Başarısız..." };
            var kul = c.Bayilers.FirstOrDefault(v => v.ID == kulid);
            if (kul != null)
            {
                var sip = c.Siparis.FirstOrDefault(v => v.BayiID == kulid && v.Durum == true && v.BayiOnay == false);
                int sipno = 0;
                if (sip == null)
                {
                    Siparis s = new Siparis();
                    s.BayiID = kulid;
                    s.SiparisBayiAciklama = "";
                    s.SiparisTarihi = DateTime.Now;
                    s.ToplamAdet = 0;
                    s.ToplamTeslimEdilen = 0;
                    s.ToplamTutar = 0;
                    s.IskontoOran = 0;
                    s.IstoktoToplam = 0;
                    s.AraToplam = 0;
                    s.OnayDurum = false;
                    s.OnayAciklama = "";
                    s.Durum = true;
                    s.SiparisDurum = "Sipariş Bayi Onay Bekliyor...";
                    s.SiparisNo = kul.BayiKodu + " " + c.Siparis.Count().ToString() + 1;
                    s.BayiOnay = false;
                    c.Siparis.Add(s);
                    c.SaveChanges();
                    var sonsip = c.Siparis.OrderByDescending(v => v.ID).FirstOrDefault(v => v.Durum == true && v.BayiID == kulid);
                    sipno = sonsip.ID;
                }
                else
                {
                    sipno = sip.ID;
                }
                var fiyat = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == model.ID && v.Durum == true);
                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == sip.BayiID);
                decimal? birimfiyat = 0;
                if (fiyat != null) if (bayi.ParaBirimi == 1) { birimfiyat = fiyat.FiyatTL; sip.ParaBirimiID = 1; } else { birimfiyat = fiyat.FiyatUSD; sip.ParaBirimiID = 2; }
                else { birimfiyat = 0; sip.ParaBirimiID = 1; }
                var varmi = c.SiparisIceriks.FirstOrDefault(v => v.UrunID == model.ID && v.SiparisID == sipno && v.Durum == true);
                if (varmi == null)
                {
                    SiparisIcerik i = new SiparisIcerik();
                    i.SiparisID = sipno;
                    i.UrunID = model.ID;
                    i.Miktar = model.Miktar;
                    i.Aciklama = model.Aciklama;
                    i.BirimFiyat = birimfiyat;
                    i.SatirToplam = birimfiyat * model.Miktar;
                    i.Durum = true;
                    c.SiparisIceriks.Add(i);
                    c.SaveChanges();
                    var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                    bool stokdurum = false;
                    int? stokid = 0;
                    foreach (var v in model.Ozellikler)
                    {
                        if (v.OzellikAdi != null)
                        {
                            int ozellikid = Convert.ToInt32(v.OzellikAdi);
                            var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                            var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                            foreach (var x in stoktavarmi)
                            {
                                var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                foreach (var b in ozellikler)
                                {
                                    if (b.UrunAltOzellikleriID == ozellikid) { stokdurum = true; stokid = b.UrunStokID; } else { stokdurum = false; break; }
                                }
                            }

                        }
                    }

                    foreach (var x in model.Ozellikler)
                    {
                        int ozellikid = Convert.ToInt32(x.OzellikAdi);
                        SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                        so.SiaprisIcerikID = sipicid.ID;
                        so.UrunID = model.ID;
                        if (stokdurum == true)
                        {
                            so.UrunStoklariID = stokid;
                        }
                        so.UrunAltOzellikID = ozellikid;
                        so.Durum = true;
                        c.SiparisIcerikUrunOzellikleris.Add(so);
                        c.SaveChanges();
                    }
                }
                else
                {
                    bool aynisi = false;
                    var sipoz = c.SiparisIcerikUrunOzellikleris.Where(v => v.SiaprisIcerikID == varmi.ID && v.Durum == true).ToList();
                    foreach (var x in sipoz)
                    {
                        foreach (var v in model.Ozellikler)
                        {
                            int ozid = Convert.ToInt32(v.OzellikAdi);
                            if (ozid == x.UrunAltOzellikID)
                            {
                                aynisi = true;
                            }
                            else
                            {
                                aynisi = false;
                                break;
                            }
                        }
                    }
                    if (aynisi == true)
                    {
                        varmi.Miktar += model.Miktar;
                        varmi.BirimFiyat = birimfiyat;
                        varmi.SatirToplam = birimfiyat * varmi.Miktar;
                    }
                    else
                    {
                        SiparisIcerik i = new SiparisIcerik();
                        i.SiparisID = sipno;
                        i.UrunID = model.ID;
                        i.Miktar = model.Miktar;
                        i.Aciklama = model.Aciklama;
                        i.BirimFiyat = birimfiyat;
                        i.SatirToplam = birimfiyat * model.Miktar;
                        i.Durum = true;
                        c.SiparisIceriks.Add(i);
                        c.SaveChanges();
                        var sipicid = c.SiparisIceriks.OrderByDescending(v => v.ID).FirstOrDefault(v => v.SiparisID == sipno && v.Durum == true);

                        bool stokdurum = false;
                        int? stokid = 0;
                        foreach (var v in model.Ozellikler)
                        {
                            if (v.OzellikAdi != null)
                            {
                                int ozellikid = Convert.ToInt32(v.OzellikAdi);
                                var ozellik = c.UrunAltOzellikleris.FirstOrDefault(x => x.ID == ozellikid);
                                var stoktavarmi = c.UrunStoklaris.Where(x => x.Durum == true && x.UrunID == model.ID).ToList();
                                foreach (var x in stoktavarmi)
                                {
                                    var ozellikler = c.UrunOzellikleris.Where(b => b.UrunStokID == x.ID && b.Durum == true).ToList();
                                    foreach (var b in ozellikler)
                                    {
                                        if (b.UrunAltOzellikleriID == ozellikid) { stokdurum = true; stokid = b.UrunStokID; } else { stokdurum = false; break; }
                                    }
                                }

                            }
                        }

                        foreach (var x in model.Ozellikler)
                        {
                            int ozellikid = Convert.ToInt32(x.OzellikAdi);
                            SiparisIcerikUrunOzellikleri so = new SiparisIcerikUrunOzellikleri();
                            so.SiaprisIcerikID = sipicid.ID;
                            so.UrunID = model.ID;
                            if (stokdurum == true)
                            {
                                so.UrunStoklariID = stokid;
                            }
                            so.UrunAltOzellikID = ozellikid;
                            so.Durum = true;
                            c.SiparisIcerikUrunOzellikleris.Add(so);
                            c.SaveChanges();
                        }
                    }
                }
                if (bayi.IskontoOran != null) sip.IskontoOran = bayi.IskontoOran; else sip.IskontoOran = 0;
                c.SaveChanges();
                siphesapla(sip.ID);
                result = new { status = "success", message = "Kayıt Başarılı..." };
            }
            return Json(result);
        }
        public void siphesapla(int id)
        {
            var sip = c.Siparis.FirstOrDefault(v => v.ID == id);
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
            c.SaveChanges();
        }

        public class SepetEkleModel
        {
            public int ID { get; set; }
            public string? Aciklama { get; set; }
            public int? Miktar { get; set; }
            public List<DtoUrunAltOzellikleri>? Ozellikler { get; set; }
        }
    }
}
