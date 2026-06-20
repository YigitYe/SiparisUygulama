using Microsoft.AspNetCore.Mvc;
using SiparisUygulama.Helpers;
using SiparisUygulama.Models;

namespace SiparisUygulama.Controllers
{
    public class ProfilController : Controller
    {
        private readonly SiparisDbContext _context;

        public ProfilController(SiparisDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol == null)
                return RedirectToAction("Index", "Giris");

            ViewBag.Rol = rol;

            if (rol == "Musteri")
            {
                int id = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
                var kullanici = _context.Kullanicis.Find(id);
                if (kullanici == null) return RedirectToAction("Index", "Giris");
                return View("Index", kullanici);
            }
            else if (rol == "Personel")
            {
                int id = HttpContext.Session.GetInt32("PersonelID") ?? 0;
                var personel = _context.Personels.Find(id);
                if (personel == null) return RedirectToAction("Index", "Giris");
                var fakeKullanici = new Kullanici
                {
                    KullaniciId = personel.PersonelId,
                    KullaniciAdi = personel.Adi ?? "",
                    Telefon = personel.IletisimNumarasi,
                    TeslimatAdresi = personel.TeslimatAlani,
                    Sifre = personel.Sifre ?? ""
                };
                return View("Index", fakeKullanici);
            }
            else
            {
                int id = HttpContext.Session.GetInt32("KullaniciId") ?? 0;
                var yonetici = _context.Yoneticis.Find(id);
                if (yonetici == null) return RedirectToAction("Index", "Giris");
                var fakeKullanici = new Kullanici
                {
                    KullaniciId = yonetici.KullaniciId,
                    KullaniciAdi = yonetici.KullaniciAdi,
                    Sifre = yonetici.Sifre
                };
                return View("Index", fakeKullanici);
            }
        }

        [HttpPost]
        public IActionResult ProfilGuncelle(string email, string telefon, string teslimatAdresi)
        {
            int id = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
            var kullanici = _context.Kullanicis.Find(id);
            if (kullanici == null) return NotFound();

            kullanici.Email = email;
            kullanici.Telefon = telefon;
            kullanici.TeslimatAdresi = teslimatAdresi;
            _context.SaveChanges();

            TempData["Mesaj"] = "Profil bilgileri güncellendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SifreDegistir(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (yeniSifre != yeniSifreTekrar)
            {
                TempData["Hata"] = "Yeni şifreler eşleşmiyor.";
                return RedirectToAction("Index");
            }

            if (rol == "Musteri")
            {
                int id = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
                var kullanici = _context.Kullanicis.Find(id);
                if (kullanici == null || !PasswordHelper.Dogrula(mevcutSifre, kullanici.Sifre))
                {
                    TempData["Hata"] = "Mevcut şifre yanlış.";
                    return RedirectToAction("Index");
                }
                kullanici.Sifre = PasswordHelper.Hash(yeniSifre);
                _context.SaveChanges();
            }
            else
            {
                int id = HttpContext.Session.GetInt32("KullaniciId") ?? 0;
                var yonetici = _context.Yoneticis.Find(id);
                if (yonetici == null || !PasswordHelper.Dogrula(mevcutSifre, yonetici.Sifre))
                {
                    TempData["Hata"] = "Mevcut şifre yanlış.";
                    return RedirectToAction("Index");
                }
                yonetici.Sifre = PasswordHelper.Hash(yeniSifre);
                _context.SaveChanges();
            }

            TempData["Mesaj"] = "Şifre başarıyla değiştirildi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AvatarGuncelle([FromBody] string avatar)
        {
            int id = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
            var kullanici = _context.Kullanicis.Find(id);
            if (kullanici == null) return NotFound();
            kullanici.Avatar = avatar;
            _context.SaveChanges();
            HttpContext.Session.SetString("Avatar", avatar);
            return Ok();
        }

        [HttpPost]
        public IActionResult KonumGuncelle(double enlem, double boylam)
        {
            int id = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
            var kullanici = _context.Kullanicis.Find(id);
            if (kullanici == null) return NotFound();

            kullanici.VarsayilanEnlem = enlem;
            kullanici.VarsayilanBoylam = boylam;
            _context.SaveChanges();

            TempData["Mesaj"] = "Konum güncellendi.";
            return RedirectToAction("Index");
        }
    }
}
