import os
import math
import itertools
import requests
import nltk
from nltk.sentiment.vader import SentimentIntensityAnalyzer
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List, Optional
import numpy as np
from scipy.sparse import csr_matrix
from scipy.sparse.linalg import svds

nltk.download("vader_lexicon", quiet=True)

app = FastAPI(title="YemekSiparis AI Service")

GOOGLE_MAPS_API_KEY = os.getenv("GOOGLE_MAPS_API_KEY", "")
sia = SentimentIntensityAnalyzer()

# Türkçe anahtar kelimeler (VADER İngilizce eğitildiği için)
TURKCE_POZITIF = {
    "harika", "mükemmel", "lezzetli", "güzel", "çok iyi", "süper", "nefis",
    "taze", "hızlı", "teşekkür", "beğendim", "tavsiye", "memnun", "muhteşem",
    "başarılı", "sıcak", "kaliteli", "şahane", "iyi", "sevdim", "tekrar",
    "muhtesem", "inanilmaz", "mükemmeldi", "harikaydı", "harikaydi", "begendim",
    "lezzetliydi", "tazeydi", "güzeldi", "tatli", "otantik", "tavsiye ederim",
    "kesinlikle", "en iyi", "deli gibi", "çok güzel", "bir daha", "gelirim",
    "ideal", "doğru adres", "dogru adres", "hak ediyor", "deneyimdi"
}
TURKCE_NEGATIF = {
    "berbat", "kötü", "soğuk", "geç", "beklettik", "sinir", "rezalet",
    "beğenmedim", "pişman", "hata", "yanlış", "eksik", "şikayet", "bozuk",
    "bayat", "çiğ", "korkunç", "mahvetti", "kesinlikle hayır",
    "soguk", "gec", "kuru", "hayal kirikligi", "beklentimin altinda",
    "onermiyorum", "önermiyorum", "küçük", "yetersiz", "uzun bekleme",
    "pismanlık", "olmaz", "çok geç", "çok kötü"
}

# Olumsuzlama (negasyon) kelimeleri: kendinden önceki pozitif kelimeyi tersine çevirir.
# Örn: "lezzetli değildi" -> negatif olarak sayılır.
NEGASYON = {
    "değil", "degil", "değildi", "degildi", "değildir", "degildir",
    "yok", "yoktu", "olmadı", "olmadi", "olmamış", "olmamis",
    "maalesef", "ne yazık", "ne yazik"
}


# ─────────────────────────────────────────────
# 1. DUYGU ANALİZİ (NLP)
# ─────────────────────────────────────────────

class DuyguIstegi(BaseModel):
    yorum_metni: str

@app.post("/duygu")
def duygu_analiz(istek: DuyguIstegi):
    skorlar = sia.polarity_scores(istek.yorum_metni)
    compound = skorlar["compound"]

    # VADER sıfır döndürdüyse Türkçe anahtar kelime kontrolü yap (negasyon farkındalıklı)
    if abs(compound) < 0.05:
        metin = istek.yorum_metni.lower()
        poz = 0
        neg = 0
        # Pozitif kelimeler: hemen ardından olumsuzlama gelirse negatife çevir
        for k in TURKCE_POZITIF:
            idx = metin.find(k)
            if idx >= 0:
                sonrasi = metin[idx + len(k): idx + len(k) + 22]  # kelimeden sonraki kısa pencere
                if any(ng in sonrasi for ng in NEGASYON):
                    neg += 1   # "lezzetli değildi" gibi
                else:
                    poz += 1
        # Negatif kelimeler
        for k in TURKCE_NEGATIF:
            if k in metin:
                neg += 1
        if poz > neg:
            compound = min(0.5 + (poz - neg) * 0.1, 1.0)
        elif neg > poz:
            compound = max(-0.5 - (neg - poz) * 0.1, -1.0)

    compound = round(compound, 4)
    if compound >= 0.05:
        etiket = "pozitif"
    elif compound <= -0.05:
        etiket = "negatif"
    else:
        etiket = "notr"

    return {"skor": compound, "etiket": etiket}


# ─────────────────────────────────────────────
# 2. ÖNERİ SİSTEMİ (Collaborative Filtering)
# ─────────────────────────────────────────────

