using DataAccessLayer.Concrate;
using EntityLayer.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VNNB2B.Models.Hata;

namespace VNNB2B.Controllers
{
    public class HammaddeController : Controller
    {
        private readonly Context c;
        public HammaddeController(Context context)
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
                ViewBag.hata = HammaddeHata.Icerik;
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
                var x = c.Hammaddes.FirstOrDefault(v => v.ID == id);
                DtoHammadde list = new DtoHammadde();
                list.ID = Convert.ToInt32(x.ID);
                if (x.HammaddeKodu != null) list.HammaddeKodu = x.HammaddeKodu.ToString(); else list.HammaddeKodu = "Tanımlanmamış...";
                if (x.HammaddeAciklama != null) list.HammaddeAciklama = x.HammaddeAciklama.ToString(); else list.HammaddeAciklama = "Tanımlanmamış...";
                if (x.KritikStokMiktari != null) list.KritikStokMiktari = x.KritikStokMiktari.ToString(); else list.KritikStokMiktari = "Tanımlanmamış...";
                if (x.BirimID != null) list.BirimID = c.Birimlers.FirstOrDefault(v => v.ID == x.BirimID).BirimAdi.ToString(); else list.BirimID = "Tanımlanmamış...";
                if (x.Stok != null) list.Stok = x.Stok.ToString(); else list.Stok = "0";
                list.Resim = "data:image/jpeg;base64," + Convert.ToBase64String(x.Resim);
                List<SelectListItem> birimler = (from v in c.Birimlers.Where(v => v.Durum == true).ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = v.BirimAdi.ToString(),
                                                     Value = v.ID.ToString()
                                                 }).ToList();

                ViewBag.birimler = birimler;
                ViewBag.id = id;
                ViewBag.hata = HammaddeHata.Icerik;
                return View(list);
            }
        }
    }
}
