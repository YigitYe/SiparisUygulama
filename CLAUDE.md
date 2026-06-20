# Proje: Yapay Zeka Destekli Akıllı Yemek Sipariş ve Lojistik Yönetim Sistemi

## Tech Stack
- Backend: ASP.NET Core MVC 8.0 + C#
- AI Servisi: Python Flask/FastAPI (mikroservis, REST/JSON)
- Veritabanı: Azure SQL + Entity Framework Core
- DevOps: Docker + Azure App Service

## AI Modülleri
- TSP tabanlı Rota Optimizasyonu (Google Maps Distance Matrix API, <200ms hedef)
- NLP Duygu Analizi (NLTK, -1/+1 skor)
- Collaborative Filtering Öneri Sistemi (Cold Start → Popüler Ürünler fallback)

## Veritabanı Özel Alanlar
- Kullanici: VarsayilanEnlem, VarsayilanBoylam
- Restoran: Enlem, Boylam, MutfakTuru
- Personel: AnlikEnlem, AnlikBoylam, MusaitlikDurumu
- Teslimat: RotaBilgisi (JSON)
- Yorum: DuyguAnalizSkoru (Float)

## Paneller
- Müşteri: AI öneri modülü ana sayfada en üstte
- Kurye: TSP tetiklenince haritada A→B→C rotası
- Admin: NLP grafikli dashboard

## Mimari Karar
Monolitik → Hibrit Mikroservis (AI işlemleri izole Python servisinde)

## Güvenlik
SHA-256 hashing

## Mevcut Durum
- Proje net6.0 ile yazılmış, net8.0'a yükseltilecek
- Docker eklenecek
- Herkese açık yayına alınacak
