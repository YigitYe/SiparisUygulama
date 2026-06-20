using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Odeme
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentMethod { get; set; }

        public virtual Sipari Order { get; set; } = null!;
    }
}
