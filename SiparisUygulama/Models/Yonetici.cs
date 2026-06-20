using System;
using System.Collections.Generic;

namespace SiparisUygulama.Models
{
    public partial class Yonetici
    {
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; } = null!;
        public string Sifre { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}
