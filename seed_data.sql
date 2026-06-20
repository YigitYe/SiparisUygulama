EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

DELETE FROM Yorum;
DELETE FROM Teslimat;
DELETE FROM Odeme;
DELETE FROM SiparisDetay;
DELETE FROM Siparis;
DELETE FROM Menu;
DELETE FROM Personel;
DELETE FROM Kullanici;
DELETE FROM Restoran;
DELETE FROM Yonetici;

-- RESTORAN (10 adet)
SET IDENTITY_INSERT Restoran ON;
INSERT INTO Restoran (RestaurantID, RestaurantName, Adres, TelefonNumarasi, Email, Enlem, Boylam, MutfakTuru) VALUES
(1,  'Lezzet Durağı',      'İstanbul, Beşiktaş',       '02123456789', 'lezzet@restaurant.com',   41.0422, 29.0094, 'Türk'),
(2,  'Anadolu Sofrası',    'Ankara, Keçiören',          '03122334454', 'anadolu@sofra.com',        39.9334, 32.8597, 'Türk'),
(3,  'Pizza Roma',         'İstanbul, Kadıköy',         '02164445566', 'info@pizzaroma.com',       40.9906, 29.0262, 'İtalyan'),
(4,  'Grön',               'İstanbul, Caddebostan',     '02124151516', 'gron@restoran.com',        40.9556, 29.0653, 'Vejetaryen'),
(5,  'Sushi Tokyo',        'İstanbul, Nişantaşı',       '02122334455', 'info@sushitokyo.com',      41.0476, 28.9939, 'Japon'),
(6,  'Burger Bros',        'İstanbul, Şişli',           '02122112233', 'info@burgerbros.com',      41.0610, 28.9878, 'Amerikan'),
(7,  'Deniz Balık',        'İzmir, Karşıyaka',          '02324567890', 'info@denizbalik.com',      38.4610, 27.1046, 'Deniz Ürünleri'),
(8,  'Çin Bahçesi',        'Ankara, Çankaya',           '03124441234', 'info@cinbahcesi.com',      39.9030, 32.8597, 'Çin'),
(9,  'Kahvaltı Evi',       'İstanbul, Moda',            '02163334455', 'info@kahvaltievi.com',     40.9872, 29.0306, 'Kahvaltı'),
(10, 'Köfte Dünyası',      'İstanbul, Fatih',           '02125556677', 'info@koftedünyasi.com',    41.0138, 28.9380, 'Türk');
SET IDENTITY_INSERT Restoran OFF;

