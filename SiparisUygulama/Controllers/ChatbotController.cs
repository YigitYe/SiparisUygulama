using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Models;
using System.Text;
using System.Text.Json;

namespace SiparisUygulama.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly SiparisDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatbotController(SiparisDbContext context, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Mesaj([FromBody] ChatMesajRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Mesaj))
                return BadRequest("Mesaj boş olamaz.");

            int kullaniciId = HttpContext.Session.GetInt32("KullaniciID") ?? 0;
            string kullaniciAdi = HttpContext.Session.GetString("KullaniciAdi") ?? "Misafir";
            var mesajLower = request.Mesaj.ToLower(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));

            // Sipariş durumu sorgusu
            if (kullaniciId > 0 && (mesajLower.Contains("sipariş") || mesajLower.Contains("nerede") ||
                mesajLower.Contains("durumu") || mesajLower.Contains("son sipariş") || mesajLower.Contains("geçmiş")))
            {
                var sonSiparisler = _context.Siparis
                    .Where(s => s.KullaniciId == kullaniciId)
                    .OrderByDescending(s => s.OrderDate)
                    .Take(5)
                    .ToList();

                if (sonSiparisler.Any())
                {
                    var sb = new StringBuilder("Son siparişleriniz:\n\n");
                    foreach (var s in sonSiparisler)
                    {
                        var ikon = s.OrderStatus switch
                        {
                            "Teslim Edildi" => "✅",
                            "Yolda" => "🛵",
                            "Hazır" or "Hazir" => "⏳",
                            _ => "📦"
                        };
                        sb.AppendLine($"{ikon} **Sipariş #{s.OrderId}** — {s.OrderStatus} — ₺{s.TotalAmount} ({s.OrderDate:dd MMM HH:mm})");
                    }
                    return Ok(new { cevap = sb.ToString() });
                }
                return Ok(new { cevap = "Henüz siparişiniz bulunmuyor. Menüden bir şeyler seçip sipariş verebilirsiniz! 🍽️" });
        }

            // Yardım/selamlama
            if (mesajLower is "merhaba" or "selam" or "hey" or "hello" or "hi")
                return Ok(new { cevap = $"Merhaba {kullaniciAdi}! 👋 Size nasıl yardımcı olabilirim?\n\n• 🍕 Yemek öneri ister misiniz?\n• 📦 Siparişlerinizi sorabilirsiniz\n• 💰 Bütçenize göre seçenekler önerebilirim" });

            var menuler = _context.Menus
                .Include(m => m.Restaurant)
                .Select(m => new MenuBilgi
                {
                    ItemName = m.ItemName,
                    Aciklama = m.Aciklama ?? "",
                    Kategori = m.Kategori ?? "",
                    Fiyat = m.Fiyat,
                    RestoranAdi = m.Restaurant.RestaurantName,
                    MenuItemId = m.MenuItemId,
                    RestaurantId = m.RestaurantId
                })
                .ToList();

            var apiKey = _config["Anthropic:ApiKey"] ?? Environment.GetEnvironmentVariable("ANTHROPIC__APIKEY") ?? "";

            if (!string.IsNullOrEmpty(apiKey))
            {
                var menuListesi = string.Join("\n", menuler.Select(m =>
                    $"- {m.ItemName} [{m.Kategori}] ₺{m.Fiyat} — {m.RestoranAdi}" +
                    (string.IsNullOrEmpty(m.Aciklama) ? "" : $": {m.Aciklama}")));

                string userContext = "";
                if (kullaniciId > 0)
                {
                    try
                    {
                        var favRestoran = _context.SiparisDetays
                            .Where(sd => sd.Order.KullaniciId == kullaniciId && sd.MenuItem != null)
                            .GroupBy(sd => sd.MenuItem!.Restaurant.RestaurantName)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .FirstOrDefault();
                        if (favRestoran != null)
                            userContext = $"\nKullanıcının favori restoranı: {favRestoran}.";
                    }
                    catch { }
                }

                var systemPrompt = $"Sen samimi ve yardımsever bir yemek sipariş asistanısın. " +
                    $"Kullanıcının adı: {kullaniciAdi}.{userContext} " +
                    "Türkçe konuş, kısa ve net cevap ver (max 3-4 cümle). " +
                    "Sadece aşağıdaki menüdeki ürünleri öner, başka restoran/ürün uydurma. " +
                    "Fiyat ve restoran adını belirt. Emoji kullanabilirsin.\n\nMenü:\n" + menuListesi;

                var cevap = await AnthropicCagir(apiKey, systemPrompt, request.Mesaj, request.Gecmis);
                return Ok(new { cevap });
            }

            return Ok(new { cevap = KeywordTabanliOneri(request.Mesaj, menuler) });
        }

        private async Task<string> AnthropicCagir(string apiKey, string systemPrompt, string userMessage, List<ChatGecmisItem>? gecmis)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                // Geçmişi dahil et (max son 8 mesaj)
                var messages = new List<object>();
                if (gecmis != null)
                {
                    foreach (var g in gecmis.TakeLast(8))
                        messages.Add(new { role = g.Rol == "bot" ? "assistant" : "user", content = g.Icerik });
                }
                messages.Add(new { role = "user", content = userMessage });

                var body = new
                {
                    model = "claude-haiku-4-5-20251001",
                    max_tokens = 400,
                    system = systemPrompt,
                    messages
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);

                if (!response.IsSuccessStatusCode)
                    return "Şu an öneri servisi müsait değil. Menüye göz atabilirsiniz!";

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);
                return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString()
                    ?? "Bir şeyler ters gitti.";
            }
            catch
            {
                return "Şu an öneri servisi müsait değil. Menüye göz atabilirsiniz!";
            }
        }

        private string KeywordTabanliOneri(string mesaj, List<MenuBilgi> menuler)
        {
            mesaj = mesaj.ToLower();

            bool ucuzMu = mesaj.Contains("ucuz") || mesaj.Contains("ekonomik") || mesaj.Contains("uygun") || mesaj.Contains("bütçe");
            bool pahaliMi = mesaj.Contains("pahalı") || mesaj.Contains("özel") || mesaj.Contains("lüks") || mesaj.Contains("en iyi");
            bool hizliMi = mesaj.Contains("hızlı") || mesaj.Contains("acil") || mesaj.Contains("az");
            bool vejetaryenMi = mesaj.Contains("vejetaryen") || mesaj.Contains("vegan") || mesaj.Contains("sebze");

            var kelimeler = new[]
            {
                "pizza", "burger", "hamburger", "tavuk", "et", "salata", "çorba",
                "tatlı", "döner", "kebap", "balık", "deniz", "kahvaltı", "sandviç",
                "sushi", "japon", "çin", "makarna", "mantı", "köfte", "izgara"
            };

            var eslesen = menuler.Where(m =>
            {
                var metin = (m.ItemName + " " + m.Aciklama + " " + m.Kategori + " " + m.RestoranAdi).ToLower();
                if (vejetaryenMi) return metin.Contains("vegan") || metin.Contains("vejetaryen") || metin.Contains("sebze") || metin.Contains("salata");
                return kelimeler.Any(k => mesaj.Contains(k) && metin.Contains(k));
            }).ToList();

            if (ucuzMu)
                eslesen = (eslesen.Any() ? eslesen : menuler).OrderBy(m => m.Fiyat).Take(5).ToList();
            else if (pahaliMi)
                eslesen = (eslesen.Any() ? eslesen : menuler).OrderByDescending(m => m.Fiyat).Take(4).ToList();
            else if (hizliMi)
                eslesen = (eslesen.Any() ? eslesen : menuler).OrderBy(m => m.Fiyat).Take(3).ToList();
            else if (!eslesen.Any())
                eslesen = menuler.OrderBy(_ => Guid.NewGuid()).Take(4).ToList();
            else
                eslesen = eslesen.Take(4).ToList();

            if (!eslesen.Any())
                return "Mevcut menüde uygun seçenek bulamadım. Menüye göz atabilirsiniz! 👀";

            // Sonuçları RESTORANA göre grupla — kullanıcı hangi restorandan sipariş
            // verebileceğini ve o restoranın ilgili ürünlerini bir arada görsün.
            var gruplu = eslesen
                .GroupBy(m => new { m.RestaurantId, m.RestoranAdi })
                .ToList();

            var sb = new StringBuilder();
            if (gruplu.Count == 1)
                sb.AppendLine($"**{gruplu[0].Key.RestoranAdi}** bunun için harika bir seçim! 🍽️\n");
            else
                sb.AppendLine($"Bunu sunan **{gruplu.Count} restoran** buldum: 🍽️\n");

            foreach (var g in gruplu.Take(4))
            {
                sb.AppendLine($"🏪 **{g.Key.RestoranAdi}**");
                foreach (var m in g.Take(3))
                    sb.AppendLine($"   • {m.ItemName} — ₺{m.Fiyat}");
                sb.AppendLine($"   👉 [Restorana git](/Restoran/Details/{g.Key.RestaurantId})");
                sb.AppendLine();
            }

            if (ucuzMu) sb.Append("💡 En uygun fiyatlı seçenekleri listeledim!");
            else if (pahaliMi) sb.Append("⭐ En özel seçenekleri listeledim!");

            return sb.ToString().TrimEnd();
        }

        private class MenuBilgi
        {
            public string ItemName { get; set; } = "";
            public string Aciklama { get; set; } = "";
            public string Kategori { get; set; } = "";
            public decimal Fiyat { get; set; }
            public string RestoranAdi { get; set; } = "";
            public int MenuItemId { get; set; }
            public int RestaurantId { get; set; }
        }
    }

    public class ChatMesajRequest
    {
        public string? Mesaj { get; set; }
        public List<ChatGecmisItem>? Gecmis { get; set; }
    }

    public class ChatGecmisItem
    {
        public string Rol { get; set; } = "";   // "user" veya "bot"
        public string Icerik { get; set; } = "";
    }
}
