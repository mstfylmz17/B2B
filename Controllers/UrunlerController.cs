using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class UrunlerController : Controller
    {
        private readonly Context c;
        public UrunlerController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {

                List<SelectListItem> urungrubu = (from x in c.UrunTurlaris.Where(x => x.Durum == true).ToList().OrderBy(v => v.SiraNo)
                                                  select new SelectListItem
                                                  {
                                                      Text = x.UrunGrubuAdi.ToString(),
                                                      Value = x.ID.ToString()
                                                  }).ToList();

                ViewBag.urungrubu = urungrubu;

                List<SelectListItem> birim = (from x in c.Birimlers.Where(x => x.Durum == true).ToList()
                                              select new SelectListItem
                                              {
                                                  Text = x.BirimAdi.ToString(),
                                                  Value = x.ID.ToString()
                                              }).ToList();

                ViewBag.birim = birim;

                List<SelectListItem> kat = (from x in c.UrunKategoris.Where(x => x.Durum == true).ToList().OrderBy(v => v.SiraNo)
                                            select new SelectListItem
                                            {
                                                Text = x.Adi.ToString(),
                                                Value = x.ID.ToString()
                                            }).ToList();

                ViewBag.kat = kat;

                List<SelectListItem> ozellikler = (from x in c.UrunOzelikTurlaris.Where(x => x.Durum == true).ToList().OrderBy(v => v.OzellikAdi)
                                                   select new SelectListItem
                                                   {
                                                       Text = x.OzellikAdi.ToString(),
                                                       Value = x.ID.ToString()
                                                   }).OrderBy(v => v.Text).ToList();

                ViewBag.ozellikler = ozellikler;

                ViewBag.hata = UrunHata.Icerik;
                return View();
            }
        }
        public IActionResult Detay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                List<SelectListItem> urungrubu = (from v in c.UrunTurlaris.Where(v => v.Durum == true).ToList().OrderBy(v => v.SiraNo)
                                                  select new SelectListItem
                                                  {
                                                      Text = v.UrunGrubuAdi.ToString(),
                                                      Value = v.ID.ToString()
                                                  }).ToList();

                ViewBag.urungrubu = urungrubu;

                List<SelectListItem> birim = (from v in c.Birimlers.Where(v => v.Durum == true).ToList()
                                              select new SelectListItem
                                              {
                                                  Text = v.BirimAdi.ToString(),
                                                  Value = v.ID.ToString()
                                              }).ToList();

                ViewBag.birim = birim;

                List<SelectListItem> kat = (from v in c.UrunKategoris.Where(v => v.Durum == true).ToList().OrderBy(v => v.SiraNo)
                                            select new SelectListItem
                                            {
                                                Text = v.Adi.ToString(),
                                                Value = v.ID.ToString()
                                            }).ToList();

                ViewBag.kat = kat;

                List<SelectListItem> urunozellikleri = (from v in c.UrunOzelikTurlaris.Where(v => v.Durum == true).ToList()
                                                        select new SelectListItem
                                                        {
                                                            Text = v.OzellikAdi.ToString(),
                                                            Value = v.ID.ToString()
                                                        }).ToList();

                ViewBag.urunozellikleri = urunozellikleri;

                ViewBag.id = id;
                decimal guncel = 0;
                var x = c.Urunlers.FirstOrDefault(v => v.ID == id);
                DtoUrunler list = new DtoUrunler();
                list.ID = Convert.ToInt32(x.ID);
                if (x.UrunKodu != null) list.UrunKodu = x.UrunKodu.ToString(); else list.UrunKodu = "Tanımlanmamış...";
                if (x.UrunAdi != null) list.UrunAdi = x.UrunAdi.ToString(); else list.UrunAdi = "Tanımlanmamış...";
                if (x.UrunAciklama != null) list.UrunAciklama = x.UrunAciklama.ToString(); else list.UrunAciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.Birim = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.Birim = "Tanımlanmamış...";
                list.UrunTuru = c.UrunTurlaris.FirstOrDefault(v => v.ID == x.UrunTuruID).UrunGrubuAdi.ToString();
                if (x.UrunKategoriID != null) list.UrunKategori = c.UrunKategoris.FirstOrDefault(v => v.ID == x.UrunKategoriID).Adi.ToString(); else list.UrunKategori = "Tanımlanmamış...";
                list.StokMiktari = c.UrunStoklaris.Where(v => v.ID == x.ID && v.Durum == true && v.StokMiktari > 0).Sum(v => v.StokMiktari).ToString();
                if (guncel < x.KritikStokMiktari) list.Durum = "Red";
                if (x.Resim != null) list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                if (x.BirimM3 != null) list.BirimM3 = x.BirimM3.ToString(); else list.BirimM3 = "Belirtilmedi...";
                if (x.BirimKG != null) list.BirimKG = x.BirimKG.ToString(); else list.BirimKG = "Belirtilmedi...";
                if (x.Boyut != null) list.Boyut = x.Boyut.ToString(); else list.Boyut = "Belirtilmedi...";
                if (x.PaketAdet != null) list.PaketAdet = x.PaketAdet.ToString(); else list.PaketAdet = "Belirtilmedi...";
                var fiyatlar = c.UrunFiyatlaris.FirstOrDefault(v => v.UrunID == id && v.Durum == true);
                if (fiyatlar != null)
                {
                    list.FiyatTl = Convert.ToDecimal(fiyatlar.FiyatTL).ToString("N2");
                    list.FiyatUSD = Convert.ToDecimal(fiyatlar.FiyatUSD).ToString("N2");
                }
                else
                {
                    list.FiyatTl = "0,00 ₺";
                    list.FiyatUSD = "0,00 $";
                }
                ViewBag.hata = UrunHata.Icerik;
                return View(list);
            }
        }
        public IActionResult Stok()
        {
            HttpContext.Request.Cookies.TryGetValue("VNNCerez", out var Cerez);
            if (Cerez == null && Cerez == "" && Cerez == "0")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {

                List<SelectListItem> kat = (from x in c.UrunKategoris.Where(x => x.Durum == true).ToList().OrderBy(v => v.SiraNo)
                                            select new SelectListItem
                                            {
                                                Text = x.Adi.ToString(),
                                                Value = x.ID.ToString()
                                            }).ToList();

                ViewBag.kat = kat;

                List<SelectListItem> urungrubu = (from x in c.UrunTurlaris.Where(x => x.Durum == true).ToList().OrderBy(v => v.SiraNo)
                                                  select new SelectListItem
                                                  {
                                                      Text = x.UrunGrubuAdi.ToString(),
                                                      Value = x.ID.ToString()
                                                  }).ToList();

                ViewBag.urungrubu = urungrubu;
                ViewBag.hata = UrunHata.Icerik;
                return View();
            }
        }
    }
}