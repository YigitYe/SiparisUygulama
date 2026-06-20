using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;
using SiparisUygulama.Services;

namespace SiparisUygulama.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SiparisDbContext _context;
    private readonly AiServiceClient _ai;

    public HomeController(ILogger<HomeController> logger, SiparisDbContext context, AiServiceClient ai)
    {
        _logger = logger;
        _context = context;
        _ai = ai;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("Rol") == null)
            return RedirectToAction("Index", "Giris");

        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;

        if (kullaniciId > 0)
        {
            // Yorum bazlı puanlar
            var yorumPuanlari = _context.Yorums
                .Where(y => y.Puan != null)
                .GroupBy(y => new { y.KullaniciId, y.RestaurantId })
                .Select(g => new { g.Key.KullaniciId, g.Key.RestaurantId, OrtPuan = (float)g.Average(y => (double)y.Puan!) })
                .ToList();

            // Sipariş geçmişinden implicit puan
            var siparisPuanlari = _context.SiparisDetays
                .Where(sd => sd.Order.OrderStatus == "Teslim Edildi" && sd.MenuItem != null)
                .GroupBy(sd => new { sd.Order.KullaniciId, sd.MenuItem!.RestaurantId })
                .Select(g => new { g.Key.KullaniciId, g.Key.RestaurantId, SiparisSayisi = g.Count() })
                .ToList();

            var puanDict = new Dictionary<int, Dictionary<int, float>>();
            foreach (var sp in siparisPuanlari)
            {
                if (!puanDict.ContainsKey(sp.KullaniciId)) puanDict[sp.KullaniciId] = new();
                puanDict[sp.KullaniciId][sp.RestaurantId] = (float)Math.Min(5.0, sp.SiparisSayisi * 1.5);
            }
            foreach (var yp in yorumPuanlari)
            {
                if (!puanDict.ContainsKey(yp.KullaniciId)) puanDict[yp.KullaniciId] = new();
                puanDict[yp.KullaniciId][yp.RestaurantId] = yp.OrtPuan;
            }

            var puanlar = puanDict.ToDictionary(
                kv => kv.Key.ToString(),
                kv => kv.Value.ToDictionary(x => x.Key.ToString(), x => x.Value)
            );

            // Kullanıcının daha önce sipariş verdiği restoranlar
            var siparisVerilenRestoranIds = puanDict.ContainsKey(kullaniciId)
                ? puanDict[kullaniciId].Keys.ToHashSet()
                : new HashSet<int>();

            var oneri = await _ai.OneriGetirAsync(kullaniciId, puanlar);
            if (oneri?.Oneriler != null && oneri.Oneriler.Any())
            {
                var restoranIds = oneri.Oneriler.Select(o => o.MenuItemId).ToList();
                var restoranlarHam = _context.Restorans
                    .Where(r => restoranIds.Contains(r.RestaurantId))
                    .ToList();
                // CF sıralamasını koru
                var restoranlar = restoranIds
                    .Select(id => restoranlarHam.FirstOrDefault(r => r.RestaurantId == id))
                    .Where(r => r != null)
                    .ToList()!;

                if (restoranlar.Any())
                {
                    // Yıldızda SVD ham skoru yerine restoranın GERÇEK ortalama puanını göster
                    var gercekPuanlar = _context.Yorums
                        .Where(y => restoranIds.Contains(y.RestaurantId) && y.Puan != null)
                        .GroupBy(y => y.RestaurantId)
                        .Select(g => new { g.Key, Ort = (float)g.Average(y => (double)y.Puan!) })
                        .ToDictionary(x => x.Key, x => x.Ort);

                    ViewBag.OneriRestoranlar = restoranlar;
                    ViewBag.Oneriler = oneri.Oneriler;
                    ViewBag.OneriKaynak = oneri.Kaynak;
                    ViewBag.OneriPuanlar = gercekPuanlar;
                }
            }

            // AI başarısız olduysa veya öneri yoksa: DB'den popüler restoranlar
            if (ViewBag.OneriRestoranlar == null)
            {
                var populer = _context.Restorans
                    .Where(r => !siparisVerilenRestoranIds.Contains(r.RestaurantId))
                    .Select(r => new {
                        Restoran = r,
                        OrtPuan = r.Yorums.Any() ? r.Yorums.Average(y => (double?)y.Puan ?? 0) : 0
                    })
                    .OrderByDescending(x => x.OrtPuan)
                    .Take(6)
                    .ToList();

                if (!populer.Any())
                {
                    populer = _context.Restorans
                        .Select(r => new {
                            Restoran = r,
                            OrtPuan = r.Yorums.Any() ? r.Yorums.Average(y => (double?)y.Puan ?? 0) : 0
                        })
                        .OrderByDescending(x => x.OrtPuan)
                        .Take(6)
                        .ToList();
                }

                ViewBag.OneriRestoranlar = populer.Select(x => x.Restoran).ToList();
                ViewBag.OneriPuanlar = populer.ToDictionary(x => x.Restoran.RestaurantId, x => (float)x.OrtPuan);
                ViewBag.OneriKaynak = "populer";
            }

            // Son siparişler
            var sonSiparisler = _context.Siparis
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.OrderDate)
                .Take(3)
                .Select(s => new { s.OrderId, s.OrderDate, s.TotalAmount, s.OrderStatus })
                .ToList();
            ViewBag.SonSiparisler = sonSiparisler;
        }

        // Mutfak türleri
        ViewBag.MutfakTurleri = _context.Restorans
            .Where(r => r.MutfakTuru != null)
            .Select(r => r.MutfakTuru!)
            .Distinct()
            .ToList();

        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult HttpError(int? code)
    {
        ViewBag.StatusCode = code ?? 404;
        return View();
    }
}
