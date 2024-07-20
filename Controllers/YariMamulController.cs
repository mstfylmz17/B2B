using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class YariMamulController : Controller
    {
        private readonly Context c;
        public YariMamulController(Context context)
        {
            c = context;
        }
        public IActionResult Index()
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                List<SelectListItem> birimler = (from x in c.Birimlers.Where(x => x.Durum == true).ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = x.BirimAdi.ToString(),
                                                     Value = x.ID.ToString()
                                                 }).ToList();

                ViewBag.birimler = birimler;

                List<SelectListItem> yarimamulgruplari = (from x in c.YariMamulGruplaris.Where(x => x.Durum == true).ToList()
                                                          select new SelectListItem
                                                          {
                                                              Text = x.Adi.ToString(),
                                                              Value = x.ID.ToString()
                                                          }).ToList();

                ViewBag.yarimamulgruplari = yarimamulgruplari;
                ViewBag.hata = YariMamulHata.Icerik;
                return View();
            }
        }
        public IActionResult Detay(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("EnvanterTakipCerez", out var Cerez);
            if (Cerez == null && Cerez == "")
            {
                LoginHata.Icerik = "Lütfen Giriş Yapınız...";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var x = c.YariMamuls.FirstOrDefault(v => v.ID == id);
                DtoYariMamul list = new DtoYariMamul();
                list.ID = Convert.ToInt32(x.ID);
                if (x.Kodu != null) list.Kodu = x.Kodu.ToString(); else list.Kodu = "Tanımlanmamış...";
                if (x.Aciklama != null) list.Aciklama = x.Aciklama.ToString(); else list.Aciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.BirimID = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.BirimID = "Tanımlanmamış...";
                if (x.YariMamulGrupID != null) list.YariMamulGrupID = c.YariMamulGruplaris.FirstOrDefault(v => v.ID == x.YariMamulGrupID).Adi.ToString(); else list.YariMamulGrupID = "Tanımlanmamış...";
                if (x.Stok != null) list.Stok = x.Stok.ToString(); else list.Stok = "0";
                list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                
                List<SelectListItem> birimler = (from v in c.Birimlers.Where(v => v.Durum == true).ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = v.BirimAdi.ToString(),
                                                     Value = v.ID.ToString()
                                                 }).ToList();

                ViewBag.birimler = birimler;

                List<SelectListItem> yarimamulgruplari = (from v in c.YariMamulGruplaris.Where(v => v.Durum == true).ToList()
                                                          select new SelectListItem
                                                          {
                                                              Text = v.Adi.ToString(),
                                                              Value = v.ID.ToString()
                                                          }).ToList();

                ViewBag.yarimamulgruplari = yarimamulgruplari;
                ViewBag.id = id;
                ViewBag.hata = HammaddeHata.Icerik;
                return View(list);
            }
        }
    }
}
