using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;
using SiparisUygulama.Services;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparis.Controllers
{
    public class RestoranController : Controller
    {
        private readonly SiparisDbContext _context;
        private readonly AiServiceClient _ai;

        public RestoranController(SiparisDbContext context, AiServiceClient ai)
        {
            _context = context;
            _ai = ai;
        }

        private bool MusteriMi()
        {
            return HttpContext.Session.GetString("Rol") == "Musteri";
        }

        public async Task<IActionResult> Index(string? arama, string? mutfak, string? siralama)
        {
            if (!MusteriMi())
                return RedirectToAction("Yetkisiz", "Giris");

            var query = _context.Restorans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(arama))
                query = query.Where(r => r.RestaurantName.Contains(arama) || (r.MutfakTuru != null && r.MutfakTuru.Contains(arama)));

            if (!string.IsNullOrWhiteSpace(mutfak))
                query = query.Where(r => r.MutfakTuru != null && r.MutfakTuru == mutfak);

            var restoranlar = query.ToList();

            // Sıralama
            var restoranPuanlariAll = _context.Yorums
                .Where(y => y.Puan != null)
                .GroupBy(y => y.RestaurantId)
                .Select(g => new { RestoranId = g.Key, Ort = g.Average(y => (double)y.Puan!), Sayi = g.Count() })
                .ToList()
                .ToDictionary(x => x.RestoranId, x => (x.Ort, x.Sayi));

            restoranlar = siralama switch {
                "puan" => restoranlar.OrderByDescending(r => restoranPuanlariAll.ContainsKey(r.RestaurantId) ? restoranPuanlariAll[r.RestaurantId].Ort : 0).ToList(),
                "isim" => restoranlar.OrderBy(r => r.RestaurantName).ToList(),
                _ => restoranlar
            };

            ViewBag.Arama = arama;
            ViewBag.SecilenMutfak = mutfak;
            ViewBag.Siralama = siralama;
            ViewBag.MutfakTurleri = _context.Restorans.Where(r => r.MutfakTuru != null).Select(r => r.MutfakTuru!).Distinct().ToList();

            ViewBag.RestoranPuanlari = restoranPuanlariAll;

            // Kullanıcıya öneri getir
            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
            if (kullaniciId > 0)
            {
                var yorumPuanlari = _context.Yorums
                    .Where(y => y.Puan != null)
                    .GroupBy(y => new { y.KullaniciId, y.RestaurantId })
                    .Select(g => new { g.Key.KullaniciId, g.Key.RestaurantId, OrtPuan = (float)g.Average(y => (double)y.Puan!) })
                    .ToList();

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

                var oneri = await _ai.OneriGetirAsync(kullaniciId, puanlar);
                ViewBag.Oneriler = oneri?.Oneriler;
                ViewBag.OneriKaynak = oneri?.Kaynak;
            }

            // Favori restoranlar
            if (kullaniciId > 0)
            {
                var favoriler = _context.Favoris
                    .Where(f => f.KullaniciId == kullaniciId)
                    .Select(f => f.RestaurantId)
                    .ToHashSet();
                ViewBag.Favoriler = favoriler;
            }

            ViewData["Title"] = "Restoranlar";
            ViewData["HeaderTitle"] = "Restoranlar";
            ViewData["HeaderSubtitle"] = "Şehrinizdeki En İyi Restoranları Keşfedin";

            return View(restoranlar);
        }


        public IActionResult Details(int id)
        {
            var restoran = _context.Restorans
                .Include(r => r.Menus) 
                .FirstOrDefault(r => r.RestaurantId == id);

            if (restoran == null)
                return NotFound();

            return View(restoran);
        }


    }
}
