-- Seed data for online_store database

USE online_store;

INSERT INTO ProductCategories (Name, Description, CreatedAt) VALUES
('Áo', 'Áo thun, áo sơ mi, áo kiểu ', NOW()),
('Quần', 'Jean, quần tây, quần short', NOW()),
('Áo khoác', 'Áo khoác, áo vest, áo bomber', NOW()),
('Đầm', 'Đầm, đầm váy, đầm ngắn', NOW()),
('Chân váy', 'Chân váy, chân váy ngắn, chân váy dài', NOW());

INSERT INTO AdminUsers (Username, Password, IsDefault) VALUES
('admin', 'admin', 1),
('manager', 'manager123', 0);

INSERT INTO CustomerUsers (Username, Password, CreatedAt) VALUES
('alice', 'alice123', NOW()),
('bob', 'bob123', NOW());

-- Shirts (Category 1) - 25 products
INSERT INTO Products (Name, Description, Price, IsActive, IsFeatured, CreatedAt, CategoryId, ProductImageUrl) VALUES
('Faux leather top', 'High neck, sleeveless top with defined seams and a back zip fastening.', 19.99, 1, 0, NOW(), 1, 'https://image.uniqlo.com/UQ/ST3/vn/imagesgoods/469871/item/vngoods_06_469871_3x4.jpg?width=423'),
('ÁShimmer-efect rufled top', 'Top with a round neck and sleeveless design.', 24.99, 1, 0, NOW(), 1),
('Striped Long Sleeve', 'Comfortable striped shirt with long sleeves.', 27.99, 1, 0, NOW(), 1),
('Polo Shirt Navy', 'Classic polo shirt in navy blue.', 22.99, 1, 0, NOW(), 1),
('Oxford Button Down', 'Premium oxford cotton shirt, perfect for business.', 34.99, 1, 0, NOW(), 1),
('Flannel Plaid Shirt', 'Warm flannel shirt with plaid pattern.', 29.99, 1, 0, NOW(), 1),
('Denim Shirt', 'Casual denim shirt for everyday style.', 26.99, 1, 0, NOW(), 1),
('Linen Summer Shirt', 'Lightweight linen shirt for hot weather.', 31.99, 1, 0, NOW(), 1),
('Checkered Shirt', 'Classic checkered pattern shirt.', 23.99, 1, 0, NOW(), 1),
('T-Shirt White', 'Basic white t-shirt, essential wardrobe piece.', 12.99, 1, 0, NOW(), 1),
('T-Shirt Black', 'Classic black t-shirt.', 12.99, 1, 0, NOW(), 1),
('T-Shirt Gray', 'Comfortable gray t-shirt.', 12.99, 1, 0, NOW(), 1),
('Henley Shirt', 'Casual henley shirt with buttons.', 19.99, 1, 0, NOW(), 1),
('V-Neck T-Shirt', 'Simple v-neck t-shirt in various colors.', 14.99, 1, 0, NOW(), 1),
('Long Sleeve Tee', 'Long sleeve t-shirt for cooler days.', 18.99, 1, 0, NOW(), 1),
('Crew Neck Sweatshirt', 'Comfortable crew neck sweatshirt.', 32.99, 1, 0, NOW(), 1),
('Hooded Sweatshirt', 'Warm hooded sweatshirt.', 35.99, 1, 0, NOW(), 1),
('Tank Top White', 'Simple white tank top.', 9.99, 1, 0, NOW(), 1),
('Tank Top Black', 'Classic black tank top.', 9.99, 1, 0, NOW(), 1),
('Graphic Tee', 'Fun graphic print t-shirt.', 16.99, 1, 0, NOW(), 1),
('Raglan Sleeve Shirt', 'Sporty raglan sleeve design.', 21.99, 1, 0, NOW(), 1),
('Turtleneck Sweater', 'Warm turtleneck sweater shirt.', 39.99, 1, 0, NOW(), 1),
('Cardigan Shirt', 'Button-up cardigan style shirt.', 42.99, 1, 0, NOW(), 1),
('Chambray Shirt', 'Lightweight chambray fabric shirt.', 28.99, 1, 0, NOW(), 1),
('Work Shirt', 'Durable work shirt for tough jobs.', 24.99, 1, 0, NOW(), 1);

