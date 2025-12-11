CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Books` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Number` int NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Authors` longtext CHARACTER SET utf8mb4 NULL,
    `Publisher` longtext CHARACTER SET utf8mb4 NULL,
    `PublishYear` int NULL,
    `Categorys` longtext CHARACTER SET utf8mb4 NULL,
    `Introduction` longtext CHARACTER SET utf8mb4 NULL,
    `CoverImageUrl` longtext CHARACTER SET utf8mb4 NULL,
    `Price` decimal(65,30) NOT NULL,
    `Sales` int NOT NULL,
    CONSTRAINT `PK_Books` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Number` int NOT NULL,
    `UserName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserRole` int NOT NULL,
    `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
    `RegistrationDate` datetime(6) NOT NULL,
    `Email` longtext CHARACTER SET utf8mb4 NULL,
    `IsEmailVerified` tinyint(1) NOT NULL,
    `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
    `IsPhoneNumberVerified` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Carts` (
    `UserId` int NOT NULL,
    CONSTRAINT `PK_Carts` PRIMARY KEY (`UserId`),
    CONSTRAINT `FK_Carts_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Orders` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Number` int NOT NULL,
    `UserId` int NOT NULL,
    `OrderStatus` int NOT NULL,
    `PaymentMethod` int NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_Orders` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Orders_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `CartItems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CartId` int NOT NULL,
    `BookId` int NOT NULL,
    `Count` int NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_CartItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CartItems_Books_BookId` FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CartItems_Carts_CartId` FOREIGN KEY (`CartId`) REFERENCES `Carts` (`UserId`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `OrderItems` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Number` int NOT NULL,
    `OrderId` int NOT NULL,
    `BookId` int NOT NULL,
    `Price` decimal(65,30) NULL,
    `Count` int NOT NULL,
    CONSTRAINT `PK_OrderItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_OrderItems_Books_BookId` FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_OrderItems_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Orders` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_CartItems_BookId` ON `CartItems` (`BookId`);

CREATE INDEX `IX_CartItems_CartId` ON `CartItems` (`CartId`);

CREATE INDEX `IX_OrderItems_BookId` ON `OrderItems` (`BookId`);

CREATE INDEX `IX_OrderItems_OrderId` ON `OrderItems` (`OrderId`);

CREATE INDEX `IX_Orders_UserId` ON `Orders` (`UserId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251029112816_UserAddPhoneNumberAndTowVerifyAndChangeOrderStateToOrderStatus', '8.0.20');

COMMIT;

START TRANSACTION;

UPDATE `Books` SET `Publisher` = ''
WHERE `Publisher` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `Books` MODIFY COLUMN `Publisher` longtext CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `Books` MODIFY COLUMN `PublishYear` int NOT NULL DEFAULT 0;

UPDATE `Books` SET `Introduction` = ''
WHERE `Introduction` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `Books` MODIFY COLUMN `Introduction` longtext CHARACTER SET utf8mb4 NOT NULL;

UPDATE `Books` SET `CoverImageUrl` = ''
WHERE `CoverImageUrl` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `Books` MODIFY COLUMN `CoverImageUrl` longtext CHARACTER SET utf8mb4 NOT NULL;

UPDATE `Books` SET `Categorys` = ''
WHERE `Categorys` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `Books` MODIFY COLUMN `Categorys` longtext CHARACTER SET utf8mb4 NOT NULL;

UPDATE `Books` SET `Authors` = ''
WHERE `Authors` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `Books` MODIFY COLUMN `Authors` longtext CHARACTER SET utf8mb4 NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251116082945_BookNoMorewenhao', '8.0.20');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251201112949_AddUserRoleNone', '8.0.20');

COMMIT;

