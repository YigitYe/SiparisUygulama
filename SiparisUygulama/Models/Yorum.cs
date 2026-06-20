using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Yorum
    {
        public int ReviewId { get; set; }
        public int KullaniciId { get; set; }
        public int RestaurantId { get; set; }
        public int? Puan { get; set; }
        public string? Yorum1 { get; set; }
        public DateTime? ReviewDate { get; set; }
        public float? DuyguAnalizSkoru { get; set; }

        public virtual Kullanici Kullanici { get; set; } = null!;
        public virtual Restoran Restaurant { get; set; } = null!;
    }
}
