# 🍔 Yapay Zeka Destekli Akıllı Yemek Sipariş ve Lojistik Yönetim Sistemi

İstanbul Ticaret Üniversitesi – Bilgisayar Mühendisliği **Bitirme Projesi (2026)**
**Yiğit Yeniçeri** · Danışman: Saadettin Aksoy

Geleneksel yemek sipariş süreçlerini; **kişiselleştirilmiş öneri**, **akıllı rota optimizasyonu** ve **müşteri yorumu duygu analizi** ile güçlendiren, mikroservis mimarisine sahip uçtan uca bir platform.

🌐 **Canlı sistem:** [siparis-web-yg.azurewebsites.net](https://siparis-web-yg.azurewebsites.net)

---

## 🧠 Yapay Zeka Modülleri

| Modül | Yöntem | Açıklama |
|-------|--------|----------|
| **Öneri Sistemi** | SVD tabanlı İşbirlikçi Filtreleme (Collaborative Filtering) | Kullanıcıya kişiye özel restoran/menü önerisi. Yeni kullanıcı için popülerlik bazlı *cold-start* yedeği. |
| **Rota Optimizasyonu** | Gezgin Satıcı Problemi (TSP) | ≤10 nokta için tam çözüm, fazlası için Nearest-Neighbor sezgiseli. Google Maps ile harita üzerinde rota. |
| **Duygu Analizi** | NLP – NLTK VADER + Türkçe leksikon | Müşteri yorumlarını Pozitif / Nötr / Negatif sınıflandırır (negasyon farkındalıklı). Admin panelinde 14 günlük trend. |

## 🏗️ Mimari

```
┌─────────────┐     HTTP/JSON     ┌──────────────────────┐
│  Web (C#)   │ ────────────────> │  AI Servisi (Python) │
│ ASP.NET Core│                   │  FastAPI             │
│  8.0 MVC    │ <──────────────── │  /duygu /oneri /rota │
└──────┬──────┘                   └──────────────────────┘
       │ EF Core
┌──────▼──────┐     ┌──────────────┐
│  Azure SQL  │     │   SignalR    │  ← gerçek zamanlı sipariş durumu
└─────────────┘     └──────────────┘
```

**Hibrit mikroservis** kararı: Yapay zeka işlemleri ana uygulamadan izole edilmiş bağımsız bir Python servisinde çalışır — AI servisi kesintiye uğrasa bile sipariş sistemi çalışmaya devam eder.

## 🛠️ Teknolojiler

- **Backend:** C# / ASP.NET Core 8.0 MVC, Entity Framework Core 8.0
- **Yapay Zeka:** Python, FastAPI, NLTK (VADER), scikit-learn (TruncatedSVD), NumPy/SciPy
- **Veritabanı:** Azure SQL Database (Serverless)
- **Gerçek Zamanlı:** SignalR (WebSocket)
- **Frontend:** Bootstrap 5, Chart.js, Leaflet / Google Maps API
- **DevOps:** Docker & Docker Compose, Azure App Service, Azure Container Registry
- **Güvenlik:** SHA-256 parola hashleme, session tabanlı kimlik doğrulama, HTTPS

## 👥 Kullanıcı Rolleri

- **Müşteri:** Restoran/menü listeleme, AI destekli öneri, sepet, sipariş, yorum & puan
- **Restoran:** Menü yönetimi, sipariş durumu güncelleme
- **Kurye:** Bekleyen teslimatlar, TSP optimize rota haritası, canlı konum
- **Yönetici:** Dashboard istatistikleri, NLP duygu trendi, CRUD yönetimi, CSV dışa aktarma

## 🚀 Yerel Kurulum (Docker)

```bash
git clone https://github.com/YigitYe/SiparisUygulama.git
cd SiparisUygulama
docker compose up -d
# Web: http://localhost:8080
```

> `docker-compose.yml` üç servisi ayağa kaldırır: `sqlserver`, `ai-service`, `web`.

## 📂 Proje Yapısı

```
SiparisUygulama/
├── SiparisUygulama/        # ASP.NET Core MVC (web)
│   ├── Controllers/        # Admin, Siparis, Personel, Chatbot, RestoranYonetim...
│   ├── Models/             # EF Core varlıkları
│   ├── Services/           # AiServiceClient (Python servisine HTTP)
│   ├── Hubs/               # SiparisHub (SignalR)
│   └── Views/
├── ai-service/             # Python FastAPI (NLP, TSP, SVD)
└── docker-compose.yml
```

---

📄 *Bu proje, İstanbul Ticaret Üniversitesi Bilgisayar Mühendisliği bitirme projesi kapsamında geliştirilmiştir.*
