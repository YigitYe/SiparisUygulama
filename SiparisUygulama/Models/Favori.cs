using System;

namespace SiparisUygulama.Models
{
    public class Favori
    {
        public int FavoriId { get; set; }
        public int KullaniciId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        public virtual Kullanici? Kullanici { get; set; }
        public virtual Restoran? Restoran { get; set; }
    }
}
