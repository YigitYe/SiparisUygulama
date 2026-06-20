using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Helpers;
using SiparisUygulama.Models;

namespace SiparisUygulama.Data
{
    public static class DbSeeder
    {
        public static void Seed(SiparisDbContext context)
        {
            // Sabit kullanıcıları her zaman kontrol et
            SeedKullanicilar(context);
            SeedYoneticiler(context);
            SeedPersonel(context);

            // Restoran sayısı 10'dan azsa tam reseed yap
            if (context.Restorans.Count() < 10)
            {
                FullReseed(context);
            }

            // tugba_keyf kullanıcısını ve verilerini ekle
            SeedTugba(context);
        }

        private static void SeedTugba(SiparisDbContext context)
        {
            // Zaten varsa atla
            if (context.Kullanicis.Any(k => k.KullaniciAdi == "tugba_keyf")) return;

            // Kullanıcı ekle (tugba123 şifresi)
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Kullanici ON;
INSERT INTO Kullanici (KullaniciId,KullaniciAdi,Sifre,Email,Telefon,TeslimatAdresi,VarsayilanEnlem,VarsayilanBoylam)
VALUES (50,'tugba_keyf','cdb5dfcc1238e6d87e305dc84430eea150e817a6cb1da11f85d0801a4808253d','tugba@example.com','05321112233','Istanbul, Kadikoy, Moda Cad. No:12',40.9906,29.0262);
SET IDENTITY_INSERT Kullanici OFF;");

            // 8 sipariş ekle
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Siparis ON;
INSERT INTO Siparis (OrderId,KullaniciId,OrderDate,DeliveryAddress,TotalAmount,OrderStatus) VALUES
(101,50,'2026-03-01 12:30:00','Istanbul, Kadikoy, Moda Cad. No:12',245.00,'Teslim Edildi'),
(102,50,'2026-03-05 19:15:00','Istanbul, Kadikoy, Moda Cad. No:12',180.00,'Teslim Edildi'),
(103,50,'2026-03-10 13:00:00','Istanbul, Kadikoy, Moda Cad. No:12',320.00,'Teslim Edildi'),
(104,50,'2026-03-14 20:45:00','Istanbul, Kadikoy, Moda Cad. No:12',95.00,'Teslim Edildi'),
(105,50,'2026-03-18 18:00:00','Istanbul, Kadikoy, Moda Cad. No:12',410.00,'Teslim Edildi'),
(106,50,'2026-03-22 12:00:00','Istanbul, Kadikoy, Moda Cad. No:12',155.00,'Teslim Edildi'),
(107,50,'2026-03-26 21:30:00','Istanbul, Kadikoy, Moda Cad. No:12',280.00,'Teslim Edildi'),
(108,50,'2026-03-29 19:00:00','Istanbul, Kadikoy, Moda Cad. No:12',130.00,'Teslim Edildi');
SET IDENTITY_INSERT Siparis OFF;");

            // Sipariş detayları (restoranların var olan menu item ID'leri)
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT SiparisDetay ON;
INSERT INTO SiparisDetay (OrderDetailId,OrderId,MenuItemId,Miktar,Fiyat) VALUES
(201,101,1,2,55.00),(202,101,2,1,45.00),(203,101,3,1,90.00),
(204,102,5,2,40.00),(205,102,6,1,60.00),(206,102,8,1,40.00),
(207,103,10,1,85.00),(208,103,11,2,75.00),(209,103,12,1,85.00),
(210,104,15,1,45.00),(211,104,16,1,50.00),
(212,105,18,2,90.00),(213,105,19,1,95.00),(214,105,20,1,135.00),
(215,106,22,2,35.00),(216,106,23,1,45.00),(217,106,24,1,40.00),
(218,107,26,1,120.00),(219,107,27,2,80.00),
(220,108,29,1,65.00),(221,108,30,1,65.00);
SET IDENTITY_INSERT SiparisDetay OFF;");

            // Teslimatlar
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Teslimat ON;
INSERT INTO Teslimat (DeliveryId,OrderId,DeliveryStatus,DeliveryTime,PersonelId) VALUES
(101,101,'Teslim Edildi','2026-03-01 13:10:00',1),
(102,102,'Teslim Edildi','2026-03-05 20:05:00',2),
(103,103,'Teslim Edildi','2026-03-10 13:55:00',3),
(104,104,'Teslim Edildi','2026-03-14 21:30:00',4),
(105,105,'Teslim Edildi','2026-03-18 18:50:00',5),
(106,106,'Teslim Edildi','2026-03-22 12:45:00',1),
(107,107,'Teslim Edildi','2026-03-26 22:15:00',2),
(108,108,'Teslim Edildi','2026-03-29 19:50:00',3);
SET IDENTITY_INSERT Teslimat OFF;");

            // Yorumlar (farklı restoranlar, çeşitli puanlar ve yorumlar)
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Yorum ON;
INSERT INTO Yorum (ReviewId,KullaniciId,RestaurantId,Puan,Yorum,ReviewDate,DuyguAnalizSkoru) VALUES
(101,50,1,5,'Lezzet Duragi harika! Kofte cok lezzetliydi, servis hizliydi.','2026-03-01 14:00:00',0.72),
(102,50,3,5,'Pizza Romanin Margherita pizzasi mukemmeldi, kesinlikle tavsiye ederim.','2026-03-05 21:00:00',0.65),
(103,50,5,5,'Sushi Tokyoda yedigim salmon roll hayatimin en iyisiydi!','2026-03-10 15:00:00',0.80),
(104,50,6,3,'Burger Bros biraz yavas geldi ama burgerin tadi yerindeydi.','2026-03-14 22:00:00',0.05),
(105,50,4,4,'Gronun vejetaryen tabagi cok doyurucu ve saglikliydı.','2026-03-18 19:30:00',0.45),
(106,50,9,5,'Kahvalti Evinin menemen ve pogacasi cok tazeydi, sabah kahvaltisi icin ideal.','2026-03-22 13:30:00',0.70),
(107,50,10,4,'Kofte Dunayasinda piyaz ile servis edilen kofteler nefisti.','2026-03-26 23:00:00',0.50),
(108,50,2,2,'Anadolu Sofrasinin lahmacunu biraz soguk geldi, beklentimin altinda kaldi.','2026-03-29 20:30:00',-0.30);
SET IDENTITY_INSERT Yorum OFF;");
        }

