using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Sipari
    {
        public Sipari()
        {
            Odemes = new HashSet<Odeme>();
            SiparisDetays = new HashSet<SiparisDetay>();
            Teslimats = new HashSet<Teslimat>();
        }

        public int OrderId { get; set; }
        public int KullaniciId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? OrderStatus { get; set; }

        public virtual Kullanici Kullanici { get; set; } = null!;
        public virtual ICollection<Odeme> Odemes { get; set; }
        public virtual ICollection<SiparisDetay> SiparisDetays { get; set; }
        public virtual ICollection<Teslimat> Teslimats { get; set; }
    }
}
