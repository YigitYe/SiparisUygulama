using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Restoran
    {
        public Restoran()
        {
            Menus = new HashSet<Menu>();
            Yorums = new HashSet<Yorum>();
        }

        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = null!;
        public string? Adres { get; set; }
        public string? TelefonNumarasi { get; set; }
        public string? Email { get; set; }
        public double? Enlem { get; set; }
        public double? Boylam { get; set; }
        public string? MutfakTuru { get; set; }
        public string? GorselUrl { get; set; }
        public decimal? MinSiparisAmount { get; set; }
        public decimal? TeslimatUcreti { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Yorum> Yorums { get; set; }
    }
}
