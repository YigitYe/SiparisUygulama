using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Helpers;
using SiparisUygulama.Hubs;
using SiparisUygulama.Models;
using SiparisUygulama.Models.ViewModels;

namespace SiparisUygulama.Controllers
{
    public class AdminController : Controller
    {
        private readonly SiparisDbContext _context;
        private readonly IHubContext<SiparisHub> _hub;

        public AdminController(SiparisDbContext context, IHubContext<SiparisHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        private bool IsAuthorized()
        {
            return HttpContext.Session.GetString("Rol") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAuthorized())
                return RedirectToAction("Yetkisiz", "Giris");

            var yorumlar = _context.Yorums.Where(y => y.DuyguAnalizSkoru.HasValue).ToList();

            var model = new AdminPanelViewModel
            {
                Restorans = _context.Restorans.ToList(),
                Personeller = _context.Personels.ToList(),
                Kullanicilar = _context.Kullanicis.ToList(),
                Yoneticiler = _context.Yoneticis.ToList(),
                Siparisler = _context.Siparis
                    .Include(s => s.Kullanici)
                    .Include(s => s.SiparisDetays).ThenInclude(sd => sd.MenuItem)
                    .OrderByDescending(s => s.OrderDate)
                    .ToList(),
                ToplamSiparis = _context.Siparis.Count(),
                ToplamGelir = _context.Siparis.Sum(s => s.TotalAmount) ?? 0,
                ToplamKullanici = _context.Kullanicis.Count(),
                AktifPersonel = _context.Personels.Count(p => p.MusaitlikDurumu),
                PozitifYorum = yorumlar.Count(y => y.DuyguAnalizSkoru >= 0.05f),
                NegatiYorum = yorumlar.Count(y => y.DuyguAnalizSkoru <= -0.05f),
                NotorYorum = yorumlar.Count(y => y.DuyguAnalizSkoru > -0.05f && y.DuyguAnalizSkoru < 0.05f)
            };

            // Son 7 günün sipariş ve gelir grafiği
            var tumSiparisler = _context.Siparis.ToList();
            for (int i = 6; i >= 0; i--)
            {
                var gun = DateTime.Today.AddDays(-i);
                var sonraki = gun.AddDays(1);
                var gunSiparisler = tumSiparisler.Where(s => s.OrderDate >= gun && s.OrderDate < sonraki).ToList();
                model.GrafikEtiketler.Add(gun.ToString("dd MMM"));
                model.GrafikDegerler.Add(gunSiparisler.Count);
                model.GelirDegerleri.Add(gunSiparisler.Sum(s => s.TotalAmount ?? 0));
            }

            // Restoran bazlı sipariş sayısı (top 6)
            var restoranSiparisler = _context.SiparisDetays
                .Include(sd => sd.MenuItem)
                .GroupBy(sd => sd.MenuItem.Restaurant.RestaurantName)
                .Select(g => new { Ad = g.Key, Sayi = g.Select(x => x.OrderId).Distinct().Count() })
                .OrderByDescending(x => x.Sayi)
                .Take(6)
                .ToList();
            model.RestoranGrafikEtiketler = restoranSiparisler.Select(x => x.Ad).ToList();
            model.RestoranGrafikDegerler = restoranSiparisler.Select(x => x.Sayi).ToList();

            // NLP duygu trend: son 14 günün ortalama skoru
            var tumYorumlar = _context.Yorums.Where(y => y.DuyguAnalizSkoru.HasValue && y.ReviewDate.HasValue).ToList();
            for (int i = 13; i >= 0; i--)
            {
                var gun = DateTime.Today.AddDays(-i);
                var gunYorumlar = tumYorumlar.Where(y => y.ReviewDate!.Value.Date == gun).ToList();
                model.NlpTrendEtiketler.Add(gun.ToString("dd MMM"));
                model.NlpTrendDegerler.Add(gunYorumlar.Any() ? Math.Round(gunYorumlar.Average(y => (double)y.DuyguAnalizSkoru!.Value), 2) : 0);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SiparisDurumGuncelle(int orderId, string yeniDurum)
        {
            if (!IsAuthorized()) return RedirectToAction("Yetkisiz", "Giris");
            var siparis = _context.Siparis.Find(orderId);
            if (siparis != null)
            {
                siparis.OrderStatus = yeniDurum;
                _context.SaveChanges();
                await _hub.Clients.All.SendAsync("SiparisDurumGuncellendi", orderId, yeniDurum);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult KullaniciSil(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Yetkisiz", "Giris");
            var k = _context.Kullanicis.Find(id);
            if (k != null)
            {
                _context.Kullanicis.Remove(k);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RestoranEkle(Restoran r)
        {
            _context.Restorans.Add(r);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RestoranGuncelle(Restoran r)
        {
            _context.Restorans.Update(r);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RestoranSil(int id)
        {
            var r = _context.Restorans.Find(id);
            if (r != null)
            {
                _context.Restorans.Remove(r);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        

        [HttpPost]
        public IActionResult PersonelEkle(Personel p)
        {
            _context.Personels.Add(p);
            _context.SaveChanges();
          
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult PersonelGuncelle(Personel p)
        {
            _context.Personels.Update(p);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PersonelSil(int id)
        {
            var p = _context.Personels.Find(id);
            if (p != null)
            {
                _context.Personels.Remove(p);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult YoneticiEkle(string kullaniciAdi, string sifre, string rol)
        {
            if (!IsAuthorized()) return RedirectToAction("Yetkisiz", "Giris");
            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
                return RedirectToAction("Index");
            _context.Yoneticis.Add(new Yonetici
            {
                KullaniciAdi = kullaniciAdi,
                Sifre = PasswordHelper.Hash(sifre),
                Rol = string.IsNullOrWhiteSpace(rol) ? "Restoran" : rol
            });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PersonelSifreGuncelle(int personelId, string yeniSifre)
        {
            if (!IsAuthorized()) return RedirectToAction("Yetkisiz", "Giris");
            var p = _context.Personels.Find(personelId);
            if (p != null && !string.IsNullOrWhiteSpace(yeniSifre))
            {
                p.Sifre = PasswordHelper.Hash(yeniSifre);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult YoneticiSil(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Yetkisiz", "Giris");
            var y = _context.Yoneticis.Find(id);
            if (y != null)
            {
                _context.Yoneticis.Remove(y);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult ExportCSV()
        {
            if (!IsAuthorized()) return RedirectToAction("Yetkisiz", "Giris");

            var siparisler = _context.Siparis
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisDetays).ThenInclude(sd => sd.MenuItem)
                .OrderByDescending(s => s.OrderDate)
                .ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("SiparisID,Musteri,Tarih,Tutar,Durum,Urunler,Adres");
            foreach (var s in siparisler)
            {
                var urunler = string.Join(" | ", s.SiparisDetays.Select(d => $"{d.MenuItem?.ItemName} x{d.Miktar}"));
                sb.AppendLine($"{s.OrderId},{EscapeCsv(s.Kullanici?.KullaniciAdi)},{s.OrderDate:yyyy-MM-dd HH:mm},{s.TotalAmount},{EscapeCsv(s.OrderStatus)},{EscapeCsv(urunler)},{EscapeCsv(s.DeliveryAddress)}");
            }

            var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv", $"siparisler_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}