class OneriVerisi(BaseModel):
    # kullanici_id -> [{"menu_item_id": int, "puan": float}, ...]
    kullanici_puanlari: dict
    hedef_kullanici_id: int
    limit: int = 10

@app.post("/oneri")
def oneri_getir(veri: OneriVerisi):
    puanlar = veri.kullanici_puanlari
    hedef = str(veri.hedef_kullanici_id)

    # Cold start: hedef kullanıcı yok ya da çok az veri
    tum_kullanicilar = list(puanlar.keys())
    if hedef not in tum_kullanicilar or len(tum_kullanicilar) < 3:
        populer = _populer_urunler(puanlar, veri.limit)
        return {"kullanici_id": veri.hedef_kullanici_id, "oneriler": populer, "kaynak": "populer"}

    # Tüm menu item id'lerini topla
    tum_urunler = sorted({item for u in puanlar.values() for item in u.keys()})
    kullanici_listesi = tum_kullanicilar
    k2i = {k: i for i, k in enumerate(kullanici_listesi)}
    u2i = {u: i for i, u in enumerate(tum_urunler)}

    # Rating matrix oluştur
    satir, sutun, deger = [], [], []
    for k, urunler in puanlar.items():
        for urun, puan in urunler.items():
            satir.append(k2i[k])
            sutun.append(u2i[urun])
            deger.append(float(puan))

    matris = csr_matrix((deger, (satir, sutun)), shape=(len(kullanici_listesi), len(tum_urunler)))

    # SVD
    k = min(10, min(matris.shape) - 1)
    if k < 1:
        populer = _populer_urunler(puanlar, veri.limit)
        return {"kullanici_id": veri.hedef_kullanici_id, "oneriler": populer, "kaynak": "populer"}

    U, sigma, Vt = svds(matris.astype(float), k=k)
    tahmin_matris = np.dot(np.dot(U, np.diag(sigma)), Vt)

    hedef_idx = k2i[hedef]
    hedef_puanlar = tahmin_matris[hedef_idx]

    # Zaten puanlananları çıkar
    gosterilmis = set(puanlar.get(hedef, {}).keys())
    sirali = sorted(
        [(tum_urunler[i], float(hedef_puanlar[i])) for i in range(len(tum_urunler)) if tum_urunler[i] not in gosterilmis],
        key=lambda x: x[1], reverse=True
    )

    oneriler = [{"menu_item_id": int(uid), "tahmini_puan": round(p, 2)} for uid, p in sirali[:veri.limit]]
    return {"kullanici_id": veri.hedef_kullanici_id, "oneriler": oneriler, "kaynak": "collaborative"}


def _populer_urunler(puanlar: dict, limit: int):
    sayac: dict = {}
    for urunler in puanlar.values():
        for urun, puan in urunler.items():
            if urun not in sayac:
                sayac[urun] = {"toplam": 0.0, "adet": 0}
            sayac[urun]["toplam"] += float(puan)
            sayac[urun]["adet"] += 1

    ortalama = [(uid, v["toplam"] / v["adet"]) for uid, v in sayac.items()]
    ortalama.sort(key=lambda x: x[1], reverse=True)
    return [{"menu_item_id": int(uid), "ort_puan": round(p, 2)} for uid, p in ortalama[:limit]]


# ─────────────────────────────────────────────
# 3. ROTA OPTİMİZASYONU (TSP)
# ─────────────────────────────────────────────

class TeslimatNoktasi(BaseModel):
    siparis_id: int
    enlem: float
    boylam: float

class RotaIstegi(BaseModel):
    baslangic_enlem: float
    baslangic_boylam: float
    teslimat_noktalari: List[TeslimatNoktasi]

