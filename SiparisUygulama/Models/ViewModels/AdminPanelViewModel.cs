using System.Collections.Generic;
using SiparisUygulama.Models;

namespace SiparisUygulama.Models.ViewModels
{
    public class AdminPanelViewModel
    {
        public List<Restoran> Restorans { get; set; } = new();
        public List<Personel> Personeller { get; set; } = new();
        public List<Kullanici> Kullanicilar { get; set; } = new();
        public List<Yonetici> Yoneticiler { get; set; } = new();
        public List<Sipari> Siparisler { get; set; } = new();

        // İstatistikler
        public int ToplamSiparis { get; set; }
        public decimal ToplamGelir { get; set; }
        public int ToplamKullanici { get; set; }
        public int AktifPersonel { get; set; }

        // NLP Duygu İstatistikleri
        public int PozitifYorum { get; set; }
        public int NegatiYorum { get; set; }
        public int NotorYorum { get; set; }

        // Grafik verisi: son 7 günün sipariş sayıları
        public List<string> GrafikEtiketler { get; set; } = new();
        public List<int> GrafikDegerler { get; set; } = new();

        // Gelir grafiği: son 7 günün geliri
        public List<decimal> GelirDegerleri { get; set; } = new();

        // Restoran bazlı sipariş sayısı
        public List<string> RestoranGrafikEtiketler { get; set; } = new();
        public List<int> RestoranGrafikDegerler { get; set; } = new();

        // NLP trend: son 14 günün ortalama duygu skoru
        public List<string> NlpTrendEtiketler { get; set; } = new();
        public List<double> NlpTrendDegerler { get; set; } = new();
    }
}
