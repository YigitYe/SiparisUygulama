using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SiparisUygulama.Controllers
{
    public class RestoranYonetimController : Controller
    {
        private readonly SiparisDbContext _context;
        public RestoranYonetimController(SiparisDbContext context)
        {
            _context = context;
        }
        private bool RestoranMi()
        {
            return HttpContext.Session.GetString("Rol") == "Restoran";
        }

        public IActionResult Index()
        {
            if (!RestoranMi())
            {
                return RedirectToAction("Yetkisiz", "Giris");
            }

            var restoranlar = _context.Restorans.ToList();

            ViewData["Title"] = "Restoranlar";
            ViewData["HeaderTitle"] = "Restoranlar";
            ViewData["HeaderSubtitle"] = "Şehrinizdeki En İyi Restoranları Keşfedin";

            return View(restoranlar);
        }

        [HttpPost]
        public IActionResult EkleAjax(MenuEkleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var yeniUrun = new Menu
                {
                    RestaurantId = model.RestaurantId,
                    ItemName = model.ItemName,
                    Aciklama = model.Aciklama,
                    Fiyat = model.Fiyat,
                    Kategori = model.Kategori
                };

                _context.Menus.Add(yeniUrun);
                _context.SaveChanges();
                return Ok();
            }

            var hatalar = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest("Geçersiz veri:\n" + string.Join("\n", hatalar));
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

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("Rol") == null)
                return RedirectToAction("Index", "Giris");

            var restoran = _context.Restorans
                .Include(r => r.Menus)
                .FirstOrDefault(r => r.RestaurantId == id);

            if (restoran == null)
                return NotFound();

            return View(restoran);
        }

        public IActionResult Siparisler()
        {
            if (!RestoranMi())
                return RedirectToAction("Yetkisiz", "Giris");

            var siparisler = _context.Siparis
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisDetays)
                    .ThenInclude(sd => sd.MenuItem)
                .OrderByDescending(s => s.OrderDate)
                .ToList();

            return View(siparisler);
        }

        [HttpPost]
        public IActionResult DurumGuncelle(int orderId, string yeniDurum)
        {
            if (!RestoranMi())
                return RedirectToAction("Yetkisiz", "Giris");

            var siparis = _context.Siparis.FirstOrDefault(s => s.OrderId == orderId);
            if (siparis == null)
                return NotFound();

            siparis.OrderStatus = yeniDurum;
            _context.SaveChanges();

            return RedirectToAction("Siparisler");
        }
    }
}

