-- Seed data for online_store database

USE online_store;

INSERT INTO ProductCategories (Name, Description, CreatedAt) VALUES
('Shirts', 'Casual and formal shirts', NOW()),
('Pants', 'Jeans, chinos and more', NOW()),
('Jackets', 'Light and warm jackets', NOW()),
('Dresses', 'Casual and party dresses', NOW()),
('Skirts', 'Skirts for all styles', NOW());

INSERT INTO AdminUsers (Username, Password, IsDefault) VALUES
('admin', 'admin', 1),
('manager', 'manager123', 0);

INSERT INTO CustomerUsers (Username, Password, CreatedAt) VALUES
('alice', 'alice123', NOW()),
('bob', 'bob123', NOW());

-- Sample products
INSERT INTO Products (Name, Description, Price, IsActive, CreatedAt, CategoryId) VALUES
('Basic White Shirt', 'Simple cotton shirt, perfect for everyday wear.', 19.99, 1, NOW(), 1),
('Blue Denim Jeans', 'Classic straight fit denim jeans.', 39.99, 1, NOW(), 2),
('Black Hoodie Jacket', 'Comfortable hoodie jacket for cool days.', 49.99, 1, NOW(), 3),
('Summer Floral Dress', 'Light dress for summer days.', 59.99, 1, NOW(), 4),
('Pleated Skirt', 'Pleated skirt for everyday outfits.', 29.99, 1, NOW(), 5);

-- Basic inventory: 10 of each
INSERT INTO ProductInventories (ProductId, StockQuantity, LastUpdated)
SELECT Id, 10, NOW() FROM Products;

-- Simple primary image placeholder for each product
INSERT INTO ProductImages (ProductId, ImageUrl, IsPrimary)
SELECT Id, '/images/sample-placeholder.png', 1 FROM Products;

-- Sample promos (amount off)
INSERT INTO ProductPromos (ProductId, AmountOff, StartDate, EndDate, IsActive) VALUES
((SELECT Id FROM Products WHERE Name = 'Basic White Shirt'), 5.00, NOW(), DATE_ADD(NOW(), INTERVAL 30 DAY), 1),
((SELECT Id FROM Products WHERE Name = 'Blue Denim Jeans'), 7.50, NOW(), DATE_ADD(NOW(), INTERVAL 15 DAY), 1);

-- Fake admin reviews
INSERT INTO ProductReviews (ProductId, Title, Content, Rating, CreatedByAdminId, CreatedAt) VALUES
((SELECT Id FROM Products WHERE Name = 'Basic White Shirt'), 'Great everyday shirt', 'Very comfortable and easy to match.', 5, 1, NOW()),
((SELECT Id FROM Products WHERE Name = 'Blue Denim Jeans'), 'Good value', 'Nice fit and quality for the price.', 4, 1, NOW()),
((SELECT Id FROM Products WHERE Name = 'Black Hoodie Jacket'), 'Warm and cozy', 'Perfect for chilly evenings.', 5, 2, NOW());


