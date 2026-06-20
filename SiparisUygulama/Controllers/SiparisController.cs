using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;

public class SiparisController : Controller
{
    private readonly SiparisDbContext _context;

    public SiparisController(SiparisDbContext context)
    {
        _context = context;
    }
    private bool MusteriMi()
    {
        return HttpContext.Session.GetString("Rol") == "Musteri";
    }

    public IActionResult SepeteEkle(int id)
    {
        var menuItem = _context.Menus.FirstOrDefault(m => m.MenuItemId == id);
        if (menuItem == null) return NotFound();

        var sepet = HttpContext.Session.GetObject<List<SepetItem>>("sepet") ?? new List<SepetItem>();
        var mevcut = sepet.FirstOrDefault(s => s.MenuItemID == id);

        if (mevcut != null)
            mevcut.Miktar++;
        else
            sepet.Add(new SepetItem
            {
                MenuItemID = menuItem.MenuItemId,
                ItemName = menuItem.ItemName,
                Fiyat = menuItem.Fiyat,
                Miktar = 1
            });

        HttpContext.Session.SetObject("sepet", sepet);
        return RedirectToAction("Sepet");
    }

    [HttpPost]
    public IActionResult SilAjax(int id)
    {
        var menu = _context.Menus.FirstOrDefault(x => x.MenuItemId == id);
        if (menu == null) return NotFound();

        _context.Menus.Remove(menu);
        _context.SaveChanges();
        return Ok();
    }

    [HttpPost]
    public IActionResult SepeteEkleAjax(int id)
    {
        var menuItem = _context.Menus.FirstOrDefault(m => m.MenuItemId == id);
        if (menuItem == null) return NotFound();

        var sepet = HttpContext.Session.GetObject<List<SepetItem>>("sepet") ?? new List<SepetItem>();
        var mevcut = sepet.FirstOrDefault(s => s.MenuItemID == id);

        if (mevcut != null)
            mevcut.Miktar++;
        else
            sepet.Add(new SepetItem
            {
                MenuItemID = menuItem.MenuItemId,
                ItemName = menuItem.ItemName,
                Fiyat = menuItem.Fiyat,
                Miktar = 1
            });

        HttpContext.Session.SetObject("sepet", sepet);
        return Ok();
    }

    [HttpPost]
    public IActionResult SepeteTopluEkle([FromBody] List<int> menuIds)
    {
        var sepet = HttpContext.Session.GetObject<List<SepetItem>>("sepet") ?? new List<SepetItem>();

        foreach (var id in menuIds)
        {
            var menuItem = _context.Menus.FirstOrDefault(m => m.MenuItemId == id);
            if (menuItem == null) continue;

            var mevcut = sepet.FirstOrDefault(s => s.MenuItemID == id);
            if (mevcut != null)
                mevcut.Miktar++;
            else
                sepet.Add(new SepetItem
                {
                    MenuItemID = menuItem.MenuItemId,
                    ItemName = menuItem.ItemName,
                    Fiyat = menuItem.Fiyat,
                    Miktar = 1
                });
        }

        HttpContext.Session.SetObject("sepet", sepet);
        return Ok();
    }

    [HttpPost]
    public IActionResult GuncelleAjax(int id, [FromForm] Menu guncel)
    {
        var menu = _context.Menus.FirstOrDefault(x => x.MenuItemId == id);
        if (menu == null) return NotFound();

        menu.ItemName = guncel.ItemName;
        menu.Aciklama = guncel.Aciklama;
        menu.Fiyat = guncel.Fiyat;
        menu.Kategori = guncel.Kategori;

        _context.SaveChanges();
        return Ok();
    }

    [HttpPost]
    public IActionResult SepettenSil(int id)
    {
        var sepet = HttpContext.Session.GetObject<List<SepetItem>>("sepet") ?? new List<SepetItem>();

        var silinecek = sepet.FirstOrDefault(x => x.MenuItemID == id);
        if (silinecek != null)
        {
            sepet.Remove(silinecek);
            HttpContext.Session.SetObject("sepet", sepet);
        }

        return RedirectToAction("Sepet");
    }

    [HttpPost]
    public IActionResult EkleAjax(Menu yeniUrun)
    {
        if (ModelState.IsValid)
        {
            _context.Menus.Add(yeniUrun);
            _context.SaveChanges();
            return Ok();
        }
        return BadRequest("Geçersiz veri");
    }