@app.post("/rota")
def rota_optimize(istek: RotaIstegi):
    if not istek.teslimat_noktalari:
        raise HTTPException(status_code=400, detail="Teslimat noktası yok")

    baslangic = (istek.baslangic_enlem, istek.baslangic_boylam)
    noktalar = [(n.enlem, n.boylam) for n in istek.teslimat_noktalari]
    tum_noktalar = [baslangic] + noktalar

    # Google Maps API varsa kullan, yoksa Haversine ile fallback
    if GOOGLE_MAPS_API_KEY:
        mesafe_matrisi = _google_maps_matrisi(tum_noktalar)
    else:
        mesafe_matrisi = _haversine_matrisi(tum_noktalar)

    if mesafe_matrisi is None:
        mesafe_matrisi = _haversine_matrisi(tum_noktalar)

    # TSP: başlangıç hariç noktaların permütasyonları (küçük setler için exact)
    n = len(noktalar)
    if n <= 8:
        en_iyi_rota, en_kisa_mesafe = _tsp_exact(mesafe_matrisi, n)
    else:
        en_iyi_rota, en_kisa_mesafe = _tsp_nearest_neighbor(mesafe_matrisi, n)

    sirali_noktalar = [istek.teslimat_noktalari[i] for i in en_iyi_rota]
    tahmini_sure = int(en_kisa_mesafe / 400)  # ortalama 400m/dk şehir içi

    return {
        "optimized_route": [
            {"siparis_id": n.siparis_id, "enlem": n.enlem, "boylam": n.boylam, "sira": idx + 1}
            for idx, n in enumerate(sirali_noktalar)
        ],
        "toplam_mesafe_m": round(en_kisa_mesafe),
        "tahmini_sure_dk": tahmini_sure,
        "algoritma": "exact_tsp" if n <= 8 else "nearest_neighbor"
    }


def _google_maps_matrisi(noktalar: list) -> Optional[list]:
    try:
        koordinatlar = "|".join(f"{lat},{lng}" for lat, lng in noktalar)
        url = (
            f"https://maps.googleapis.com/maps/api/distancematrix/json"
            f"?origins={koordinatlar}&destinations={koordinatlar}"
            f"&mode=driving&key={GOOGLE_MAPS_API_KEY}"
        )
        resp = requests.get(url, timeout=5)
        data = resp.json()

        if data.get("status") != "OK":
            return None

        n = len(noktalar)
        matris = [[0.0] * n for _ in range(n)]
        for i, satir in enumerate(data["rows"]):
            for j, element in enumerate(satir["elements"]):
                if element["status"] == "OK":
                    matris[i][j] = float(element["distance"]["value"])
                else:
                    matris[i][j] = _haversine(noktalar[i], noktalar[j])
        return matris
    except Exception:
        return None


def _haversine_matrisi(noktalar: list) -> list:
    n = len(noktalar)
    return [[_haversine(noktalar[i], noktalar[j]) for j in range(n)] for i in range(n)]


def _haversine(p1: tuple, p2: tuple) -> float:
    R = 6371000
    lat1, lon1 = math.radians(p1[0]), math.radians(p1[1])
    lat2, lon2 = math.radians(p2[0]), math.radians(p2[1])
    dlat = lat2 - lat1
    dlon = lon2 - lon1
    a = math.sin(dlat / 2) ** 2 + math.cos(lat1) * math.cos(lat2) * math.sin(dlon / 2) ** 2
    return R * 2 * math.asin(math.sqrt(a))


def _tsp_exact(matris: list, n: int):
    en_iyi, en_kisa = None, float("inf")
    for perm in itertools.permutations(range(1, n + 1)):
        mesafe = matris[0][perm[0]]
        for i in range(len(perm) - 1):
            mesafe += matris[perm[i]][perm[i + 1]]
        if mesafe < en_kisa:
            en_kisa = mesafe
            en_iyi = perm
    return [i - 1 for i in en_iyi], en_kisa


def _tsp_nearest_neighbor(matris: list, n: int):
    ziyaret_edildi = [False] * (n + 1)
    ziyaret_edildi[0] = True
    rota = []
    simdiki = 0
    toplam = 0.0

    for _ in range(n):
        en_yakin, en_az = -1, float("inf")
        for j in range(1, n + 1):
            if not ziyaret_edildi[j] and matris[simdiki][j] < en_az:
                en_az = matris[simdiki][j]
                en_yakin = j
        rota.append(en_yakin)
        ziyaret_edildi[en_yakin] = True
        toplam += en_az
        simdiki = en_yakin

    return [i - 1 for i in rota], toplam


# ─────────────────────────────────────────────
# HEALTH CHECK
# ─────────────────────────────────────────────

@app.get("/health")
def health():
    return {"status": "ok"}
