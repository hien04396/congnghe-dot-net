CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `AdminUsers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Password` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `IsDefault` tinyint(1) NOT NULL,
    CONSTRAINT `PK_AdminUsers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `CustomerUsers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Password` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_CustomerUsers` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProductCategories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_ProductCategories` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Orders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CustomerUserId` int NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `TotalAmount` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Orders_CustomerUsers_CustomerUserId` FOREIGN KEY (`CustomerUserId`) REFERENCES `CustomerUsers` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Products` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `Price` decimal(18,2) NOT NULL,
    `IsActive` tinyint(1) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `CategoryId` int NOT NULL,
    CONSTRAINT `PK_Products` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Products_ProductCategories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `ProductCategories` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `OrderItems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `OrderId` int NOT NULL,
    `ProductId` int NOT NULL,
    `Quantity` int NOT NULL,
    `UnitPrice` decimal(18,2) NOT NULL,
    `LineTotal` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_OrderItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_OrderItems_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProductImages` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ProductId` int NOT NULL,
    `ImageUrl` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `IsPrimary` tinyint(1) NOT NULL,
    CONSTRAINT `PK_ProductImages` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProductImages_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProductInventories` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ProductId` int NOT NULL,
    `StockQuantity` int NOT NULL,
    `LastUpdated` datetime(6) NOT NULL,
    CONSTRAINT `PK_ProductInventories` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProductInventories_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProductPromos` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ProductId` int NOT NULL,
    `AmountOff` decimal(18,2) NOT NULL,
    `StartDate` datetime(6) NULL,
    `EndDate` datetime(6) NULL,
    `IsActive` tinyint(1) NOT NULL,
    CONSTRAINT `PK_ProductPromos` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProductPromos_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProductReviews` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ProductId` int NOT NULL,
    `Title` varchar(200) CHARACTER SET utf8mb4 NULL,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `Rating` int NOT NULL,
    `CreatedByAdminId` int NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_ProductReviews` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProductReviews_AdminUsers_CreatedByAdminId` FOREIGN KEY (`CreatedByAdminId`) REFERENCES `AdminUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ProductReviews_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_OrderItems_OrderId` ON `OrderItems` (`OrderId`);

CREATE INDEX `IX_OrderItems_ProductId` ON `OrderItems` (`ProductId`);

CREATE INDEX `IX_Orders_CustomerUserId` ON `Orders` (`CustomerUserId`);

CREATE INDEX `IX_ProductImages_ProductId` ON `ProductImages` (`ProductId`);

CREATE UNIQUE INDEX `IX_ProductInventories_ProductId` ON `ProductInventories` (`ProductId`);

CREATE INDEX `IX_ProductPromos_ProductId` ON `ProductPromos` (`ProductId`);

CREATE INDEX `IX_ProductReviews_CreatedByAdminId` ON `ProductReviews` (`CreatedByAdminId`);

CREATE INDEX `IX_ProductReviews_ProductId` ON `ProductReviews` (`ProductId`);

CREATE INDEX `IX_Products_CategoryId` ON `Products` (`CategoryId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251215171338_InitialCreate', '9.0.0');

COMMIT;

