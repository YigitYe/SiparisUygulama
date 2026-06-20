using Microsoft.AspNetCore.Mvc;
using SiparisUygulama.Models;
using SiparisUygulama.Helpers;

public class GirisController : Controller
{
    private readonly SiparisDbContext _context;

    public GirisController(SiparisDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Index(string kullaniciAdi, string sifre, string girisTipi)
    {
        if (girisTipi == "Musteri")
        {
            var musteri = _context.Kullanicis
                .FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);

            if (musteri == null || !PasswordHelper.Dogrula(sifre, musteri.Sifre))
            {
                ViewBag.Hata = "Müşteri girişi başarısız!";
                return View();
            }

            HttpContext.Session.SetInt32("KullaniciID", musteri.KullaniciId);
            HttpContext.Session.SetString("Rol", "Musteri");
            HttpContext.Session.SetString("KullaniciAdi", musteri.KullaniciAdi);
            if (!string.IsNullOrEmpty(musteri.Avatar))
                HttpContext.Session.SetString("Avatar", musteri.Avatar);
            return RedirectToAction("Index", "Home");
        }

        else if (girisTipi == "Yonetici")
        {
            var yonetici = _context.Yoneticis
                .FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);

            if (yonetici == null || !PasswordHelper.Dogrula(sifre, yonetici.Sifre))
            {
                ViewBag.Hata = "Yönetici girişi başarısız!";
                return View();
            }

            HttpContext.Session.SetInt32("KullaniciId", yonetici.KullaniciId);
            HttpContext.Session.SetString("Rol", yonetici.Rol);
            HttpContext.Session.SetString("KullaniciAdi", yonetici.KullaniciAdi);

            return yonetici.Rol switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Restoran" => RedirectToAction("Index", "RestoranYonetim"),
                
                _ => RedirectToAction("Index", "Home")
            };
        }

        else if (girisTipi == "Personel")
        {
            var personel = _context.Personels.FirstOrDefault(x => x.Adi == kullaniciAdi);
            bool girisBasarili = personel != null && (
                (!string.IsNullOrEmpty(personel.Sifre) && PasswordHelper.Dogrula(sifre, personel.Sifre))
                || (string.IsNullOrEmpty(personel.Sifre) && personel.IletisimNumarasi == sifre)
            );

            if (!girisBasarili || personel == null)
            {
                ViewBag.Hata = "Personel girişi başarısız!";
                return View();
            }

            HttpContext.Session.SetInt32("PersonelID", personel.PersonelId);
            HttpContext.Session.SetString("Rol", "Personel");
            HttpContext.Session.SetString("KullaniciAdi", personel.Adi);

            return RedirectToAction("BekleyenTeslimatlar", "Personel");
        }

        ViewBag.Hata = "Geçersiz giriş türü!";
        return View();
    }
    [HttpPost]
    public IActionResult KullaniciKayit(Kullanici yeniKullanici)
    {
        if (string.IsNullOrWhiteSpace(yeniKullanici.KullaniciAdi) || string.IsNullOrWhiteSpace(yeniKullanici.Sifre))
        {
            return BadRequest("Kullanıcı adı ve şifre boş olamaz.");
        }

        yeniKullanici.Sifre = PasswordHelper.Hash(yeniKullanici.Sifre);
        _context.Kullanicis.Add(yeniKullanici);
        _context.SaveChanges();

        return Ok("Kayıt başarılı");
    }

    public IActionResult Yetkisiz()
    {
        return View();
    }

    public IActionResult Cikis()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}