-- MENU (her restoran için farklı)
SET IDENTITY_INSERT Menu ON;
INSERT INTO Menu (MenuItemID, RestaurantID, ItemName, Aciklama, Fiyat, Kategori) VALUES
-- Lezzet Durağı
(1,  1, 'Adana Kebap',      'Acılı el yapımı kebap',           120.00, 'Et'),
(2,  1, 'Lahmacun',         'İnce hamurlu kıymalı',             30.00, 'Hamur'),
(3,  1, 'İskender',         'Tereyağlı yoğurtlu döner',        150.00, 'Et'),
(4,  1, 'Mercimek Çorbası', 'Ev yapımı',                        35.00, 'Çorba'),
-- Anadolu Sofrası
(5,  2, 'Mantı',            'Kayseri mantısı yoğurtlu',        110.00, 'Hamur'),
(6,  2, 'Kuru Fasulye',     'Ev yapımı kuru fasulye',           50.00, 'Zeytinyağlı'),
(7,  2, 'Pilav',            'Tereyağlı pirinç pilavı',          40.00, 'Yan Ürün'),
(8,  2, 'Gözleme',          'Peynirli ıspanaklı',               55.00, 'Hamur'),
-- Pizza Roma
(9,  3, 'Margarita',        'Domates sos mozzarella',          110.00, 'Pizza'),
(10, 3, 'Pepperoni',        'Bol pepperonili',                 130.00, 'Pizza'),
(11, 3, 'Dört Peynir',      'Parmesan gouda cheddar',          140.00, 'Pizza'),
(12, 3, 'Sezar Salata',     'Kruton parmesan',                  75.00, 'Salata'),
-- Grön
(13, 4, 'Avokado Toast',    'Tam tahıl ekmek avokado',          90.00, 'Vegan'),
(14, 4, 'Kinoa Kasesi',     'Sebzeli kinoa tahıl',              95.00, 'Vegan'),
(15, 4, 'Smoothie Bowl',    'Meyve granola',                    80.00, 'Vegan'),
-- Sushi Tokyo
(16, 5, 'Salmon Nigiri',    '6 parça somon',                   120.00, 'Sushi'),
(17, 5, 'Dragon Roll',      'Avokado karides',                 160.00, 'Maki'),
(18, 5, 'Miso Çorba',       'Geleneksel Japon',                 45.00, 'Çorba'),
-- Burger Bros
(19, 6, 'Classic Burger',   'Dana köfte cheddar',              110.00, 'Burger'),
(20, 6, 'BBQ Burger',       'Barbekü sos karamelize soğan',    130.00, 'Burger'),
(21, 6, 'Patates Kızartma', 'Çıtır patates',                    45.00, 'Yan Ürün'),
-- Deniz Balık
(22, 7, 'Levrek Izgara',    'Taze levrek',                     200.00, 'Balık'),
(23, 7, 'Karides Güveç',    'Domates soslu karides',           180.00, 'Deniz'),
(24, 7, 'Balık Çorbası',    'Taze deniz ürünleri',              70.00, 'Çorba'),
-- Çin Bahçesi
(25, 8, 'Kung Pao Tavuk',   'Baharatlı tavuk fıstık',          115.00, 'Ana Yemek'),
(26, 8, 'Dim Sum',          '8 parça buharda',                  90.00, 'Başlangıç'),
(27, 8, 'Chow Mein',        'Sebzeli erişte',                  100.00, 'Erişte'),
-- Kahvaltı Evi
(28, 9, 'Serpme Kahvaltı',  'Tam Türk kahvaltısı',             180.00, 'Kahvaltı'),
(29, 9, 'Menemen',          'Domates biber yumurta',            70.00, 'Kahvaltı'),
(30, 9, 'Simit Tabağı',     'Simit peynir zeytin',              55.00, 'Kahvaltı'),
-- Köfte Dünyası
(31, 10, 'Izgara Köfte',    'El yapımı dana köfte',            100.00, 'Et'),
(32, 10, 'Çiğ Köfte Dürüm', 'Acılı tatlı nar ekşili',          45.00, 'Dürüm'),
(33, 10, 'Ayran',           'Ev yapımı ayran',                  20.00, 'İçecek');
SET IDENTITY_INSERT Menu OFF;

