using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SiparisUygulama.Services
{
    public class AiServiceClient
    {
        private readonly HttpClient _http;

        // Python AI servisi snake_case (optimized_route, menu_item_id, ...) döndürür;
        // C# PascalCase özellikleriyle eşleştirmek için snake_case okuma seçenekleri.
        private static readonly JsonSerializerOptions _snake = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };

        public AiServiceClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<float?> DuyguAnalizAsync(string yorumMetni)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/duygu", new { yorum_metni = yorumMetni });
                if (!response.IsSuccessStatusCode) return null;
                var sonuc = await response.Content.ReadFromJsonAsync<DuyguSonuc>(_snake);
                return sonuc?.Skor;
            }
            catch
            {
                return null;
            }
        }

        public async Task<RotaSonuc?> RotaOptimizeAsync(double basEnlem, double basBoylam, List<TeslimatNokta> noktalar)
        {
            try
            {
                var istek = new
                {
                    baslangic_enlem = basEnlem,
                    baslangic_boylam = basBoylam,
                    teslimat_noktalari = noktalar
                };
                var response = await _http.PostAsJsonAsync("/rota", istek);
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<RotaSonuc>(_snake);
            }
            catch
            {
                return null;
            }
        }

        public async Task<OneriSonuc?> OneriGetirAsync(int kullaniciId, Dictionary<string, Dictionary<string, float>> puanlar, int limit = 10)
        {
            try
            {
                var istek = new
                {
                    kullanici_puanlari = puanlar,
                    hedef_kullanici_id = kullaniciId,
                    limit
                };
                var response = await _http.PostAsJsonAsync("/oneri", istek);
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<OneriSonuc>(_snake);
            }
            catch
            {
                return null;
            }
        }

        // DTO'lar
        public record DuyguSonuc(float Skor, string Etiket);

        public class TeslimatNokta
        {
            [JsonPropertyName("siparis_id")]
            public int SiparisId { get; set; }
            [JsonPropertyName("enlem")]
            public double Enlem { get; set; }
            [JsonPropertyName("boylam")]
            public double Boylam { get; set; }
        }

        public class RotaSonuc
        {
            public List<RotaNoktasi>? OptimizedRoute { get; set; }
            public int ToplamMesafeM { get; set; }
            public int TahminiSureDk { get; set; }
        }

        public class RotaNoktasi
        {
            public int SiparisId { get; set; }
            public double Enlem { get; set; }
            public double Boylam { get; set; }
            public int Sira { get; set; }
        }

        public class OneriSonuc
        {
            public int KullaniciId { get; set; }
            public List<OneriItem>? Oneriler { get; set; }
            public string Kaynak { get; set; } = "populer";
        }

        public class OneriItem
        {
            public int MenuItemId { get; set; }
            public float TahminiPuan { get; set; }
            public float OrtPuan { get; set; }
        }
    }
}
