using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Hubs;
using SiparisUygulama.Models;
using SiparisUygulama.Services;
using System.Text.Json;

public class PersonelController : Controller
{
    private readonly SiparisDbContext _context;
    private readonly AiServiceClient _ai;
    private readonly IHubContext<SiparisHub> _hub;

    public PersonelController(SiparisDbContext context, AiServiceClient ai, IHubContext<SiparisHub> hub)
    {
        _context = context;
        _ai = ai;
        _hub = hub;
    }

    private bool PersonelMi()
    {
        return HttpContext.Session.GetString("Rol") == "Personel";
    }


    public IActionResult BekleyenTeslimatlar()
    {
        if (!PersonelMi())
            return RedirectToAction("Yetkisiz", "Giris");
        var teslimatlar = _context.Teslimats
            .Include(t => t.Order)
            .ThenInclude(o => o.Kullanici)
            .Where(t => t.Order.OrderStatus == "Hazır" && (t.PersonelId == null || t.PersonelId == 0))
            .ToList();

        return View(teslimatlar);
    }

   
    public IActionResult Teslimatlarim()
    {
        if (HttpContext.Session.GetString("Rol") == null)
        {
           
            return RedirectToAction("Index", "Giris");
        }
        int personelId = HttpContext.Session.GetInt32("PersonelID") ?? 0;

        var teslimatlar = _context.Teslimats
            .Include(t => t.Order)
            .ThenInclude(o => o.Kullanici)
            .Where(t => t.PersonelId == personelId)
            .ToList();

        return View(teslimatlar);
    }


    [HttpPost]
    public async Task<IActionResult> Ata(int deliveryId)
    {
        int personelId = HttpContext.Session.GetInt32("PersonelID") ?? 0;

        var teslimat = _context.Teslimats
            .Include(t => t.Order)
            .ThenInclude(o => o.Kullanici)
            .FirstOrDefault(t => t.DeliveryId == deliveryId);

        if (teslimat != null && teslimat.PersonelId == null)
        {
            teslimat.PersonelId = personelId;
            teslimat.DeliveryStatus = "Yolda";
            if (teslimat.Order != null)
                teslimat.Order.OrderStatus = "Yolda";

            _context.SaveChanges();

            // Kuryenin TÜM aktif teslimatlarını birden TSP ile optimize et
            var personel = _context.Personels.Find(personelId);
            if (personel?.AnlikEnlem != null)
            {
                var aktifTeslimatlar = _context.Teslimats
                    .Include(t => t.Order)
                    .ThenInclude(o => o.Kullanici)
                    .Where(t => t.PersonelId == personelId && t.DeliveryStatus == "Yolda")
                    .ToList();

                var noktalar = aktifTeslimatlar
                    .Where(t => t.Order?.Kullanici?.VarsayilanEnlem != null)
                    .Select(t => new AiServiceClient.TeslimatNokta
                    {
                        SiparisId = t.OrderId,
                        Enlem = t.Order!.Kullanici!.VarsayilanEnlem!.Value,
                        Boylam = t.Order.Kullanici.VarsayilanBoylam ?? 0
                    }).ToList();

                if (noktalar.Any())
                {
                    var rota = await _ai.RotaOptimizeAsync(
                        personel.AnlikEnlem.Value,
                        personel.AnlikBoylam ?? 0,
                        noktalar
                    );

                    if (rota != null)
                    {
                        var rotaJson = JsonSerializer.Serialize(rota);
                        // Tüm aktif teslimatları aynı rota bilgisiyle güncelle
                        foreach (var t in aktifTeslimatlar)
                            t.RotaBilgisi = rotaJson;
                        _context.SaveChanges();
                    }
                }
            }

            await _hub.Clients.All.SendAsync("SiparisDurumGuncellendi", teslimat.OrderId, "Yolda");
        }

        return RedirectToAction("Teslimatlarim");
    }

    [HttpPost]
    public async Task<IActionResult> TeslimEt(int deliveryId)
    {
        if (!PersonelMi()) return RedirectToAction("Yetkisiz", "Giris");
        int personelId = HttpContext.Session.GetInt32("PersonelID") ?? 0;

        var teslimat = _context.Teslimats
            .Include(t => t.Order)
            .FirstOrDefault(t => t.DeliveryId == deliveryId && t.PersonelId == personelId);

        if (teslimat != null && teslimat.DeliveryStatus == "Yolda")
        {
            teslimat.DeliveryStatus = "Teslim Edildi";
            teslimat.DeliveryTime = DateTime.Now;
            if (teslimat.Order != null)
                teslimat.Order.OrderStatus = "Teslim Edildi";
            _context.SaveChanges();
            await _hub.Clients.All.SendAsync("SiparisDurumGuncellendi", teslimat.OrderId, "Teslim Edildi");
        }

        return RedirectToAction("Teslimatlarim");
    }

    [HttpPost]
    public async Task<IActionResult> KonumGuncelle([FromBody] KonumGuncelleRequest req)
    {
        if (!PersonelMi()) return Unauthorized();
        int personelId = HttpContext.Session.GetInt32("PersonelID") ?? 0;
        var personel = _context.Personels.Find(personelId);
        if (personel != null)
        {
            personel.AnlikEnlem = req.Enlem;
            personel.AnlikBoylam = req.Boylam;
            personel.MusaitlikDurumu = true;
            _context.SaveChanges();

            // Aktif teslimatlar için konum SignalR üzerinden broadcast et
            var aktifOrderIdler = _context.Teslimats
                .Where(t => t.PersonelId == personelId && t.DeliveryStatus == "Yolda")
                .Select(t => t.OrderId)
                .ToList();

            foreach (var orderId in aktifOrderIdler)
                await _hub.Clients.All.SendAsync("KureyeKonumu", orderId, req.Enlem, req.Boylam);
        }
        return Ok();
    }
}

public class KonumGuncelleRequest
{
    public double Enlem { get; set; }
    public double Boylam { get; set; }
}