-- KULLANİCİ (30 adet)
SET IDENTITY_INSERT Kullanici ON;
INSERT INTO Kullanici (KullaniciID, KullaniciAdi, Sifre, Email, Telefon, TeslimatAdresi, VarsayilanEnlem, VarsayilanBoylam) VALUES
(1,  'ahmet123',    'sifre123',      'ahmet@mail.com',    '05331234567', 'İstanbul, Kadıköy',        40.9933, 29.0302),
(2,  'elifg',       'guvenliSifre',  'elif@mail.com',     '05349876543', 'Ankara, Çankaya',          39.9030, 32.8597),
(3,  'mehmet_34',   '123456',        'mehmet@mail.com',   '05441239876', 'İzmir, Bornova',           38.4681, 27.2197),
(4,  'ayse_k',      'ayse1234',      'ayse@mail.com',     '05321112233', 'İstanbul, Beşiktaş',      41.0422, 29.0094),
(5,  'mustafa_y',   'pass123',       'mustafa@mail.com',  '05334445566', 'İstanbul, Şişli',         41.0610, 28.9878),
(6,  'zeynep_t',    'zeynep456',     'zeynep@mail.com',   '05367778899', 'Ankara, Keçiören',        39.9334, 32.8597),
(7,  'ali_can',     'ali789',        'ali@mail.com',      '05389990011', 'İstanbul, Fatih',         41.0138, 28.9380),
(8,  'selin_d',     'selin321',      'selin@mail.com',    '05312223344', 'İstanbul, Bakırköy',      40.9833, 28.8719),
(9,  'emre_b',      'emre654',       'emre@mail.com',     '05345556677', 'Ankara, Etimesgut',       39.9455, 32.6789),
(10, 'pinar_a',     'pinar987',      'pinar@mail.com',    '05378889900', 'İzmir, Karşıyaka',        38.4610, 27.1046),
(11, 'burak_s',     'burak111',      'burak@mail.com',    '05391112233', 'İstanbul, Ümraniye',      41.0167, 29.1167),
(12, 'dilan_m',     'dilan222',      'dilan@mail.com',    '05323334455', 'İstanbul, Üsküdar',       41.0233, 29.0150),
(13, 'can_oz',      'can333',        'can@mail.com',      '05356667788', 'Ankara, Mamak',           39.9408, 32.9237),
(14, 'merve_y',     'merve444',      'merve@mail.com',    '05389990022', 'İstanbul, Sarıyer',       41.1667, 29.0500),
(15, 'enes_k',      'enes555',       'enes@mail.com',     '05312221133', 'İstanbul, Pendik',        40.8756, 29.2342),
(16, 'buse_c',      'buse666',       'buse@mail.com',     '05345552266', 'İzmir, Alsancak',         38.4377, 27.1444),
(17, 'furkan_t',    'furkan777',     'furkan@mail.com',   '05378883399', 'Ankara, Sincan',          39.9753, 32.5836),
(18, 'irem_b',      'irem888',       'irem@mail.com',     '05391114455', 'İstanbul, Maltepe',       40.9286, 29.1311),
(19, 'oguz_d',      'oguz999',       'oguz@mail.com',     '05324445566', 'İstanbul, Ataşehir',      40.9833, 29.1167),
(20, 'elif_n',      'elif000',       'elifn@mail.com',    '05357776677', 'Ankara, Yenimahalle',     39.9667, 32.7833),
(21, 'sercan_a',    'sercan11',      'sercan@mail.com',   '05381118899', 'İstanbul, Beyoğlu',       41.0333, 28.9833),
(22, 'tugba_k',     'tugba22',       'tugba@mail.com',    '05314440011', 'İzmir, Buca',             38.3833, 27.1667),
(23, 'tolga_m',     'tolga33',       'tolga@mail.com',    '05347772233', 'İstanbul, Avcılar',       40.9795, 28.7218),
(24, 'simge_y',     'simge44',       'simge@mail.com',    '05370003344', 'Ankara, Gölbaşı',         39.7919, 32.8097),
(25, 'kaan_b',      'kaan55',        'kaan@mail.com',     '05392225566', 'İstanbul, Tuzla',         40.8167, 29.3000),
(26, 'hilal_s',     'hilal66',       'hilal@mail.com',    '05325557788', 'İstanbul, Eyüpsultan',    41.0833, 28.9167),
(27, 'berk_o',      'berk77',        'berk@mail.com',     '05358889900', 'İzmir, Konak',            38.4192, 27.1287),
(28, 'nazli_c',     'nazli88',       'nazli@mail.com',    '05381110022', 'Ankara, Altındağ',        39.9564, 32.8897),
(29, 'umut_d',      'umut99',        'umut@mail.com',     '05313332244', 'İstanbul, Gaziosmanpaşa', 41.0667, 28.9167),
(30, 'gizem_k',     'gizem00',       'gizem@mail.com',    '05346664466', 'İstanbul, Kartal',        40.8897, 29.1897);
SET IDENTITY_INSERT Kullanici OFF;

-- PERSONEL
SET IDENTITY_INSERT Personel ON;
INSERT INTO Personel (PersonelID, Adi, IletisimNumarasi, TeslimatAlani, MusaitlikDurumu, AnlikEnlem, AnlikBoylam) VALUES
(1, 'Ali Kaya',       '05332221111', 'Kadıköy',   1, 40.9933, 29.0302),
(2, 'Zeynep Yılmaz',  '05443334444', 'Çankaya',   1, 39.9030, 32.8597),
(3, 'Mustafa Tozan',  '04531234562', 'Maltepe',   1, 40.9286, 29.1311),
(4, 'Fatma Demir',    '05367778888', 'Şişli',     1, 41.0610, 28.9878),
(5, 'Hasan Çelik',    '05389991111', 'Fatih',     1, 41.0138, 28.9380);
SET IDENTITY_INSERT Personel OFF;

