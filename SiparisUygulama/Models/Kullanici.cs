using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Kullanici
    {
        public Kullanici()
        {
            Siparis = new HashSet<Sipari>();
            Yorums = new HashSet<Yorum>();
        }

        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = null!;
        public string Sifre { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public string? TeslimatAdresi { get; set; }
        public double? VarsayilanEnlem { get; set; }
        public double? VarsayilanBoylam { get; set; }
        public string? Avatar { get; set; }

        public virtual ICollection<Sipari> Siparis { get; set; }
        public virtual ICollection<Yorum> Yorums { get; set; }
    }
}
