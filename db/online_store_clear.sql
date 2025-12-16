USE online_store;

-- Disable safe update mode
SET SQL_SAFE_UPDATES = 0;

-- Delete in reverse dependency order
DELETE FROM ProductReviews;
DELETE FROM ProductPromos;
DELETE FROM ProductInventories;
DELETE FROM ProductImages;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Products;
DELETE FROM ProductCategories;
DELETE FROM CustomerUsers;
DELETE FROM AdminUsers;

-- Re-enable safe update mode
SET SQL_SAFE_UPDATES = 1;

-- Reset auto-increment counters
ALTER TABLE ProductReviews AUTO_INCREMENT = 1;
ALTER TABLE ProductPromos AUTO_INCREMENT = 1;
ALTER TABLE ProductInventories AUTO_INCREMENT = 1;
ALTER TABLE ProductImages AUTO_INCREMENT = 1;
ALTER TABLE OrderItems AUTO_INCREMENT = 1;
ALTER TABLE Orders AUTO_INCREMENT = 1;
ALTER TABLE Products AUTO_INCREMENT = 1;
ALTER TABLE ProductCategories AUTO_INCREMENT = 1;
ALTER TABLE CustomerUsers AUTO_INCREMENT = 1;
ALTER TABLE AdminUsers AUTO_INCREMENT = 1;