-- YONETİCİ
SET IDENTITY_INSERT Yonetici ON;
INSERT INTO Yonetici (KullaniciID, KullaniciAdi, Sifre, Rol) VALUES
(1, 'admin1',   '1234', 'Admin'),
(2, 'mehmet',   '1234', 'Musteri'),
(3, 'kebapci',  '1234', 'Restoran'),
(4, 'kurye01',  '1234', 'Personel');
SET IDENTITY_INSERT Yonetici OFF;

-- SİPARİŞ (25 adet)
SET IDENTITY_INSERT Siparis ON;
INSERT INTO Siparis (OrderID, KullaniciID, OrderDate, DeliveryAddress, TotalAmount, OrderStatus) VALUES
(1,  1,  '2025-07-25 12:00:00', 'İstanbul, Kadıköy',        150.00, 'Teslim Edildi'),
(2,  2,  '2025-07-25 13:00:00', 'Ankara, Çankaya',          160.00, 'Teslim Edildi'),
(3,  3,  '2025-07-26 11:00:00', 'İzmir, Bornova',           200.00, 'Teslim Edildi'),
(4,  4,  '2025-07-26 14:00:00', 'İstanbul, Beşiktaş',       110.00, 'Teslim Edildi'),
(5,  5,  '2025-07-27 12:30:00', 'İstanbul, Şişli',          175.00, 'Teslim Edildi'),
(6,  6,  '2025-07-27 13:30:00', 'Ankara, Keçiören',         110.00, 'Teslim Edildi'),
(7,  7,  '2025-07-28 11:00:00', 'İstanbul, Fatih',          145.00, 'Teslim Edildi'),
(8,  8,  '2025-07-28 14:00:00', 'İstanbul, Bakırköy',       240.00, 'Teslim Edildi'),
(9,  9,  '2025-07-29 10:00:00', 'Ankara, Etimesgut',        115.00, 'Hazır'),
(10, 10, '2025-07-29 11:00:00', 'İzmir, Karşıyaka',         200.00, 'Hazır'),
(11, 11, '2025-07-29 12:00:00', 'İstanbul, Ümraniye',       130.00, 'Hazır'),
(12, 12, '2025-07-30 09:00:00', 'İstanbul, Üsküdar',        180.00, 'Hazır'),
(13, 13, '2025-07-30 10:30:00', 'Ankara, Mamak',            100.00, 'Hazır'),
(14, 14, '2025-07-30 11:00:00', 'İstanbul, Sarıyer',        160.00, 'Hazır'),
(15, 15, '2025-07-30 12:00:00', 'İstanbul, Pendik',         110.00, 'Hazır'),
(16, 1,  '2025-08-01 13:00:00', 'İstanbul, Kadıköy',        280.00, 'Teslim Edildi'),
(17, 3,  '2025-08-01 14:00:00', 'İzmir, Bornova',            90.00, 'Teslim Edildi'),
(18, 5,  '2025-08-02 11:00:00', 'İstanbul, Şişli',          155.00, 'Hazır'),
(19, 7,  '2025-08-02 12:00:00', 'İstanbul, Fatih',          120.00, 'Hazır'),
(20, 2,  '2025-08-03 10:00:00', 'Ankara, Çankaya',          200.00, 'Hazır'),
(21, 16, '2025-08-03 13:00:00', 'İzmir, Alsancak',          130.00, 'Hazır'),
(22, 17, '2025-08-04 11:00:00', 'Ankara, Sincan',           115.00, 'Hazır'),
(23, 18, '2025-08-04 14:00:00', 'İstanbul, Maltepe',        180.00, 'Hazır'),
(24, 19, '2025-08-05 10:00:00', 'İstanbul, Ataşehir',       110.00, 'Hazır'),
(25, 20, '2025-08-05 12:00:00', 'Ankara, Yenimahalle',      200.00, 'Hazır');
SET IDENTITY_INSERT Siparis OFF;