-- Pants (Category 2) - 25 products
INSERT INTO Products (Name, Description, Price, IsActive, IsFeatured, CreatedAt, CategoryId) VALUES
('Blue Denim Jeans', 'Classic straight fit denim jeans.', 39.99, 1, 0, NOW(), 2),
('Black Skinny Jeans', 'Slim fit black denim jeans.', 42.99, 1, 0, NOW(), 2),
('Khaki Chinos', 'Classic khaki chino pants.', 34.99, 1, 0, NOW(), 2),
('Navy Chinos', 'Versatile navy chino pants.', 34.99, 1, 0, NOW(), 2),
('Gray Chinos', 'Comfortable gray chino pants.', 34.99, 1, 0, NOW(), 2),
('Cargo Pants', 'Practical cargo pants with pockets.', 38.99, 1, 0, NOW(), 2),
('Sweatpants Gray', 'Comfortable gray sweatpants.', 29.99, 1, 0, NOW(), 2),
('Sweatpants Black', 'Classic black sweatpants.', 29.99, 1, 0, NOW(), 2),
('Joggers', 'Athletic style jogger pants.', 32.99, 1, 0, NOW(), 2),
('Wide Leg Pants', 'Trendy wide leg pants.', 36.99, 1, 0, NOW(), 2),
('Cropped Pants', 'Modern cropped length pants.', 33.99, 1, 0, NOW(), 2),
('Corduroy Pants', 'Classic corduroy fabric pants.', 37.99, 1, 0, NOW(), 2),
('Linen Pants', 'Lightweight linen pants for summer.', 35.99, 1, 0, NOW(), 2),
('Wool Trousers', 'Professional wool trousers.', 49.99, 1, 0, NOW(), 2),
('Leggings Black', 'Comfortable black leggings.', 19.99, 1, 0, NOW(), 2),
('Leggings Gray', 'Versatile gray leggings.', 19.99, 1, 0, NOW(), 2),
('Yoga Pants', 'Stretchy yoga pants for active wear.', 24.99, 1, 0, NOW(), 2),
('Track Pants', 'Athletic track pants.', 27.99, 1, 0, NOW(), 2),
('Harem Pants', 'Comfortable harem style pants.', 31.99, 1, 0, NOW(), 2),
('Palazzo Pants', 'Flowy palazzo pants.', 33.99, 1, 0, NOW(), 2),
('Culottes', 'Modern culotte style pants.', 32.99, 1, 0, NOW(), 2),
('High Waist Pants', 'Flattering high waist design.', 36.99, 1, 0, NOW(), 2),
('Ankle Pants', 'Stylish ankle length pants.', 34.99, 1, 0, NOW(), 2),
('Straight Leg Jeans', 'Classic straight leg denim.', 39.99, 1, 0, NOW(), 2),
('Bootcut Jeans', 'Timeless bootcut style jeans.', 41.99, 1, 0, NOW(), 2);

