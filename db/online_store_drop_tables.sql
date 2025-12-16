-- Drop all tables from online_store database in correct order (child tables first)

USE online_store;

-- Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

-- Drop all tables
DROP TABLE IF EXISTS ProductReviews;
DROP TABLE IF EXISTS ProductPromos;
DROP TABLE IF EXISTS ProductInventories;
DROP TABLE IF EXISTS ProductImages;
DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS ProductCategories;
DROP TABLE IF EXISTS CustomerUsers;
DROP TABLE IF EXISTS AdminUsers;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

