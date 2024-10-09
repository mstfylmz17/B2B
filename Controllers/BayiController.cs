using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class BayiController : Controller
    {
        private readonly Context c;
        public BayiController(Context context)
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
                List<SelectListItem> parabirimleri = (from v in c.ParaBirimleris.Where(v => v.Durum == true).ToList()
                                                      select new SelectListItem
                                                      {
                                                          Text = v.ParaBirimAdi.ToString(),
                                                          Value = v.ID.ToString()
                                                      }).ToList();

                ViewBag.parabirimleri = parabirimleri;

                List<SelectListItem> kdvdurumlari = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "KDV'li SATIŞ",
                        Value = true.ToString()
                    },
                    new SelectListItem
                    {
                        Text = "KDV'siz SATIŞ",
                        Value = false.ToString()
                    },
                    new SelectListItem
                    {
                        Text = "İHR.FAT. SATIŞ",
                        Value = false.ToString()
                    },
                    new SelectListItem
                    {
                        Text = "İHR.KAY. Satış",
                        Value = false.ToString()
                    }
                };

                ViewBag.kdvdurumlari = kdvdurumlari;
                ViewBag.hata = BayiHata.Icerik;
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
                List<SelectListItem> parabirimleri = (from v in c.ParaBirimleris.Where(v => v.Durum == true).ToList()
                                                      select new SelectListItem
                                                      {
                                                          Text = v.ParaBirimAdi.ToString(),
                                                          Value = v.ID.ToString()
                                                      }).ToList();

                ViewBag.parabirimleri = parabirimleri;

                List<SelectListItem> kdvdurumlari = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = "KDV'li SATIŞ",
                        Value = true.ToString()
                    },
                    new SelectListItem
                    {
                        Text = "KDV'siz SATIŞ",
                        Value = false.ToString()
                    },
                    new SelectListItem
                    {
                        Text = "İHR.FAT. SATIŞ",
                        Value = false.ToString()
                    },
                    new SelectListItem
                    {
                        Text = "İHR.KAY. Satış",
                        Value = false.ToString()
                    }
                };

                ViewBag.kdvdurumlari = kdvdurumlari;

                var bayi = c.Bayilers.FirstOrDefault(v => v.ID == id);
                DtoBayiler veri = new DtoBayiler();
                if (bayi.Unvan != null) veri.Unvan = bayi.Unvan; else veri.Unvan = "";
                if (bayi.KullaniciAdi != null) veri.KullaniciAdi = bayi.KullaniciAdi; else veri.KullaniciAdi = "";
                if (bayi.Sifre != null) veri.Sifre = bayi.Sifre; else veri.Sifre = "";
                if (bayi.Adres != null) veri.Adres = bayi.Adres; else veri.Adres = "";
                if (bayi.Telefon != null) veri.Telefon = bayi.Telefon; else veri.Telefon = "";
                if (bayi.EPosta != null) veri.EPosta = bayi.EPosta; else veri.EPosta = "";
                if (bayi.Yetkili != null) veri.Yetkili = bayi.Yetkili; else veri.Yetkili = "";
                if (bayi.IskontoOran != null) veri.IskontoOran = bayi.IskontoOran.ToString(); else veri.IskontoOran = "";
                if (bayi.AlisVerisLimiti != null) veri.AlisVerisLimiti = bayi.AlisVerisLimiti.ToString(); else veri.AlisVerisLimiti = "";
                if (bayi.BayiKodu != null) veri.BayiKodu = bayi.BayiKodu.ToString(); else veri.BayiKodu = "";
                if (bayi.ParaBirimi != null)
                {
                    var para = c.ParaBirimleris.FirstOrDefault(v => v.ID == bayi.ParaBirimi);
                    if (para != null) veri.ParaBirimi = para.ParaBirimAdi.ToString();
                }
                else veri.ParaBirimi = "TL";
                if (bayi.KDVDurum == true) veri.KDVDurumu = "KDV'li SATIŞ"; else { if (bayi.KDVBilgi != null) { veri.KDVDurumu = bayi.KDVBilgi.ToString(); } else veri.KDVDurumu = "KDV'siz SATIŞ"; };
                ViewBag.id = id;
                ViewBag.hata = BayiHata.Icerik;
                return View(veri);
            }
        }
    }
}