-- SİPARİŞ DETAY
SET IDENTITY_INSERT SiparisDetay ON;
INSERT INTO SiparisDetay (OrderDetailID, OrderID, MenuItemID, Miktar, Fiyat) VALUES
(1,  1,  1,  1, 120.00),
(2,  1,  4,  1,  35.00),
(3,  2,  5,  1, 110.00),
(4,  2,  6,  1,  50.00),
(5,  3,  16, 1, 120.00),
(6,  3,  18, 1,  45.00),
(7,  3,  17, 1, 160.00),
(8,  4,  9,  1, 110.00),
(9,  5,  19, 1, 110.00),
(10, 5,  20, 1, 130.00),
(11, 6,  5,  1, 110.00),
(12, 7,  31, 1, 100.00),
(13, 7,  33, 2,  40.00),
(14, 8,  28, 1, 180.00),
(15, 8,  30, 1,  55.00),
(16, 9,  25, 1, 115.00),
(17, 10, 22, 1, 200.00),
(18, 11, 20, 1, 130.00),
(19, 12, 22, 1, 200.00),
(20, 13, 32, 2,  90.00),
(21, 14, 10, 1, 130.00),
(22, 14, 12, 1,  75.00),
(23, 15, 9,  1, 110.00),
(24, 16, 17, 1, 160.00),
(25, 16, 16, 1, 120.00),
(26, 17, 32, 2,  90.00),
(27, 18, 3,  1, 150.00),
(28, 19, 31, 1, 100.00),
(29, 19, 33, 1,  20.00),
(30, 20, 23, 1, 180.00),
(31, 21, 13, 1,  90.00),
(32, 21, 14, 1,  95.00),
(33, 22, 6,  1,  50.00),
(34, 22, 7,  1,  40.00),
(35, 23, 28, 1, 180.00),
(36, 24, 19, 1, 110.00),
(37, 25, 5,  1, 110.00),
(38, 25, 8,  1,  55.00);
SET IDENTITY_INSERT SiparisDetay OFF;

-- ÖDEME
SET IDENTITY_INSERT Odeme ON;
INSERT INTO Odeme (PaymentID, OrderID, PaymentDate, Amount, PaymentMethod) VALUES
(1,  1,  '2025-07-25 12:05:00', 155.00, 'Kredi Kartı'),
(2,  2,  '2025-07-25 13:05:00', 160.00, 'Nakit'),
(3,  3,  '2025-07-26 11:05:00', 325.00, 'Kredi Kartı'),
(4,  4,  '2025-07-26 14:05:00', 110.00, 'Nakit'),
(5,  5,  '2025-07-27 12:35:00', 240.00, 'Kredi Kartı'),
(6,  6,  '2025-07-27 13:35:00', 110.00, 'Nakit'),
(7,  7,  '2025-07-28 11:05:00', 140.00, 'Kredi Kartı'),
(8,  8,  '2025-07-28 14:05:00', 235.00, 'Kredi Kartı'),
(9,  16, '2025-08-01 13:05:00', 280.00, 'Kredi Kartı'),
(10, 17, '2025-08-01 14:05:00',  90.00, 'Nakit');
SET IDENTITY_INSERT Odeme OFF;

-- TESLİMAT (10 adet)
SET IDENTITY_INSERT Teslimat ON;
INSERT INTO Teslimat (DeliveryID, OrderID, PersonelID, DeliveryStatus, DeliveryTime) VALUES
(1,  1,  1, 'Teslim Edildi', '2025-07-25 12:45:00'),
(2,  2,  2, 'Teslim Edildi', '2025-07-25 13:50:00'),
(3,  3,  1, 'Teslim Edildi', '2025-07-26 12:00:00'),
(4,  4,  4, 'Teslim Edildi', '2025-07-26 14:55:00'),
(5,  5,  4, 'Teslim Edildi', '2025-07-27 13:20:00'),
(6,  6,  2, 'Teslim Edildi', '2025-07-27 14:15:00'),
(7,  7,  5, 'Teslim Edildi', '2025-07-28 12:00:00'),
(8,  8,  3, 'Teslim Edildi', '2025-07-28 15:00:00'),
(9,  16, 1, 'Teslim Edildi', '2025-08-01 14:00:00'),
(10, 17, 3, 'Teslim Edildi', '2025-08-01 15:00:00');
SET IDENTITY_INSERT Teslimat OFF;

