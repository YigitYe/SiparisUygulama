using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SiparisUygulama.Models
{
    public partial class Menu
    {
        public Menu()
        {
            SiparisDetays = new HashSet<SiparisDetay>();
        }

        public int MenuItemId { get; set; }
        public int RestaurantId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public string? Kategori { get; set; }
        public string? GorselUrl { get; set; }

        public virtual Restoran Restaurant { get; set; } = null!;
        public virtual ICollection<SiparisDetay> SiparisDetays { get; set; }
    }
}