-- Jackets (Category 3) - 25 products
INSERT INTO Products (Name, Description, Price, IsActive, IsFeatured, CreatedAt, CategoryId) VALUES
('Black Hoodie Jacket', 'Comfortable hoodie jacket for cool days.', 49.99, 1, 0, NOW(), 3),
('Denim Jacket', 'Classic denim jacket for all seasons.', 44.99, 1, 0, NOW(), 3),
('Leather Jacket', 'Stylish black leather jacket.', 89.99, 1, 0, NOW(), 3),
('Bomber Jacket', 'Trendy bomber style jacket.', 54.99, 1, 0, NOW(), 3),
('Windbreaker', 'Lightweight windbreaker jacket.', 39.99, 1, 0, NOW(), 3),
('Rain Jacket', 'Waterproof rain jacket.', 45.99, 1, 0, NOW(), 3),
('Puffer Jacket', 'Warm puffer jacket for winter.', 69.99, 1, 0, NOW(), 3),
('Trench Coat', 'Classic trench coat.', 79.99, 1, 0, NOW(), 3),
('Blazer Navy', 'Professional navy blazer.', 64.99, 1, 0, NOW(), 3),
('Blazer Black', 'Elegant black blazer.', 64.99, 1, 0, NOW(), 3),
('Cardigan', 'Comfortable cardigan sweater.', 42.99, 1, 0, NOW(), 3),
('Fleece Jacket', 'Soft fleece jacket.', 38.99, 1, 0, NOW(), 3),
('Quilted Jacket', 'Warm quilted design jacket.', 52.99, 1, 0, NOW(), 3),
('Parka', 'Long parka jacket for cold weather.', 74.99, 1, 0, NOW(), 3),
('Jean Jacket', 'Casual jean jacket.', 43.99, 1, 0, NOW(), 3),
('Track Jacket', 'Sporty track jacket.', 34.99, 1, 0, NOW(), 3),
('Varsity Jacket', 'Classic varsity style jacket.', 59.99, 1, 0, NOW(), 3),
('Sherpa Jacket', 'Cozy sherpa lined jacket.', 56.99, 1, 0, NOW(), 3),
('Utility Jacket', 'Practical utility style jacket.', 48.99, 1, 0, NOW(), 3),
('Cropped Jacket', 'Modern cropped length jacket.', 46.99, 1, 0, NOW(), 3),
('Oversized Jacket', 'Trendy oversized fit jacket.', 51.99, 1, 0, NOW(), 3),
('Wool Coat', 'Warm wool winter coat.', 84.99, 1, 0, NOW(), 3),
('Peacoat', 'Classic navy peacoat.', 69.99, 1, 0, NOW(), 3),
('Anorak', 'Lightweight anorak jacket.', 47.99, 1, 0, NOW(), 3),
('Field Jacket', 'Durable field jacket.', 55.99, 1, 0, NOW(), 3);

-- Dresses (Category 4) - 25 products
INSERT INTO Products (Name, Description, Price, IsActive, IsFeatured, CreatedAt, CategoryId) VALUES
('Summer Floral Dress', 'Light dress for summer days.', 59.99, 1, 0, NOW(), 4),
('Little Black Dress', 'Classic little black dress.', 64.99, 1, 0, NOW(), 4),
('Maxi Dress', 'Elegant long maxi dress.', 69.99, 1, 0, NOW(), 4),
('Midi Dress', 'Versatile midi length dress.', 54.99, 1, 0, NOW(), 4),
('Mini Dress', 'Fun short mini dress.', 49.99, 1, 0, NOW(), 4),
('Wrap Dress', 'Flattering wrap style dress.', 59.99, 1, 0, NOW(), 4),
('Shift Dress', 'Simple shift dress design.', 44.99, 1, 0, NOW(), 4),
('A-Line Dress', 'Classic A-line silhouette.', 52.99, 1, 0, NOW(), 4),
('Bodycon Dress', 'Fitted bodycon style dress.', 47.99, 1, 0, NOW(), 4),
('Sundress', 'Light and airy sundress.', 39.99, 1, 0, NOW(), 4),
('Cocktail Dress', 'Elegant cocktail party dress.', 74.99, 1, 0, NOW(), 4),
('Casual Day Dress', 'Comfortable everyday dress.', 42.99, 1, 0, NOW(), 4),
('Office Dress', 'Professional office appropriate dress.', 54.99, 1, 0, NOW(), 4),
('Floral Print Dress', 'Beautiful floral pattern dress.', 57.99, 1, 0, NOW(), 4),
('Striped Dress', 'Classic striped pattern dress.', 49.99, 1, 0, NOW(), 4),
('Polka Dot Dress', 'Playful polka dot dress.', 52.99, 1, 0, NOW(), 4),
('Lace Dress', 'Delicate lace detail dress.', 64.99, 1, 0, NOW(), 4),
('Denim Dress', 'Casual denim dress.', 47.99, 1, 0, NOW(), 4),
('T-Shirt Dress', 'Comfortable t-shirt style dress.', 34.99, 1, 0, NOW(), 4),
('Shirt Dress', 'Button-up shirt style dress.', 44.99, 1, 0, NOW(), 4),
('Smock Dress', 'Relaxed smock style dress.', 49.99, 1, 0, NOW(), 4),
('Tiered Dress', 'Layered tiered design dress.', 59.99, 1, 0, NOW(), 4),
('Ruffled Dress', 'Feminine ruffled detail dress.', 54.99, 1, 0, NOW(), 4),
('Off-Shoulder Dress', 'Trendy off-shoulder style.', 57.99, 1, 0, NOW(), 4),
('V-Neck Dress', 'Elegant v-neck design dress.', 52.99, 1, 0, NOW(), 4);