        private static void FullReseed(SiparisDbContext context)
        {
            // FK sırasına göre temizle
            context.Database.ExecuteSqlRaw("DELETE FROM SiparisDetay");
            context.Database.ExecuteSqlRaw("DELETE FROM Teslimat");
            context.Database.ExecuteSqlRaw("DELETE FROM Yorum");
            context.Database.ExecuteSqlRaw("DELETE FROM Odeme");
            context.Database.ExecuteSqlRaw("DELETE FROM Siparis");
            context.Database.ExecuteSqlRaw("DELETE FROM Menu");
            context.Database.ExecuteSqlRaw("DELETE FROM Restoran");
            context.Database.ExecuteSqlRaw("DELETE FROM Kullanici");
            context.Database.ExecuteSqlRaw("DELETE FROM Yonetici");
            context.Database.ExecuteSqlRaw("DELETE FROM Personel");

            // Restoranlar (IDENTITY INSERT ON)
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Restoran ON;
INSERT INTO Restoran (RestaurantId,RestaurantName,Adres,TelefonNumarasi,MutfakTuru,GorselUrl,Enlem,Boylam) VALUES
(1,'Lezzet Duragi','Istanbul, Besiktas','02123456789','Türk','https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600&q=80',41.0422,29.0094),
(2,'Anadolu Sofrasi','Ankara, Keçiören','03122334454','Türk','https://images.unsplash.com/photo-1561043433-aaf687c4cf04?w=600&q=80',39.9334,32.8597),
(3,'Pizza Roma','Istanbul, Kadiköy','02164445566','Italyan','https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=600&q=80',40.9906,29.0262),
(4,'Grön','Istanbul, Caddebostan','02124151516','Vejetaryen','https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600&q=80',40.9556,29.0653),
(5,'Sushi Tokyo','Istanbul, Nisantasi','02122334455','Japon','https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=600&q=80',41.0476,28.9939),
(6,'Burger Bros','Istanbul, Sisli','02122112233','Amerikan','https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600&q=80',41.061,28.9878),
(7,'Deniz Balik','Izmir, Karsiyaka','02324567890','Deniz Ürünleri','https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=600&q=80',38.461,27.1046),
(8,'Çin Bahçesi','Ankara, Çankaya','03124441234','Çin','https://images.unsplash.com/photo-1563245372-f21724e3856d?w=600&q=80',39.903,32.8597),
(9,'Kahvalti Evi','Istanbul, Moda','02163334455','Kahvalti','https://images.unsplash.com/photo-1533089860892-a7c6f0a88666?w=600&q=80',40.9872,29.0306),
(10,'Köfte Dünyasi','Istanbul, Fatih','02125556677','Türk','https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600&q=80',41.0138,28.938);
SET IDENTITY_INSERT Restoran OFF;");

            // Menüler
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Menu ON;
INSERT INTO Menu (MenuItemId,RestaurantId,ItemName,Aciklama,Fiyat,Kategori,GorselUrl) VALUES
(1,1,'Adana Kebap','Acili el yapimi kebap',120.00,'Et','https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=400&q=80'),
(2,1,'Lahmacun','Ince hamurlu kiymali',30.00,'Hamur','https://images.unsplash.com/photo-1548940740-204726a19be3?w=400&q=80'),
(3,1,'Iskender','Tereyagli yogurtlu döner',150.00,'Et','https://images.unsplash.com/photo-1544025162-d76694265947?w=400&q=80'),
(4,1,'Mercimek Çorbasi','Ev yapimi',35.00,'Çorba','https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400&q=80'),
(5,2,'Manti','Kayseri mantisi yogurtlu',110.00,'Hamur','https://images.unsplash.com/photo-1565557623262-b51c2513a641?w=400&q=80'),
(6,2,'Kuru Fasulye','Ev yapimi kuru fasulye',50.00,'Zeytinyagli','https://images.unsplash.com/photo-1512058564366-18510be2db19?w=400&q=80'),
(7,2,'Pilav','Tereyagli pirinç pilavi',40.00,'Yan Ürün','https://images.unsplash.com/photo-1536304929831-ee1ca9d44906?w=400&q=80'),
(8,2,'Gözleme','Peynirli ispanakli',55.00,'Hamur','https://images.unsplash.com/photo-1630383249896-424e482df921?w=400&q=80'),
(9,3,'Margarita','Domates sos mozzarella',110.00,'Pizza','https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400&q=80'),
(10,3,'Pepperoni','Bol pepperonili',130.00,'Pizza','https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&q=80'),
(11,3,'Dört Peynir','Parmesan gouda cheddar',140.00,'Pizza','https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400&q=80'),
(12,3,'Sezar Salata','Kruton parmesan',75.00,'Salata','https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&q=80'),
(13,4,'Avokado Toast','Tam tahil ekmek avokado',90.00,'Vegan','https://images.unsplash.com/photo-1541519227354-08fa5d50c820?w=400&q=80'),
(14,4,'Kinoa Kasesi','Sebzeli kinoa tahil',95.00,'Vegan','https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400&q=80'),
(15,4,'Smoothie Bowl','Meyve granola',80.00,'Vegan','https://images.unsplash.com/photo-1502741224143-90386d7f8c82?w=400&q=80'),
(16,5,'Salmon Nigiri','6 parça somon',120.00,'Sushi','https://images.unsplash.com/photo-1534482421-64566f976cfa?w=400&q=80'),
(17,5,'Dragon Roll','Avokado karides',160.00,'Maki','https://images.unsplash.com/photo-1617196034183-421b4040d0a1?w=400&q=80'),
(18,5,'Miso Çorba','Geleneksel Japon',45.00,'Çorba','https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400&q=80'),
(19,6,'Classic Burger','Dana köfte cheddar',110.00,'Burger','https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400&q=80'),
(20,6,'BBQ Burger','Barbekü sos karamelize sogan',130.00,'Burger','https://images.unsplash.com/photo-1550317138-10000687a72b?w=400&q=80'),
(21,6,'Patates Kizartma','Çitir patates',45.00,'Yan Ürün','https://images.unsplash.com/photo-1541592106381-b31e9677c0e5?w=400&q=80'),
(22,7,'Levrek Izgara','Taze levrek',200.00,'Balik','https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?w=400&q=80'),
(23,7,'Karides Güveç','Domates soslu karides',180.00,'Deniz','https://images.unsplash.com/photo-1565680018434-b513d5e5fd47?w=400&q=80'),
(24,7,'Balik Çorbasi','Taze deniz ürünleri',70.00,'Çorba','https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400&q=80'),
(25,8,'Kung Pao Tavuk','Baharatli tavuk fistik',115.00,'Ana Yemek','https://images.unsplash.com/photo-1525755662778-989d0524087e?w=400&q=80'),
(26,8,'Dim Sum','8 parça buharda',90.00,'Baslangiç','https://images.unsplash.com/photo-1582456891045-d5fcf476f40f?w=400&q=80'),
(27,8,'Chow Mein','Sebzeli eriste',100.00,'Eriste','https://images.unsplash.com/photo-1563245372-f21724e3856d?w=400&q=80'),
(28,9,'Serpme Kahvalti','Tam Türk kahvaltisi',180.00,'Kahvalti','https://images.unsplash.com/photo-1533089860892-a7c6f0a88666?w=400&q=80'),
(29,9,'Menemen','Domates biber yumurta',70.00,'Kahvalti','https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400&q=80'),
(30,9,'Simit Tabagi','Simit peynir zeytin',55.00,'Kahvalti','https://images.unsplash.com/photo-1509482560494-4126f8225994?w=400&q=80'),
(31,10,'Izgara Köfte','El yapimi dana köfte',100.00,'Et','https://images.unsplash.com/photo-1529042410759-befb1204b468?w=400&q=80'),
(32,10,'Çig Köfte Dürüm','Acili tatli nar eksili',45.00,'Dürüm','https://images.unsplash.com/photo-1548940740-204726a19be3?w=400&q=80'),
(33,10,'Ayran','Ev yapimi ayran',20.00,'Içecek','https://images.unsplash.com/photo-1553361371-9b22f78e8b1d?w=400&q=80');
SET IDENTITY_INSERT Menu OFF;");

            // Kullanıcılar
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Kullanici ON;
INSERT INTO Kullanici (KullaniciId,KullaniciAdi,Sifre,Email,Telefon,TeslimatAdresi,VarsayilanEnlem,VarsayilanBoylam) VALUES
(1,'ahmet123','18e8df240ddd4e8b778961056ae40b65a3417c0e806410a66c66775c7b7152a3','ahmet@mail.com','05331234567','Istanbul, Kadiköy',40.9933,29.0302),
(2,'elifg','cb85b7aa5022b0944d3cfc6dc30a62c5d6b60ee1d282617995e69104eb50cc7f','elif@mail.com','05349876543','Ankara, Çankaya',39.903,32.8597),
(3,'mehmet_34','8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92','mehmet@mail.com','05441239876','Izmir, Bornova',38.4681,27.2197),
(4,'ayse_k','996aa6e520c29274205cf4e4db4d56488ea3bf41ccacea53076c7f9b8d0d2812','ayse@mail.com','05321112233','Istanbul, Besiktas',41.0422,29.0094),
(5,'mustafa_y','9b8769a4a742959a2d0298c36fb70623f2dfacda8436237df08d8dfd5b37374c','mustafa@mail.com','05334445566','Istanbul, Sisli',41.061,28.9878),
(6,'zeynep_t','d995c99662b02f1457e385767c576159a567f46b8c05cc7afe33555a1d04d01b','zeynep@mail.com','05367778899','Ankara, Keçiören',39.9334,32.8597),
(7,'ali_can','8aaa731eaa1031dc0512bafaffe26fc9f7047f736b35cf5e254cf0038bb98a8a','ali@mail.com','05389990011','Istanbul, Fatih',41.0138,28.938),
(8,'selin_d','db0c62df2cb10881745ed37ec33728b132bbf976b3924ce680ce90e96f3794c0','selin@mail.com','05312223344','Istanbul, Bakirköy',40.9833,28.8719),
(9,'emre_b','ceeb2fb5ab82a468a5e800afa72f799520d4d2977cab6659d58a07ea5d2dc904','emre@mail.com','05345556677','Ankara, Etimesgut',39.9455,32.6789),
(10,'pinar_a','852993dc2644f4f717184234a40a4908aa4866fffed28f7ef83575816a2744e8','pinar@mail.com','05378889900','Izmir, Karsiyaka',38.461,27.1046),
(11,'burak_s','ff296a6ceba983736e27e102b027ffac44317f8448b21fa652494feb9f6167fe','burak@mail.com','05391112233','Istanbul, Ümraniye',41.0167,29.1167),
(12,'dilan_m','0903d834cbcd64477af2d9467f81f9d9a57646d7287fa029b862f63582331078','dilan@mail.com','05323334455','Istanbul, Üsküdar',41.0233,29.015),
(13,'can_oz','363cbb0d01b504e7b546b0941be44e1784eaa3b56db9aee7f868df3a6e7196c6','can@mail.com','05356667788','Ankara, Mamak',39.9408,32.9237),
(14,'merve_y','0fe0333a3d3f3258f04a29a966c64fcfaa05e3af0d8e9a888fccb69f9704cd1b','merve@mail.com','05389990022','Istanbul, Sariyer',41.1667,29.05),
(15,'enes_k','d76fc228e9a151fd80cc34a44b25b19108d1393102c71100c26c3efaabc0aae3','enes@mail.com','05312221133','Istanbul, Pendik',40.8756,29.2342),
(16,'buse_c','f92e8d21d88f983d060b2e77951175508d5436b22de771617f4d5cb0704ac2aa','buse@mail.com','05345552266','Izmir, Alsancak',38.4377,27.1444),
(17,'furkan_t','916577b5373024b5e2c51fb78a9d1e4402135dbc3f539efa56362ce64899e323','furkan@mail.com','05378883399','Ankara, Sincan',39.9753,32.5836),
(18,'irem_b','951cfb807d5574f1e169861c7bd8376b8202cc53bd6b0a3dd905ff4b3a866aa1','irem@mail.com','05391114455','Istanbul, Maltepe',40.9286,29.1311),
(19,'oguz_d','e6c4f1e08b207bca3293aa18f651ab2dff406de38eb1696890769ab56bbc8a27','oguz@mail.com','05324445566','Istanbul, Atasehir',40.9833,29.1167),
(20,'elif_n','d4b43a2b118171959d76b56c47c27199bb989d65e617f6115257dbca8b72e67f','elifn@mail.com','05357776677','Ankara, Yenimahalle',39.9667,32.7833),
(21,'sercan_a','df5e60aae44cc3f224858c9b0a95e3b42905b826ea56099d483610134f99ebd9','sercan@mail.com','05381118899','Istanbul, Beyoglu',41.0333,28.9833),
(22,'tugba_k','df08a549bffaa2b7dd5b6d7dd0bffadf2dac01971a96415305d06d968fbe1e3d','tugba@mail.com','05314440011','Izmir, Buca',38.3833,27.1667),
(23,'tolga_m','d575c03a9d342bb147c5942ca48591c9cfc6e1ad11b9bd5be0d8121341b9eb36','tolga@mail.com','05347772233','Istanbul, Avcilar',40.9795,28.7218),
(24,'simge_y','b3efba364498c0df6dc267772a2ff505149ec387d6fbbf536502cf5ce2de630c','simge@mail.com','05370003344','Ankara, Gölbasi',39.7919,32.8097),
(25,'kaan_b','1011339385247d9487ab63e9bea0de23dadec35ce8acd290a6acc18a6c71e587','kaan@mail.com','05392225566','Istanbul, Tuzla',40.8167,29.3),
(26,'hilal_s','76d7c6a1f253a4786be124016faaad004121227fe3cf2517f64b097377bbc63a','hilal@mail.com','05325557788','Istanbul, Eyüpsultan',41.0833,28.9167),
(27,'berk_o','d3a8cacbfb8afde0d609ee13c2d804b5cca0b31d628f48f28ad0919c1405dd27','berk@mail.com','05358889900','Izmir, Konak',38.4192,27.1287),
(28,'nazli_c','6ef104d8a33d0d40b7cb5ef835c57e68d12fd5df4bb43dd381580e8fd3ea44be','nazli@mail.com','05381110022','Ankara, Altindag',39.9564,32.8897),
(29,'umut_d','73de464418f28dbfba92bcec06c4fcad0ced63a3452bd02cf368a2edfce7ceae','umut@mail.com','05313332244','Istanbul, Gaziosmanpasa',41.0667,28.9167),
(30,'gizem_k','4a235a3419f9b0e78bae26df801c61cfb5594ee5080e248e021796a196cbe2e2','gizem@mail.com','05346664466','Istanbul, Kartal',40.8897,29.1897),
(31,'Mert Demirok','4ccf3d5681660ab6e759a9c90bcf270ffdc1ad57678f15c40d1336f6174c7f6d','mert@gmail.com','054565432',NULL,NULL,NULL);
SET IDENTITY_INSERT Kullanici OFF;");

            // Yöneticiler
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Yonetici ON;
INSERT INTO Yonetici (KullaniciID,KullaniciAdi,Sifre,Rol) VALUES
(1,'admin1','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9','Admin'),
(2,'mehmet','03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4','Musteri'),
(3,'kebapci','6e4896950cae800fae0e3e1ae26f1ee90e9a70c7c30fb95715f28be0ce13eac0','Restoran'),
(4,'kurye01','03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4','Personel');
SET IDENTITY_INSERT Yonetici OFF;");

            // Personel
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Personel ON;
INSERT INTO Personel (PersonelId,Adi,IletisimNumarasi,TeslimatAlani,AnlikEnlem,AnlikBoylam,MusaitlikDurumu,Sifre) VALUES
(1,'Ali Kaya','05332221111','Kadiköy',40.9933,29.0302,1,NULL),
(2,'Zeynep Yilmaz','05443334444','Çankaya',39.903,32.8597,1,NULL),
(3,'Mustafa Tozan','04531234562','Maltepe',40.9160,29.1487,1,NULL),
(4,'Fatma Demir','05367778888','Sisli',41.061,28.9878,1,NULL),
(5,'Hasan Çelik','05389991111','Fatih',41.0138,28.938,1,NULL);
SET IDENTITY_INSERT Personel OFF;");

            // Siparişler
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Siparis ON;
INSERT INTO Siparis (OrderId,KullaniciId,OrderDate,DeliveryAddress,TotalAmount,OrderStatus) VALUES
(1,1,'2025-07-25 12:00:00','Istanbul, Kadiköy',150.00,'Teslim Edildi'),
(2,2,'2025-07-25 13:00:00','Ankara, Çankaya',160.00,'Teslim Edildi'),
(3,3,'2025-07-26 11:00:00','Izmir, Bornova',200.00,'Teslim Edildi'),
(4,4,'2025-07-26 14:00:00','Istanbul, Besiktas',110.00,'Teslim Edildi'),
(5,5,'2025-07-27 12:30:00','Istanbul, Sisli',175.00,'Teslim Edildi'),
(6,6,'2025-07-27 13:30:00','Ankara, Keçiören',110.00,'Teslim Edildi'),
(7,7,'2025-07-28 11:00:00','Istanbul, Fatih',145.00,'Teslim Edildi'),
(8,8,'2025-07-28 14:00:00','Istanbul, Bakirköy',240.00,'Teslim Edildi'),
(9,9,'2025-07-29 10:00:00','Ankara, Etimesgut',115.00,'Teslim Edildi'),
(10,10,'2025-07-29 11:00:00','Izmir, Karsiyaka',200.00,'Teslim Edildi'),
(11,11,'2025-07-29 12:00:00','Istanbul, Ümraniye',130.00,'Teslim Edildi'),
(12,12,'2025-07-30 09:00:00','Istanbul, Üsküdar',180.00,'Teslim Edildi'),
(13,13,'2025-07-30 10:30:00','Ankara, Mamak',100.00,'Teslim Edildi'),
(14,14,'2025-07-30 11:00:00','Istanbul, Sariyer',160.00,'Teslim Edildi'),
(15,15,'2025-07-30 12:00:00','Istanbul, Pendik',110.00,'Teslim Edildi'),
(16,1,'2025-08-01 13:00:00','Istanbul, Kadiköy',280.00,'Teslim Edildi'),
(17,3,'2025-08-01 14:00:00','Izmir, Bornova',90.00,'Teslim Edildi'),
(18,5,'2025-08-02 11:00:00','Istanbul, Sisli',155.00,'Teslim Edildi'),
(19,7,'2025-08-02 12:00:00','Istanbul, Fatih',120.00,'Teslim Edildi'),
(20,2,'2025-08-03 10:00:00','Ankara, Çankaya',200.00,'Teslim Edildi'),
(21,16,'2025-08-03 13:00:00','Izmir, Alsancak',130.00,'Teslim Edildi'),
(22,17,'2025-08-04 11:00:00','Ankara, Sincan',115.00,'Teslim Edildi'),
(23,18,'2025-08-04 14:00:00','Istanbul, Maltepe',180.00,'Teslim Edildi'),
(24,19,'2025-08-05 10:00:00','Istanbul, Atasehir',110.00,'Teslim Edildi'),
(25,20,'2025-08-05 12:00:00','Ankara, Yenimahalle',200.00,'Teslim Edildi'),
(26,18,'2026-03-27 17:26:00','Istanbul, Maltepe',450.00,'Teslim Edildi'),
(27,18,'2026-03-27 17:32:00','Istanbul, Maltepe',150.00,'Teslim Edildi'),
(28,18,'2026-03-28 11:12:00','Istanbul, Maltepe',165.00,'Teslim Edildi'),
(29,18,'2026-03-28 11:24:00','Istanbul, Maltepe',205.00,'Teslim Edildi');
SET IDENTITY_INSERT Siparis OFF;");

            // Sipariş Detayları
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT SiparisDetay ON;
INSERT INTO SiparisDetay (OrderDetailId,OrderId,MenuItemId,Miktar,Fiyat) VALUES
(1,1,1,1,120.00),(2,1,4,1,35.00),(3,2,5,1,110.00),(4,2,6,1,50.00),
(5,3,16,1,120.00),(6,3,18,1,45.00),(7,3,17,1,160.00),(8,4,9,1,110.00),
(9,5,19,1,110.00),(10,5,20,1,130.00),(11,6,5,1,110.00),(12,7,31,1,100.00),
(13,7,33,2,40.00),(14,8,28,1,180.00),(15,8,30,1,55.00),(16,9,25,1,115.00),
(17,10,22,1,200.00),(18,11,20,1,130.00),(19,12,22,1,200.00),(20,13,32,2,90.00),
(21,14,10,1,130.00),(22,14,12,1,75.00),(23,15,9,1,110.00),(24,16,17,1,160.00),
(25,16,16,1,120.00),(26,17,32,2,90.00),(27,18,3,1,150.00),(28,19,31,1,100.00),
(29,19,33,1,20.00),(30,20,23,1,180.00),(31,21,13,1,90.00),(32,21,14,1,95.00),
(33,22,6,1,50.00),(34,22,7,1,40.00),(35,23,28,1,180.00),(36,24,19,1,110.00),
(37,25,5,1,110.00),(38,25,8,1,55.00),(39,26,22,1,200.00),(40,26,23,1,180.00),
(41,26,24,1,70.00),(42,27,1,1,120.00),(43,27,2,1,30.00),(44,28,5,1,110.00),
(45,28,8,1,55.00),(46,29,25,1,115.00),(47,29,26,1,90.00);
SET IDENTITY_INSERT SiparisDetay OFF;");

            // Teslimatlar
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Teslimat ON;
INSERT INTO Teslimat (DeliveryId,OrderId,PersonelId,DeliveryStatus,DeliveryTime) VALUES
(1,1,1,'Teslim Edildi','2025-07-25 12:35:00'),
(2,2,2,'Teslim Edildi','2025-07-25 13:40:00'),
(3,3,1,'Teslim Edildi','2025-07-26 11:40:00'),
(4,4,3,'Teslim Edildi','2025-07-26 14:38:00'),
(5,5,4,'Teslim Edildi','2025-07-27 13:10:00'),
(6,6,2,'Teslim Edildi','2025-07-27 14:10:00'),
(7,7,5,'Teslim Edildi','2025-07-28 11:38:00'),
(8,8,3,'Teslim Edildi','2025-07-28 14:38:00'),
(9,9,1,'Teslim Edildi','2025-07-29 10:38:00'),
(10,10,4,'Teslim Edildi','2025-07-29 11:38:00'),
(11,11,5,'Teslim Edildi','2025-07-29 12:38:00'),
(12,12,2,'Teslim Edildi','2025-07-30 09:38:00'),
(13,13,3,'Teslim Edildi','2025-07-30 11:08:00'),
(14,14,1,'Teslim Edildi','2025-07-30 11:38:00'),
(15,15,4,'Teslim Edildi','2025-07-30 12:38:00'),
(16,16,5,'Teslim Edildi','2025-08-01 13:38:00'),
(17,17,2,'Teslim Edildi','2025-08-01 14:38:00'),
(18,18,1,'Teslim Edildi','2025-08-02 11:38:00'),
(19,19,3,'Teslim Edildi','2025-08-02 12:38:00'),
(20,20,4,'Teslim Edildi','2025-08-03 10:38:00'),
(21,21,5,'Teslim Edildi','2025-08-03 13:38:00'),
(22,22,2,'Teslim Edildi','2025-08-04 11:38:00'),
(23,23,1,'Teslim Edildi','2025-08-04 14:38:00'),
(24,24,3,'Teslim Edildi','2025-08-05 10:38:00'),
(25,25,4,'Teslim Edildi','2025-08-05 12:38:00'),
(26,26,5,'Teslim Edildi','2026-03-27 18:05:00'),
(27,27,1,'Teslim Edildi','2026-03-27 18:10:00'),
(28,28,2,'Teslim Edildi','2026-03-28 11:50:00'),
(29,29,3,'Teslim Edildi','2026-03-28 12:02:00');
SET IDENTITY_INSERT Teslimat OFF;");

            // Yorumlar
            context.Database.ExecuteSqlRaw(@"
SET IDENTITY_INSERT Yorum ON;
INSERT INTO Yorum (ReviewId,KullaniciId,RestaurantId,Puan,Yorum,ReviewDate,DuyguAnalizSkoru) VALUES
(1,1,1,5,'Adana kebap muhtesemdi, kesinlikle tavsiye ederim!','2025-07-25 13:00:00',0.6),
(2,2,2,4,'Manti çok lezzetliydi ama biraz geç geldi.','2025-07-25 14:00:00',0.1),
(3,3,5,5,'Sushi Tokyo harika, dragon roll inanilmazdi!','2025-07-26 13:00:00',0.6),
(4,4,3,3,'Pizza idare eder, özel bir sey degil.','2025-07-26 15:00:00',-0.1),
(5,5,6,5,'Burger Bros en iyi burger yeri! BBQ burgeri mükemmel.','2025-07-27 14:00:00',0.7),
(6,6,2,4,'Kuru fasulye ev yapimi tadinda, çok begendim.','2025-07-27 15:00:00',0.5),
(7,7,10,4,'Izgara köfte çok güzeldi, bir daha gelirim.','2025-07-28 12:30:00',0.6),
(8,8,9,5,'Serpme kahvalti mükemmeldi, her sey tazeydi!','2025-07-28 16:00:00',0.7),
(9,1,5,5,'Japon mutfagini bu kadar otantik yapan baska yer yok.','2025-08-01 15:00:00',0.5),
(10,3,10,2,'Köfteler biraz kuruydu, beklentimin altinda kaldi.','2025-08-01 16:00:00',-0.3),
(11,9,8,4,'Kung Pao tavuk harikaydi, biraz aciliydi ama lezzetliydi.','2025-07-29 11:00:00',0.7),
(12,10,7,5,'Levrek izgara taze ve lezzetliydi, deniz manzarasi da cabasi.','2025-07-29 12:00:00',0.7),
(13,11,6,3,'Burger iyi ama fiyatina göre biraz küçük.','2025-07-29 13:00:00',0.1),
(14,12,7,5,'En iyi balik restorani, karides güveç muhtesemdi!','2025-07-30 10:00:00',0.6),
(15,13,2,5,'Gözleme harika, çay da çok güzeldi.','2025-07-30 11:30:00',0.7),
(16,14,3,4,'Pepperoni pizza lezzetliydi, tekrar siparis veririm.','2025-07-30 12:00:00',0.7),
(17,15,3,2,'Pizza soguk geldi, hayal kirikligi yasadim.','2025-07-30 13:00:00',-0.5),
(18,16,4,5,'Vegan seçenekler çok kaliteli, avokado toast harika!','2025-08-03 14:00:00',0.7),
(19,17,2,3,'Yemekler güzeldi ama teslimat çok geç oldu.','2025-08-04 12:00:00',-0.2),
(20,18,9,5,'Kahvalti için en dogru adres, menemen süperdi.','2025-08-04 15:00:00',0.6),
(21,19,10,4,'Çig köfte dürüm tazeydi ve lezzetliydi.','2025-08-05 11:00:00',0.7),
(22,20,7,1,'Balik bayat geldi, kesinlikle önermiyorum!','2025-08-05 13:00:00',-0.6),
(23,5,1,5,'Iskender kebap inanilmaz, tereyagi ve yogurt mükemmel uyum.','2025-08-02 12:30:00',0.6),
(24,7,10,5,'Köfte dünyasi adini hak ediyor, harika bir deneyimdi.','2025-08-02 13:30:00',0.44),
(25,2,8,4,'Dim sum çok güzeldi, Çin mutfagi sevenler için ideal.','2025-08-03 11:00:00',0.53),
(26,18,7,5,'Çok beğendim serviste hızlıydı','2026-03-28 11:00:00',0.7),
(27,18,1,2,'soğuktu ve yavaş geldi','2026-03-28 11:19:00',-0.6);
SET IDENTITY_INSERT Yorum OFF;");
        }

        private static void SeedKullanicilar(SiparisDbContext context)
        {
            // Bu metod artık FullReseed içinde yapılıyor, burada sadece güvenlik için
        }

        private static void SeedYoneticiler(SiparisDbContext context)
        {
            // Bu metod artık FullReseed içinde yapılıyor
        }

        private static void SeedPersonel(SiparisDbContext context)
        {
            // Bu metod artık FullReseed içinde yapılıyor
        }
    }
}
