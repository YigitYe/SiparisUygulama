using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Teslimat
    {
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public int? PersonelId { get; set; }
        public string? DeliveryStatus { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string? RotaBilgisi { get; set; }

        public virtual Sipari Order { get; set; } = null!;
        public virtual Personel? Personel { get; set; } = null!;
    }
}
