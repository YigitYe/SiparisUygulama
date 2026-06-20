using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class SiparisDetay
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Miktar { get; set; }
        public decimal? Fiyat { get; set; }

        public virtual Menu MenuItem { get; set; } = null!;
        public virtual Sipari Order { get; set; } = null!;
    }
}
