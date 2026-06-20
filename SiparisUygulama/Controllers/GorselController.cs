using Microsoft.AspNetCore.Mvc;
using SiparisUygulama.Models;

namespace SiparisUygulama.Controllers
{
    public class GorselController : Controller
    {
        private readonly SiparisDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GorselController(SiparisDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private bool YetkiVar() =>
            HttpContext.Session.GetString("Rol") is "Admin" or "Restoran";

        [HttpPost]
        public async Task<IActionResult> RestoranGorsel(int restoranId, IFormFile gorsel)
        {
            if (!YetkiVar()) return RedirectToAction("Yetkisiz", "Giris");
            if (gorsel == null || gorsel.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var izinliTipler = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!izinliTipler.Contains(gorsel.ContentType))
                return BadRequest("Sadece JPG, PNG veya WEBP yükleyebilirsiniz.");

            if (gorsel.Length > 5 * 1024 * 1024)
                return BadRequest("Dosya 5MB'dan büyük olamaz.");

            var klasor = Path.Combine(_env.WebRootPath, "uploads", "restoranlar");
            Directory.CreateDirectory(klasor);

            var uzanti = Path.GetExtension(gorsel.FileName).ToLower();
            var dosyaAdi = $"restoran_{restoranId}{uzanti}";
            var yol = Path.Combine(klasor, dosyaAdi);

            using (var stream = new FileStream(yol, FileMode.Create))
                await gorsel.CopyToAsync(stream);

            var restoran = _context.Restorans.Find(restoranId);
            if (restoran != null)
            {
                restoran.GorselUrl = $"/uploads/restoranlar/{dosyaAdi}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                _context.SaveChanges();
            }

            return Ok(restoran?.GorselUrl);
        }

        [HttpPost]
        public async Task<IActionResult> MenuGorsel(int menuItemId, IFormFile gorsel)
        {
            if (!YetkiVar()) return RedirectToAction("Yetkisiz", "Giris");
            if (gorsel == null || gorsel.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var izinliTipler = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!izinliTipler.Contains(gorsel.ContentType))
                return BadRequest("Sadece JPG, PNG veya WEBP yükleyebilirsiniz.");

            if (gorsel.Length > 5 * 1024 * 1024)
                return BadRequest("Dosya 5MB'dan büyük olamaz.");

            var klasor = Path.Combine(_env.WebRootPath, "uploads", "menu");
            Directory.CreateDirectory(klasor);

            var uzanti = Path.GetExtension(gorsel.FileName).ToLower();
            var dosyaAdi = $"menu_{menuItemId}{uzanti}";
            var yol = Path.Combine(klasor, dosyaAdi);

            using (var stream = new FileStream(yol, FileMode.Create))
                await gorsel.CopyToAsync(stream);

            var menu = _context.Menus.Find(menuItemId);
            if (menu != null)
            {
                menu.GorselUrl = $"/uploads/menu/{dosyaAdi}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                _context.SaveChanges();
            }

            return Ok(menu?.GorselUrl);
        }
    }
}
