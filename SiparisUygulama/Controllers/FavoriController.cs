using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;

public class FavoriController : Controller
{
    private readonly SiparisDbContext _context;

    public FavoriController(SiparisDbContext context)
    {
        _context = context;
    }

    private bool MusteriMi() => HttpContext.Session.GetString("Rol") == "Musteri";

    [HttpPost]
    public IActionResult Toggle(int restoranId)
    {
        if (!MusteriMi()) return Unauthorized();
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;

        var mevcut = _context.Favoris.FirstOrDefault(f => f.KullaniciId == kullaniciId && f.RestaurantId == restoranId);
        bool eklendi;
        if (mevcut != null)
        {
            _context.Favoris.Remove(mevcut);
            eklendi = false;
        }
        else
        {
            _context.Favoris.Add(new Favori { KullaniciId = kullaniciId, RestaurantId = restoranId });
            eklendi = true;
        }
        _context.SaveChanges();
        return Json(new { eklendi });
    }

    public IActionResult Favorilerim()
    {
        if (!MusteriMi()) return RedirectToAction("Yetkisiz", "Giris");
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;

        var favoriler = _context.Favoris
            .Include(f => f.Restoran)
            .Where(f => f.KullaniciId == kullaniciId)
            .OrderByDescending(f => f.EklenmeTarihi)
            .ToList();

        return View(favoriler);
    }
}
