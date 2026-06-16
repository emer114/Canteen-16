-- ============================================================
--  CANTEEN MANAGEMENT SYSTEM - DATABASE SETUP
--  Title: Optimizing Canteen Operations Through an
--         Integrated Management System
--  Database: SQL Server (SSMS)
-- ============================================================

-- STEP 1: Create and use the database
CREATE DATABASE CanteenDB;
GO

USE CanteenDB;
GO

-- ============================================================
-- TABLE 1: Users
-- Stores Admin and Cashier accounts
-- ============================================================
CREATE TABLE Users (
    UserID      INT IDENTITY(1,1) PRIMARY KEY,
    FullName    VARCHAR(100)  NOT NULL,
    Username    VARCHAR(50)   NOT NULL UNIQUE,
    Password    VARCHAR(255)  NOT NULL,
    Role        VARCHAR(20)   NOT NULL CHECK (Role IN ('Admin', 'Cashier', 'Kitchen')),
    IsActive    BIT           NOT NULL DEFAULT 1,
    CreatedAt   DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE 2: Categories
-- Groups menu items (e.g., Meals, Drinks, Snacks)
-- ============================================================
CREATE TABLE Categories (
    CategoryID   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL UNIQUE,
    Description  VARCHAR(255)
);
GO

-- ============================================================
-- TABLE 3: MenuItems
-- All available food/drink items in the canteen
-- ============================================================
CREATE TABLE MenuItems (
    ItemID       INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID   INT           NOT NULL,
    ItemName     VARCHAR(100)  NOT NULL,
    Description  VARCHAR(255),
    Price        DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    IsAvailable  BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_MenuItems_Category FOREIGN KEY (CategoryID)
        REFERENCES Categories(CategoryID)
);
GO

-- ============================================================
-- TABLE 4: Inventory
-- Tracks stock for each menu item
-- ============================================================
CREATE TABLE Inventory (
    InventoryID    INT IDENTITY(1,1) PRIMARY KEY,
    ItemID         INT           NOT NULL UNIQUE,
    StockQuantity  INT           NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    LowStockAlert  INT           NOT NULL DEFAULT 5,
    LastUpdated    DATETIME      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Inventory_Item FOREIGN KEY (ItemID)
        REFERENCES MenuItems(ItemID)
);
GO

-- ============================================================
-- TABLE 5: Orders
-- Each order transaction placed by cashier
-- ============================================================
CREATE TABLE Orders (
    OrderID      INT IDENTITY(1,1) PRIMARY KEY,
    CashierID    INT           NOT NULL,
    OrderDate    DATETIME      NOT NULL DEFAULT GETDATE(),
    TotalAmount  DECIMAL(10,2) NOT NULL DEFAULT 0,
    AmountPaid   DECIMAL(10,2),
    Change       DECIMAL(10,2),
    Status       VARCHAR(20)   NOT NULL DEFAULT 'Pending'
                     CHECK (Status IN ('Pending', 'Preparing', 'Ready', 'Completed', 'Cancelled')),
    Notes        VARCHAR(255),

    CONSTRAINT FK_Orders_Cashier FOREIGN KEY (CashierID)
        REFERENCES Users(UserID)
);
GO

-- ============================================================
-- TABLE 6: OrderDetails
-- Line items for each order (what was ordered)
-- ============================================================
CREATE TABLE OrderDetails (
    DetailID    INT IDENTITY(1,1) PRIMARY KEY,
    OrderID     INT           NOT NULL,
    ItemID      INT           NOT NULL,
    Quantity    INT           NOT NULL CHECK (Quantity > 0),
    UnitPrice   DECIMAL(10,2) NOT NULL,
    Subtotal    AS (Quantity * UnitPrice) PERSISTED,

    CONSTRAINT FK_OrderDetails_Order FOREIGN KEY (OrderID)
        REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderDetails_Item  FOREIGN KEY (ItemID)
        REFERENCES MenuItems(ItemID)
);
GO

-- ============================================================
-- TABLE 7: SalesLog
-- Daily sales summary for reporting
-- ============================================================
CREATE TABLE SalesLog (
    LogID        INT IDENTITY(1,1) PRIMARY KEY,
    LogDate      DATE          NOT NULL UNIQUE DEFAULT CAST(GETDATE() AS DATE),
    TotalOrders  INT           NOT NULL DEFAULT 0,
    TotalRevenue DECIMAL(10,2) NOT NULL DEFAULT 0,
    UpdatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE 8: InventoryLog
-- Tracks every stock change (restock, deduction)
-- ============================================================
CREATE TABLE InventoryLog (
    LogID        INT IDENTITY(1,1) PRIMARY KEY,
    ItemID       INT           NOT NULL,
    ChangeType   VARCHAR(20)   NOT NULL CHECK (ChangeType IN ('Restock', 'Deduction', 'Adjustment')),
    QuantityChanged INT        NOT NULL,
    Reason       VARCHAR(255),
    ChangedBy    INT           NOT NULL,
    ChangedAt    DATETIME      NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_InvLog_Item FOREIGN KEY (ItemID)
        REFERENCES MenuItems(ItemID),
    CONSTRAINT FK_InvLog_User FOREIGN KEY (ChangedBy)
        REFERENCES Users(UserID)
);
GO


-- ============================================================
-- VIEWS
-- ============================================================

-- View: Kitchen Display - shows pending/preparing orders
CREATE VIEW vw_KitchenDisplay AS
    SELECT
        o.OrderID,
        o.OrderDate,
        o.Status,
        mi.ItemName,
        od.Quantity,
        od.UnitPrice,
        o.Notes
    FROM Orders o
    JOIN OrderDetails od ON o.OrderID = od.OrderID
    JOIN MenuItems mi    ON od.ItemID  = mi.ItemID
    WHERE o.Status IN ('Pending', 'Preparing')
GO

-- View: Daily Sales Summary
CREATE VIEW vw_DailySales AS
    SELECT
        CAST(o.OrderDate AS DATE)       AS SaleDate,
        COUNT(DISTINCT o.OrderID)       AS TotalOrders,
        SUM(od.Subtotal)                AS TotalRevenue,
        AVG(o.TotalAmount)              AS AvgOrderValue
    FROM Orders o
    JOIN OrderDetails od ON o.OrderID = od.OrderID
    WHERE o.Status = 'Completed'
    GROUP BY CAST(o.OrderDate AS DATE)
GO

-- View: Low Stock Items Alert
CREATE VIEW vw_LowStock AS
    SELECT
        mi.ItemID,
        mi.ItemName,
        c.CategoryName,
        i.StockQuantity,
        i.LowStockAlert
    FROM Inventory i
    JOIN MenuItems mi  ON i.ItemID     = mi.ItemID
    JOIN Categories c  ON mi.CategoryID = c.CategoryID
    WHERE i.StockQuantity <= i.LowStockAlert
      AND mi.IsAvailable = 1
GO

-- View: Full Menu with Stock
CREATE VIEW vw_MenuWithStock AS
    SELECT
        mi.ItemID,
        mi.ItemName,
        c.CategoryName,
        mi.Price,
        mi.IsAvailable,
        i.StockQuantity
    FROM MenuItems mi
    JOIN Categories c  ON mi.CategoryID = c.CategoryID
    LEFT JOIN Inventory i ON mi.ItemID   = i.ItemID
GO


-- ============================================================
-- SAMPLE DATA
-- ============================================================

-- Users
INSERT INTO Users (FullName, Username, Password, Role) VALUES
('System Admin',    'admin',    'admin123',    'Admin'),
('Juan dela Cruz',  'cashier1', 'cash123',     'Cashier'),
('Maria Santos',    'kitchen1', 'kitchen123',  'Kitchen');

-- Categories
INSERT INTO Categories (CategoryName, Description) VALUES
('Lutong Ulam',    'Pang-tanghalian na ulam at kanin'),
('Street Food',    'Kwek-kwek, fishball, at iba pang street food'),
('Meryenda',       'Turon, maruya, at iba pang pang-meryenda'),
('Inumin',         'Malamig at mainit na inumin');

-- Menu Items
-- Lutong Ulam (CategoryID = 1)
INSERT INTO MenuItems (CategoryID, ItemName, Description, Price) VALUES
(1, 'Adobong Manok + Kanin',  'Lutong bahay na adobong manok na may kanin',     35.00),
(1, 'Ginisang Monggo + Kanin','Monggo na may dahon ng ampalaya at kanin',        30.00),
(1, 'Pritong Isda + Kanin',   'Tilapia o galunggong na pinirito na may kanin',   35.00),
(1, 'Menudo + Kanin',         'Baboy na may patatas at karot na may kanin',       40.00),

-- Street Food (CategoryID = 2)
(2, 'Kwek-Kwek',              'Pritong itlog ng pugo na may orange batter',       5.00),
(2, 'Fishball',               'Isang stick ng fishball na may sarsa',             5.00),
(2, 'Kikiam',                 'Isang stick ng kikiam na may sarsa',               5.00),
(2, 'Betamax',                'Dugo ng manok na inihaw na may sarsa',             5.00),
(2, 'Isaw Manok',             'Inihaw na isaw ng manok',                          8.00),
(2, 'Hotdog sa Stick',        'Inihaw na hotdog na may ketsap',                   10.00),

-- Meryenda (CategoryID = 3)
(3, 'Turon',                  'Saging na saba at langka na binalot sa lumpia wrapper', 10.00),
(3, 'Maruya',                 'Pritong saging na may asukal',                     10.00),
(3, 'Puto',                   'Puting puto na may keso sa ibabaw',                 8.00),
(3, 'Kutsinta',               'Kulay kayumanggi na puto na may niyog',             8.00),
(3, 'Palitaw',                'Bilog na kakanin na may niyog at asukal',           8.00),
(3, 'Camote Cue',             'Kamote na may asukal na pinirito sa stick',         10.00),
(3, 'Banana Cue',             'Saging na may asukal na pinirito sa stick',         10.00),

-- Inumin (CategoryID = 4)
(4, 'Softdrinks (Bote)',       'Coke, Royal, o Sprite na 250ml',                  20.00),
(4, 'Mineral Water',           '500ml na purified water',                          15.00),
(4, 'Sago at Gulaman',         'Malamig na sago gulaman na may brown sugar',       15.00),
(4, 'Milo (Mainit)',           'Mainit na Milo sa baso',                           15.00),
(4, 'Buko Juice',              'Sariwang buko juice',                              20.00);

-- Inventory
INSERT INTO Inventory (ItemID, StockQuantity, LowStockAlert) VALUES
(1,  50, 10),   -- Adobong Manok
(2,  50, 10),   -- Ginisang Monggo
(3,  40, 10),   -- Pritong Isda
(4,  40, 10),   -- Menudo
(5, 100, 20),   -- Kwek-Kwek
(6, 150, 30),   -- Fishball
(7, 120, 25),   -- Kikiam
(8,  80, 15),   -- Betamax
(9,  80, 15),   -- Isaw Manok
(10, 60, 10),   -- Hotdog sa Stick
(11, 80, 15),   -- Turon
(12, 80, 15),   -- Maruya
(13,100, 20),   -- Puto
(14,100, 20),   -- Kutsinta
(15, 80, 15),   -- Palitaw
(16, 70, 15),   -- Camote Cue
(17, 70, 15),   -- Banana Cue
(18,100, 20),   -- Softdrinks
(19,150, 30),   -- Mineral Water
(20,100, 20),   -- Sago at Gulaman
(21, 80, 15),   -- Milo
(22, 60, 10);   -- Buko Juice

-- Sample Order 1: Isang customer na nag-order ng ulam at street food
INSERT INTO Orders (CashierID, TotalAmount, AmountPaid, Change, Status)
VALUES (2, 60.00, 100.00, 40.00, 'Completed');

INSERT INTO OrderDetails (OrderID, ItemID, Quantity, UnitPrice) VALUES
(1, 1,  1, 35.00),   -- Adobong Manok + Kanin
(1, 5,  3,  5.00),   -- Kwek-Kwek x3
(1, 19, 1, 15.00);   -- Mineral Water

-- Sample Order 2: Meryenda lang
INSERT INTO Orders (CashierID, TotalAmount, AmountPaid, Change, Status)
VALUES (2, 45.00, 50.00, 5.00, 'Completed');

INSERT INTO OrderDetails (OrderID, ItemID, Quantity, UnitPrice) VALUES
(2, 11, 2, 10.00),   -- Turon x2
(2, 12, 1, 10.00),   -- Maruya x1
(2, 20, 1, 15.00);   -- Sago at Gulaman

GO

-- ============================================================
-- VERIFY ALL TABLES
-- ============================================================
SELECT 'Users'         AS TableName, COUNT(*) AS Rows FROM Users         UNION ALL
SELECT 'Categories',                 COUNT(*)          FROM Categories     UNION ALL
SELECT 'MenuItems',                  COUNT(*)          FROM MenuItems      UNION ALL
SELECT 'Inventory',                  COUNT(*)          FROM Inventory      UNION ALL
SELECT 'Orders',                     COUNT(*)          FROM Orders         UNION ALL
SELECT 'OrderDetails',               COUNT(*)          FROM OrderDetails   UNION ALL
SELECT 'SalesLog',                   COUNT(*)          FROM SalesLog       UNION ALL
SELECT 'InventoryLog',               COUNT(*)          FROM InventoryLog;
GO

-- ============================================================
-- USEFUL QUERIES FOR YOUR PROJECT
-- ============================================================

-- 1. View full menu with stock
SELECT * FROM vw_MenuWithStock;

-- 2. Kitchen: View all pending orders
SELECT * FROM vw_KitchenDisplay ORDER BY OrderDate;

-- 3. Admin: Daily sales report
SELECT * FROM vw_DailySales ORDER BY SaleDate DESC;

-- 4. Admin: Low stock alert
SELECT * FROM vw_LowStock;

-- 5. Order history with cashier name
SELECT
    o.OrderID,
    u.FullName   AS Cashier,
    o.OrderDate,
    o.TotalAmount,
    o.AmountPaid,
    o.Change,
    o.Status
FROM Orders o
JOIN Users u ON o.CashierID = u.UserID
ORDER BY o.OrderDate DESC;