-- Skirts (Category 5) - 25 products
INSERT INTO Products (Name, Description, Price, IsActive, IsFeatured, CreatedAt, CategoryId) VALUES
('Pleated Skirt', 'Pleated skirt for everyday outfits.', 29.99, 1, 0, NOW(), 5),
('A-Line Skirt', 'Classic A-line skirt.', 32.99, 1, 0, NOW(), 5),
('Pencil Skirt', 'Professional pencil skirt.', 34.99, 1, 0, NOW(), 5),
('Mini Skirt', 'Short mini skirt.', 26.99, 1, 0, NOW(), 5),
('Midi Skirt', 'Versatile midi length skirt.', 31.99, 1, 0, NOW(), 5),
('Maxi Skirt', 'Long flowing maxi skirt.', 36.99, 1, 0, NOW(), 5),
('Wrap Skirt', 'Adjustable wrap style skirt.', 33.99, 1, 0, NOW(), 5),
('Denim Skirt', 'Casual denim skirt.', 28.99, 1, 0, NOW(), 5),
('Leather Skirt', 'Stylish leather skirt.', 44.99, 1, 0, NOW(), 5),
('Floral Skirt', 'Beautiful floral print skirt.', 35.99, 1, 0, NOW(), 5),
('Striped Skirt', 'Classic striped pattern skirt.', 32.99, 1, 0, NOW(), 5),
('Plaid Skirt', 'Preppy plaid design skirt.', 34.99, 1, 0, NOW(), 5),
('Tulle Skirt', 'Playful tulle fabric skirt.', 39.99, 1, 0, NOW(), 5),
('Tiered Skirt', 'Layered tiered design skirt.', 37.99, 1, 0, NOW(), 5),
('Circle Skirt', 'Full circle style skirt.', 35.99, 1, 0, NOW(), 5),
('High Waist Skirt', 'Flattering high waist design.', 33.99, 1, 0, NOW(), 5),
('Asymmetric Skirt', 'Modern asymmetric hem skirt.', 38.99, 1, 0, NOW(), 5),
('Ruffled Skirt', 'Feminine ruffled detail skirt.', 36.99, 1, 0, NOW(), 5),
('Bubble Skirt', 'Trendy bubble style skirt.', 39.99, 1, 0, NOW(), 5),
('Cargo Skirt', 'Practical cargo style skirt.', 31.99, 1, 0, NOW(), 5),
('Tennis Skirt', 'Sporty tennis style skirt.', 29.99, 1, 0, NOW(), 5),
('Lace Skirt', 'Delicate lace detail skirt.', 41.99, 1, 0, NOW(), 5),
('Chiffon Skirt', 'Lightweight chiffon skirt.', 34.99, 1, 0, NOW(), 5),
('Corduroy Skirt', 'Classic corduroy fabric skirt.', 32.99, 1, 0, NOW(), 5),
('Velvet Skirt', 'Luxurious velvet skirt.', 42.99, 1, 0, NOW(), 5);

