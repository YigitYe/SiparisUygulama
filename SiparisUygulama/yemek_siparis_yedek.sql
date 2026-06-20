-- MySQL dump 10.13  Distrib 9.3.0, for macos14.7 (arm64)
--
-- Host: localhost    Database: YemekSiparisDB
-- ------------------------------------------------------
-- Server version	9.3.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AspNetRoles`
--

DROP TABLE IF EXISTS `AspNetRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetRoles` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AspNetRoles`
--

LOCK TABLES `AspNetRoles` WRITE;
/*!40000 ALTER TABLE `AspNetRoles` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetRoles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AspNetUsers`
--

DROP TABLE IF EXISTS `AspNetUsers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetUsers` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AdSoyad` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Rol` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AspNetUsers`
--

LOCK TABLES `AspNetUsers` WRITE;
/*!40000 ALTER TABLE `AspNetUsers` DISABLE KEYS */;
/*!40000 ALTER TABLE `AspNetUsers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Kullanici`
--

DROP TABLE IF EXISTS `Kullanici`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Kullanici` (
  `KullaniciID` int NOT NULL AUTO_INCREMENT,
  `KullaniciAdi` varchar(50) NOT NULL,
  `Sifre` varchar(255) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Telefon` varchar(20) DEFAULT NULL,
  `TeslimatAdresi` text,
  PRIMARY KEY (`KullaniciID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Kullanici`
--

LOCK TABLES `Kullanici` WRITE;
/*!40000 ALTER TABLE `Kullanici` DISABLE KEYS */;
INSERT INTO `Kullanici` VALUES (1,'ahmet123','sifre123','ahmet@mail.com','05331234567','İstanbul, Kadıköy'),(2,'elifg','guvenliSifre!','elif@mail.com','05349876543','Ankara, Çankaya'),(3,'mehmet_34','123456','mehmet@mail.com','05441239876','İzmir, Bornova');
/*!40000 ALTER TABLE `Kullanici` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Menu`
--

DROP TABLE IF EXISTS `Menu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Menu` (
  `MenuItemID` int NOT NULL AUTO_INCREMENT,
  `RestaurantID` int NOT NULL,
  `ItemName` varchar(100) NOT NULL,
  `Aciklama` text,
  `Fiyat` decimal(10,2) NOT NULL,
  `Kategori` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`MenuItemID`),
  KEY `RestaurantID` (`RestaurantID`),
  CONSTRAINT `menu_ibfk_1` FOREIGN KEY (`RestaurantID`) REFERENCES `Restoran` (`RestaurantID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Menu`
--

LOCK TABLES `Menu` WRITE;
/*!40000 ALTER TABLE `Menu` DISABLE KEYS */;
INSERT INTO `Menu` VALUES (1,1,'Kebap','Adana usulü acılı kebap',80.00,'Et Yemekleri'),(2,1,'Lahmacun','Bol yeşillikli lahmacun',25.00,'Hamur İşleri'),(3,2,'Mantı','Kayseri mantısı yoğurtlu',100.00,'Hamur İşleri'),(4,2,'Kuru Fasulye','Ev yapımı kuru fasulye',45.00,'Zeytinyağlılar'),(5,2,'Kuru fasulye','Özel Kuru fasulye',70.00,'Bakla'),(8,2,'Pilav','Dünyanın en iyi pilavı',40.00,'Hamur İşleri');
/*!40000 ALTER TABLE `Menu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Odeme`
--

DROP TABLE IF EXISTS `Odeme`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Odeme` (
  `PaymentID` int NOT NULL AUTO_INCREMENT,
  `OrderID` int NOT NULL,
  `PaymentDate` datetime NOT NULL,
  `Amount` decimal(10,2) DEFAULT NULL,
  `PaymentMethod` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`PaymentID`),
  KEY `OrderID` (`OrderID`),
  CONSTRAINT `odeme_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `Siparis` (`OrderID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Odeme`
--

LOCK TABLES `Odeme` WRITE;
/*!40000 ALTER TABLE `Odeme` DISABLE KEYS */;
INSERT INTO `Odeme` VALUES (1,1,'2025-07-25 14:51:18',105.00,'Kredi Kartı'),(2,2,'2025-07-25 14:51:18',60.00,'Nakit');
/*!40000 ALTER TABLE `Odeme` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Personel`
--

DROP TABLE IF EXISTS `Personel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Personel` (
  `PersonelID` int NOT NULL AUTO_INCREMENT,
  `Adi` varchar(100) DEFAULT NULL,
  `IletisimNumarasi` varchar(20) DEFAULT NULL,
  `TeslimatAlani` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`PersonelID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Personel`
--

LOCK TABLES `Personel` WRITE;
/*!40000 ALTER TABLE `Personel` DISABLE KEYS */;
INSERT INTO `Personel` VALUES (1,'Ali Kaya','05332221111','Kadıköy'),(2,'Zeynep Yılmaz','05443334444','Çankaya'),(4,'Mustafa Tozan','04531234562','Maltepe');
/*!40000 ALTER TABLE `Personel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Restoran`
--

DROP TABLE IF EXISTS `Restoran`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Restoran` (
  `RestaurantID` int NOT NULL AUTO_INCREMENT,
  `RestaurantName` varchar(100) NOT NULL,
  `Adres` text,
  `TelefonNumarasi` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`RestaurantID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Restoran`
--

LOCK TABLES `Restoran` WRITE;
/*!40000 ALTER TABLE `Restoran` DISABLE KEYS */;
INSERT INTO `Restoran` VALUES (1,'Lezzet Durağı','İstanbul, Beşiktaş','02123456789','lezzet@restaurant.com'),(2,'Anadolu Sofrası','Ankara, Keçiören','03122334454','anadolu@sofra.com'),(4,'Grön','Caddebostan','212415151615','gron@restoran.com');
/*!40000 ALTER TABLE `Restoran` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Siparis`
--

DROP TABLE IF EXISTS `Siparis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Siparis` (
  `OrderID` int NOT NULL AUTO_INCREMENT,
  `KullaniciID` int NOT NULL,
  `OrderDate` datetime NOT NULL,
  `DeliveryAddress` text,
  `TotalAmount` decimal(10,2) DEFAULT NULL,
  `OrderStatus` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`OrderID`),
  KEY `KullaniciID` (`KullaniciID`),
  CONSTRAINT `siparis_ibfk_1` FOREIGN KEY (`KullaniciID`) REFERENCES `Kullanici` (`KullaniciID`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Siparis`
--

LOCK TABLES `Siparis` WRITE;
/*!40000 ALTER TABLE `Siparis` DISABLE KEYS */;
INSERT INTO `Siparis` VALUES (1,1,'2025-07-25 14:50:47','İstanbul, Kadıköy',105.00,'Hazır'),(2,2,'2025-07-25 14:50:47','Ankara, Çankaya',60.00,'Teslim Edildi'),(3,1,'2025-07-28 14:22:43','Ankara üzeyir',100.00,'Hazır'),(4,1,'2025-07-28 14:27:06','Ankara çinçin',100.00,'Hazır'),(5,1,'2025-07-28 14:30:54','İstanbul Başakşehir',105.00,'Hazır'),(6,1,'2025-07-28 15:51:22','Ankra keçiören',100.00,'Hazır');
/*!40000 ALTER TABLE `Siparis` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SiparisDetay`
--

DROP TABLE IF EXISTS `SiparisDetay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SiparisDetay` (
  `OrderDetailID` int NOT NULL AUTO_INCREMENT,
  `OrderID` int NOT NULL,
  `MenuItemID` int NOT NULL,
  `Miktar` int NOT NULL,
  `Fiyat` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`OrderDetailID`),
  KEY `OrderID` (`OrderID`),
  KEY `MenuItemID` (`MenuItemID`),
  CONSTRAINT `siparisdetay_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `Siparis` (`OrderID`),
  CONSTRAINT `siparisdetay_ibfk_2` FOREIGN KEY (`MenuItemID`) REFERENCES `Menu` (`MenuItemID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SiparisDetay`
--

LOCK TABLES `SiparisDetay` WRITE;
/*!40000 ALTER TABLE `SiparisDetay` DISABLE KEYS */;
INSERT INTO `SiparisDetay` VALUES (1,1,1,1,80.00),(2,1,2,1,25.00),(3,2,3,1,60.00),(4,3,3,1,100.00),(5,4,3,1,100.00),(6,5,1,1,80.00),(7,5,2,1,25.00),(8,6,3,1,100.00);
/*!40000 ALTER TABLE `SiparisDetay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Teslimat`
--

DROP TABLE IF EXISTS `Teslimat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Teslimat` (
  `DeliveryID` int NOT NULL AUTO_INCREMENT,
  `OrderID` int NOT NULL,
  `PersonelID` int DEFAULT NULL,
  `DeliveryStatus` varchar(50) DEFAULT NULL,
  `DeliveryTime` datetime DEFAULT NULL,
  PRIMARY KEY (`DeliveryID`),
  KEY `OrderID` (`OrderID`),
  KEY `PersonelID` (`PersonelID`),
  CONSTRAINT `teslimat_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `Siparis` (`OrderID`),
  CONSTRAINT `teslimat_ibfk_2` FOREIGN KEY (`PersonelID`) REFERENCES `Personel` (`PersonelID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Teslimat`
--

LOCK TABLES `Teslimat` WRITE;
/*!40000 ALTER TABLE `Teslimat` DISABLE KEYS */;
INSERT INTO `Teslimat` VALUES (1,1,1,'Teslim Edildi','2025-07-29 11:39:38'),(2,2,2,'Teslim Edildi','2025-07-25 14:52:12'),(3,3,4,'Teslim Edildi','2025-07-29 11:33:17'),(4,4,4,'Teslim Edildi','2025-07-29 11:55:38'),(5,5,1,'Teslim Edildi','2025-07-29 11:39:31'),(6,6,4,'Teslim Edildi','2025-07-29 11:38:14');
/*!40000 ALTER TABLE `Teslimat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Yonetici`
--

DROP TABLE IF EXISTS `Yonetici`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Yonetici` (
  `KullaniciID` int NOT NULL AUTO_INCREMENT,
  `KullaniciAdi` varchar(50) NOT NULL,
  `Sifre` varchar(100) NOT NULL,
  `Rol` varchar(20) NOT NULL,
  PRIMARY KEY (`KullaniciID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Yonetici`
--

LOCK TABLES `Yonetici` WRITE;
/*!40000 ALTER TABLE `Yonetici` DISABLE KEYS */;
INSERT INTO `Yonetici` VALUES (1,'admin1','1234','Admin'),(2,'mehmet','1234','Musteri'),(3,'kebapci','1234','Restoran'),(4,'kurye01','1234','Kurye');
/*!40000 ALTER TABLE `Yonetici` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Yorum`
--

DROP TABLE IF EXISTS `Yorum`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Yorum` (
  `ReviewID` int NOT NULL AUTO_INCREMENT,
  `KullaniciID` int NOT NULL,
  `RestaurantID` int NOT NULL,
  `Puan` int DEFAULT NULL,
  `Yorum` text,
  `ReviewDate` datetime DEFAULT NULL,
  PRIMARY KEY (`ReviewID`),
  KEY `KullaniciID` (`KullaniciID`),
  KEY `RestaurantID` (`RestaurantID`),
  CONSTRAINT `yorum_ibfk_1` FOREIGN KEY (`KullaniciID`) REFERENCES `Kullanici` (`KullaniciID`),
  CONSTRAINT `yorum_ibfk_2` FOREIGN KEY (`RestaurantID`) REFERENCES `Restoran` (`RestaurantID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Yorum`
--

LOCK TABLES `Yorum` WRITE;
/*!40000 ALTER TABLE `Yorum` DISABLE KEYS */;
INSERT INTO `Yorum` VALUES (1,1,1,5,'Mükemmel yemeklerdi!','2025-07-25 14:51:42'),(2,2,2,4,'Gayet lezzetliydi ama servis yavaştı.','2025-07-25 14:51:42'),(3,1,2,3,'Gayet güzel ve doyurucuydu.','2025-07-29 13:54:31');
/*!40000 ALTER TABLE `Yorum` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-29 15:45:02