-- YORUM (25 adet)
SET IDENTITY_INSERT Yorum ON;
INSERT INTO Yorum (ReviewID, KullaniciID, RestaurantID, Puan, Yorum, ReviewDate) VALUES
(1,  1,  1, 5, 'Adana kebap muhteşemdi, kesinlikle tavsiye ederim!',                     '2025-07-25 13:00:00'),
(2,  2,  2, 4, 'Mantı çok lezzetliydi ama biraz geç geldi.',                             '2025-07-25 14:00:00'),
(3,  3,  5, 5, 'Sushi Tokyo harika, dragon roll inanılmazdı!',                            '2025-07-26 13:00:00'),
(4,  4,  3, 3, 'Pizza idare eder, özel bir şey değil.',                                  '2025-07-26 15:00:00'),
(5,  5,  6, 5, 'Burger Bros en iyi burger yeri! BBQ burgeri mükemmel.',                  '2025-07-27 14:00:00'),
(6,  6,  2, 4, 'Kuru fasulye ev yapımı tadında, çok beğendim.',                          '2025-07-27 15:00:00'),
(7,  7,  10, 4, 'Izgara köfte çok güzeldi, bir daha gelirim.',                           '2025-07-28 12:30:00'),
(8,  8,  9, 5, 'Serpme kahvaltı mükemmeldi, her şey tazeydi!',                           '2025-07-28 16:00:00'),
(9,  1,  5, 5, 'Japon mutfağını bu kadar otantik yapan başka yer yok.',                  '2025-08-01 15:00:00'),
(10, 3,  10, 2, 'Köfteler biraz kuruydu, beklentimin altında kaldı.',                   '2025-08-01 16:00:00'),
(11, 9,  8, 4, 'Kung Pao tavuk harikaydı, biraz acılıydı ama lezzetliydi.',             '2025-07-29 11:00:00'),
(12, 10, 7, 5, 'Levrek ızgara taze ve lezzetliydi, deniz manzarası da cabası.',         '2025-07-29 12:00:00'),
(13, 11, 6, 3, 'Burger iyi ama fiyatına göre biraz küçük.',                             '2025-07-29 13:00:00'),
(14, 12, 7, 5, 'En iyi balık restoranı, karides güveç muhteşemdi!',                     '2025-07-30 10:00:00'),
(15, 13, 2, 5, 'Gözleme harika, çay da çok güzeldi.',                                   '2025-07-30 11:30:00'),
(16, 14, 3, 4, 'Pepperoni pizza lezzetliydi, tekrar sipariş veririm.',                  '2025-07-30 12:00:00'),
(17, 15, 3, 2, 'Pizza soğuk geldi, hayal kırıklığı yaşadım.',                           '2025-07-30 13:00:00'),
(18, 16, 4, 5, 'Vegan seçenekler çok kaliteli, avokado toast harika!',                  '2025-08-03 14:00:00'),
(19, 17, 2, 3, 'Yemekler güzeldi ama teslimat çok geç oldu.',                           '2025-08-04 12:00:00'),
(20, 18, 9, 5, 'Kahvaltı için en doğru adres, menemen süperdi.',                        '2025-08-04 15:00:00'),
(21, 19, 10, 4, 'Çiğ köfte dürüm tazeydi ve lezzetliydi.',                             '2025-08-05 11:00:00'),
(22, 20, 7, 1, 'Balık bayat geldi, kesinlikle önermiyorum!',                            '2025-08-05 13:00:00'),
(23, 5,  1, 5, 'İskender kebap inanılmaz, tereyağı ve yoğurt mükemmel uyum.',           '2025-08-02 12:30:00'),
(24, 7,  10, 5, 'Köfte dünyası adını hak ediyor, harika bir deneyimdi.',                '2025-08-02 13:30:00'),
(25, 2,  8, 4, 'Dim sum çok güzeldi, Çin mutfağı sevenler için ideal.',                 '2025-08-03 11:00:00');
SET IDENTITY_INSERT Yorum OFF;

EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