-- Inventory: Random stock between 5-50 for each product
INSERT INTO ProductInventories (ProductId, StockQuantity, LastUpdated)
SELECT Id, FLOOR(5 + RAND() * 46), NOW() FROM Products;

-- Images: 3 images per product with the provided URL
-- First image is primary (IsPrimary = 1), others are 0
INSERT INTO ProductImages (ProductId, ImageUrl, IsPrimary)
SELECT Id, 'https://image.uniqlo.com/UQ/ST3/vn/imagesgoods/469871/item/vngoods_06_469871_3x4.jpg?width=423', 1 FROM Products;

INSERT INTO ProductImages (ProductId, ImageUrl, IsPrimary)
SELECT Id, 'https://image.uniqlo.com/UQ/ST3/vn/imagesgoods/469871/item/vngoods_06_469871_3x4.jpg?width=423', 0 FROM Products;

INSERT INTO ProductImages (ProductId, ImageUrl, IsPrimary)
SELECT Id, 'https://image.uniqlo.com/UQ/ST3/vn/imagesgoods/469871/item/vngoods_06_469871_3x4.jpg?width=423', 0 FROM Products;

UPDATE ProductImages SET ImageUrl = 'https://static.zara.net/assets/public/6786/1f31/66214c018532/2c72e8c8a398/02180409808-a3/02180409808-a3.jpg?ts=1748875164825&w=378' 
WHERE ProductId = (SELECT Id from Products where Name = 'Mini Skirt') AND IsPrimary = 1;

-- Reviews: 5-10 reviews per product with varying ratings (1-5 stars)
-- Create a temporary table to help generate multiple reviews per product
CREATE TEMPORARY TABLE IF NOT EXISTS temp_numbers (n INT);
INSERT INTO temp_numbers VALUES (1), (2), (3), (4), (5), (6), (7), (8), (9), (10);

-- Generate reviews: First 50 products get 5 reviews each, next 50 get 7 reviews each, remaining get 10 reviews each
-- This gives us variation in review count per product

-- Reviews for first 50 products (5 reviews each = 250 reviews)
INSERT INTO ProductReviews (ProductId, Title, Content, Rating, CreatedByAdminId, CreatedAt)
SELECT 
    p.Id,
    ELT(1 + FLOOR(RAND() * 10), 
        'Great product!', 'Very satisfied', 'Good quality', 'Love it!', 'Highly recommend',
        'Nice fit', 'Good value', 'Perfect for everyday', 'Comfortable', 'Excellent purchase'),
    ELT(1 + FLOOR(RAND() * 8),
        'Very comfortable and easy to match with other clothes.',
        'Good quality for the price. Would buy again.',
        'Nice fit and comfortable to wear all day.',
        'Great addition to my wardrobe. Highly satisfied.',
        'Perfect for my needs. Good value for money.',
        'Love the style and quality. Fast shipping too.',
        'Exactly as described. Very happy with purchase.',
        'Great product, would recommend to others.'),
    FLOOR(1 + RAND() * 5),
    (SELECT Id FROM AdminUsers ORDER BY RAND() LIMIT 1),
    DATE_SUB(NOW(), INTERVAL FLOOR(RAND() * 30) DAY)
FROM (SELECT Id FROM Products ORDER BY Id LIMIT 50) p
CROSS JOIN (SELECT n FROM temp_numbers WHERE n <= 5) t;

-- Reviews for next 50 products (7 reviews each = 350 reviews)
INSERT INTO ProductReviews (ProductId, Title, Content, Rating, CreatedByAdminId, CreatedAt)
SELECT 
    p.Id,
    ELT(1 + FLOOR(RAND() * 10), 
        'Great product!', 'Very satisfied', 'Good quality', 'Love it!', 'Highly recommend',
        'Nice fit', 'Good value', 'Perfect for everyday', 'Comfortable', 'Excellent purchase'),
    ELT(1 + FLOOR(RAND() * 8),
        'Very comfortable and easy to match with other clothes.',
        'Good quality for the price. Would buy again.',
        'Nice fit and comfortable to wear all day.',
        'Great addition to my wardrobe. Highly satisfied.',
        'Perfect for my needs. Good value for money.',
        'Love the style and quality. Fast shipping too.',
        'Exactly as described. Very happy with purchase.',
        'Great product, would recommend to others.'),
    FLOOR(1 + RAND() * 5),
    (SELECT Id FROM AdminUsers ORDER BY RAND() LIMIT 1),
    DATE_SUB(NOW(), INTERVAL FLOOR(RAND() * 30) DAY)
