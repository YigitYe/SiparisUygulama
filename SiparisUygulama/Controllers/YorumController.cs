using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;
using SiparisUygulama.Services;

public class YorumController : Controller
{
    private readonly SiparisDbContext _context;
    private readonly AiServiceClient _ai;

    public YorumController(SiparisDbContext context, AiServiceClient ai)
    {
        _context = context;
        _ai = ai;
    }

    [HttpGet]
    public IActionResult Ekle(int orderId)
    {
        if (HttpContext.Session.GetString("Rol") == null)
        {

            return RedirectToAction("Index", "Giris");
        }

        var siparis = _context.Siparis
            .Include(s => s.Teslimats)
            .FirstOrDefault(s => s.OrderId == orderId);

        if (siparis == null || !siparis.Teslimats.Any(t => t.DeliveryStatus == "Teslim Edildi"))
        {
            return BadRequest("Yorum yapılabilecek bir sipariş bulunamadı.");
        }

        ViewBag.OrderId = orderId;
        return View();
    }

    public IActionResult RestoranYorumlari(int restoranId)
    {
        if (HttpContext.Session.GetString("Rol") == null)
        {

            return RedirectToAction("Index", "Giris");
        }
        var yorumlar = _context.Yorums
            .Include(y => y.Kullanici)
            .Where(y => y.RestaurantId == restoranId)
            .OrderByDescending(y => y.ReviewDate)
            .ToList();

        ViewBag.RestoranAdi = _context.Restorans.Find(restoranId)?.RestaurantName;
        return View(yorumlar);
    }



    [HttpPost]
    public async Task<IActionResult> Ekle(int orderId, int puan, string yorumMetni)
    {
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;

        var siparis = _context.Siparis
            .Include(s => s.SiparisDetays)
            .ThenInclude(sd => sd.MenuItem)
            .FirstOrDefault(s => s.OrderId == orderId);

        if (siparis == null)
            return NotFound();

        var restaurantId = siparis.SiparisDetays
            .Select(sd => sd.MenuItem.RestaurantId)
            .FirstOrDefault();

        float? duyguSkoru = null;
        if (!string.IsNullOrWhiteSpace(yorumMetni))
            duyguSkoru = await _ai.DuyguAnalizAsync(yorumMetni);

        var yorum = new Yorum
        {
            KullaniciId = kullaniciId,
            RestaurantId = restaurantId,
            Puan = puan,
            Yorum1 = yorumMetni,
            ReviewDate = DateTime.Now,
            DuyguAnalizSkoru = duyguSkoru
        };

        _context.Yorums.Add(yorum);
        _context.SaveChanges();

        return RedirectToAction("Siparislerim", "Siparis");
    }
}