    public IActionResult Sepet()
    {
        if (HttpContext.Session.GetString("Rol") == null)
        {

            return RedirectToAction("Index", "Giris");
        }
        var sepet = HttpContext.Session.GetObject<List<SepetItem>>("sepet") ?? new List<SepetItem>();
        return View(sepet);
    }

    public IActionResult SiparisiTamamla()
    {
        return View();
    }




    [HttpPost]
    public IActionResult SiparisiTamamlaAjax(string adres)
    {
        var sepet = HttpContext.Session.GetObject<List<SepetItem>>("sepet");
        if (sepet == null || !sepet.Any())
            return BadRequest("Sepetiniz boş.");

        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 1;

        var siparis = new Sipari
        {
            KullaniciId = kullaniciId,
            OrderDate = DateTime.Now,
            DeliveryAddress = adres,
            TotalAmount = sepet.Sum(x => x.Fiyat * x.Miktar),
            OrderStatus = "Beklemede"
        };

        _context.Siparis.Add(siparis);
        _context.SaveChanges();

        foreach (var item in sepet)
        {
            _context.SiparisDetays.Add(new SiparisDetay
            {
                OrderId = siparis.OrderId,
                MenuItemId = item.MenuItemID,
                Miktar = item.Miktar,
                Fiyat = item.Fiyat
            });
        }

       
        var teslimat = new Teslimat
        {
            OrderId = siparis.OrderId,
            DeliveryStatus = "Beklemede",
            DeliveryTime = null,
            PersonelId = null
        };
        _context.Teslimats.Add(teslimat);

        _context.SaveChanges();
        HttpContext.Session.Remove("sepet");

        return Ok();
    }


    [HttpPost]
    public IActionResult TekrarSiparis(int orderId)
    {
        if (!MusteriMi()) return RedirectToAction("Yetkisiz", "Giris");
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;

        var siparis = _context.Siparis
            .Include(s => s.SiparisDetays).ThenInclude(d => d.MenuItem)
            .FirstOrDefault(s => s.OrderId == orderId && s.KullaniciId == kullaniciId);

        if (siparis == null) return NotFound();

        var sepet = new List<SepetItem>();
        foreach (var item in siparis.SiparisDetays.Where(d => d.MenuItem != null))
            sepet.Add(new SepetItem { MenuItemID = item.MenuItemId, ItemName = item.MenuItem!.ItemName, Fiyat = item.Fiyat ?? 0, Miktar = item.Miktar });

        HttpContext.Session.SetObject("sepet", sepet);
        return RedirectToAction("Sepet");
    }

    [HttpPost]
    public IActionResult IptalEt(int orderId)
    {
        if (!MusteriMi()) return RedirectToAction("Yetkisiz", "Giris");
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
        var siparis = _context.Siparis.FirstOrDefault(s => s.OrderId == orderId && s.KullaniciId == kullaniciId);
        if (siparis != null && siparis.OrderStatus == "Beklemede")
        {
            siparis.OrderStatus = "İptal Edildi";
            _context.SaveChanges();
        }
        return RedirectToAction("Siparislerim");
    }

    public IActionResult Detay(int id)
    {
        if (!MusteriMi()) return RedirectToAction("Yetkisiz", "Giris");
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
        var siparis = _context.Siparis
            .Include(s => s.Kullanici)
            .Include(s => s.SiparisDetays).ThenInclude(d => d.MenuItem).ThenInclude(m => m.Restaurant)
            .Include(s => s.Teslimats)
            .FirstOrDefault(s => s.OrderId == id && s.KullaniciId == kullaniciId);
        if (siparis == null) return NotFound();
        return View(siparis);
    }

    public IActionResult Siparislerim()

    {
        if (!MusteriMi())
        {
            return RedirectToAction("Yetkisiz", "Giris");
        }
        int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;

        var siparisler = _context.Siparis
            .Include(s => s.Teslimats)
            .Include(s => s.SiparisDetays)
                .ThenInclude(d => d.MenuItem)
            .Where(s => s.KullaniciId == kullaniciId)
            .OrderByDescending(s => s.OrderDate)
            .ToList();

        return View(siparisler);
    }





}