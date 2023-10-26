-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: localhost    Database: projeto_estacionamento
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `configuracao_estacionamento`
--

DROP TABLE IF EXISTS `configuracao_estacionamento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuracao_estacionamento` (
  `id_estacionamento` int NOT NULL,
  `id_capacidade` int DEFAULT NULL,
  `capacidade` int DEFAULT NULL,
  `senha` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`id_estacionamento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `configuracao_estacionamento`
--

LOCK TABLES `configuracao_estacionamento` WRITE;
/*!40000 ALTER TABLE `configuracao_estacionamento` DISABLE KEYS */;
INSERT INTO `configuracao_estacionamento` VALUES (1,1,1000,'senha');
/*!40000 ALTER TABLE `configuracao_estacionamento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `historico_veiculos`
--

DROP TABLE IF EXISTS `historico_veiculos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historico_veiculos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `placa` varchar(10) DEFAULT NULL,
  `data_acesso` datetime DEFAULT NULL,
  `horario_entrada` datetime DEFAULT NULL,
  `horario_saida` datetime DEFAULT NULL,
  `valor_pago` decimal(10,2) DEFAULT NULL,
  `ticket` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `historico_veiculos`
--

LOCK TABLES `historico_veiculos` WRITE;
/*!40000 ALTER TABLE `historico_veiculos` DISABLE KEYS */;
INSERT INTO `historico_veiculos` VALUES (19,'aaa0001',NULL,'2023-08-01 18:58:01','2023-08-01 18:58:21',0.00,'aaa0001_20230801185801'),(20,'aaa0002',NULL,'2023-08-01 18:59:15','2023-08-01 18:59:20',0.00,'aaa0002_20230801185915'),(21,'aaa0001',NULL,'2023-08-02 21:44:29','2023-08-02 21:44:58',0.00,'aaa0001_20230802214429'),(26,'aaa0008',NULL,'2023-08-10 18:55:59','2023-08-15 18:56:13',0.00,'aaa0008_20230815185559'),(27,'aaa0003',NULL,'2023-08-10 19:00:01',NULL,NULL,'aaa0003_20230815190001'),(28,'aaa0008',NULL,'2023-08-10 19:00:05',NULL,NULL,'aaa0008_20230815190005'),(29,'aaa0009',NULL,'2023-08-10 19:00:09',NULL,NULL,'aaa0009_20230815190009'),(30,'aaa0006',NULL,'2023-08-16 18:38:39',NULL,NULL,'aaa0006_20230816183839'),(31,'bbb0001',NULL,'2023-08-23 21:33:28',NULL,NULL,'bbb0001_20230823213328'),(32,'aaa0001',NULL,'2023-10-11 20:30:20','2023-10-11 20:32:27',0.00,'aaa0001_20231011203020');
/*!40000 ALTER TABLE `historico_veiculos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tabela_valores`
--

DROP TABLE IF EXISTS `tabela_valores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tabela_valores` (
  `id` int NOT NULL AUTO_INCREMENT,
  `intervalo` varchar(5) NOT NULL,
  `valor` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tabela_valores`
--

LOCK TABLES `tabela_valores` WRITE;
/*!40000 ALTER TABLE `tabela_valores` DISABLE KEYS */;
INSERT INTO `tabela_valores` VALUES (1,'30',9.00),(2,'31',16.00),(3,'120',16.00),(4,'180',16.00),(5,'200',16.00),(6,'300',16.00),(7,'400',16.00),(8,'15',0.00),(9,'33',33.00);
/*!40000 ALTER TABLE `tabela_valores` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-10-25 21:51:12