FROM (SELECT Id FROM Products ORDER BY Id LIMIT 50 OFFSET 50) p
CROSS JOIN (SELECT n FROM temp_numbers WHERE n <= 7) t;

-- Reviews for remaining 25 products (10 reviews each = 250 reviews)
INSERT INTO ProductReviews (ProductId, Title, Content, Rating, CreatedByAdminId, CreatedAt)
SELECT 
    p.Id,
    ELT(1 + FLOOR(RAND() * 10), 
        'Great product!', 'Very satisfied', 'Good quality', 'Love it!', 'Highly recommend',
        'Nice fit', 'Good value', 'Perfect for everyday', 'Comfortable', 'Excellent purchase'),
    ELT(1 + FLOOR(RAND() * 8),
        'Very comfortable and easy to match with other clothes.',
        'Good quality for the price. Would buy again.',
        'Nice fit and comfortable to wear all day.',
        'Great addition to my wardrobe. Highly satisfied.',
        'Perfect for my needs. Good value for money.',
        'Love the style and quality. Fast shipping too.',
        'Exactly as described. Very happy with purchase.',
        'Great product, would recommend to others.'),
    FLOOR(1 + RAND() * 5),
    (SELECT Id FROM AdminUsers ORDER BY RAND() LIMIT 1),
    DATE_SUB(NOW(), INTERVAL FLOOR(RAND() * 30) DAY)
FROM (SELECT Id FROM Products ORDER BY Id LIMIT 25 OFFSET 100) p
CROSS JOIN (SELECT n FROM temp_numbers WHERE n <= 10) t;

DROP TEMPORARY TABLE IF EXISTS temp_numbers;

-- Mark 4 products as featured (1 from each of the first 4 categories)
SET SQL_SAFE_UPDATES = 0;
UPDATE Products SET IsFeatured = 1 WHERE Name = 'Basic White Shirt';      -- Shirts (Category 1)
UPDATE Products SET IsFeatured = 1 WHERE Name = 'Blue Denim Jeans';        -- Pants (Category 2)
UPDATE Products SET IsFeatured = 1 WHERE Name = 'Black Hoodie Jacket';     -- Jackets (Category 3)
UPDATE Products SET IsFeatured = 1 WHERE Name = 'Summer Floral Dress';     -- Dresses (Category 4)
SET SQL_SAFE_UPDATES = 1;

-- Sample promos (amount off) for some products
INSERT INTO ProductPromos (ProductId, AmountOff, StartDate, EndDate, IsActive) VALUES
((SELECT Id FROM Products WHERE Name = 'Basic White Shirt'), 5.00, NOW(), DATE_ADD(NOW(), INTERVAL 30 DAY), 1),
((SELECT Id FROM Products WHERE Name = 'Blue Denim Jeans'), 7.50, NOW(), DATE_ADD(NOW(), INTERVAL 15 DAY), 1),
((SELECT Id FROM Products WHERE Name = 'Black Hoodie Jacket'), 10.00, NOW(), DATE_ADD(NOW(), INTERVAL 20 DAY), 1),
((SELECT Id FROM Products WHERE Name = 'Little Black Dress'), 12.00, NOW(), DATE_ADD(NOW(), INTERVAL 25 DAY), 1),
((SELECT Id FROM Products WHERE Name = 'Pleated Skirt'), 5.00, NOW(), DATE_ADD(NOW(), INTERVAL 10 DAY), 1);
