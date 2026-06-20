using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Personel
    {
        public Personel()
        {
            Teslimats = new HashSet<Teslimat>();
        }

        public int PersonelId { get; set; }
        public string? Adi { get; set; }
        public string? IletisimNumarasi { get; set; }
        public string? TeslimatAlani { get; set; }
        public double? AnlikEnlem { get; set; }
        public double? AnlikBoylam { get; set; }
        public bool MusaitlikDurumu { get; set; } = true;
        public string? Sifre { get; set; }

        public virtual ICollection<Teslimat> Teslimats { get; set; }
    }
}
