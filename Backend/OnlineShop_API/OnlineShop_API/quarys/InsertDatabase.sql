-- Lägg till kategorier
INSERT INTO Categories (Name) 
VALUES 
    ('Clothing'), 
    ('Accessories'), 
    ('Shoes');
    
    Select * from Categories;
    -- Lägg till subkategorier
INSERT INTO SubCategories (Name, CategoryId) 
VALUES 
    ('Pants', 4),  -- Clothing
    ('T-shirts', 4),  -- Clothing
    ('Belt', 5),  -- Accessories
    ('Hats', 5),  -- Accessories
    ('Sneakers', 6),  -- shoes
    ('Loafers', 6);  -- shoes
    Select * from SubCategories;

-- Lägg till produkter med rätt subcategory-id och ett kort strängvärde för EncodedImage
INSERT INTO Products (Name, Description, Price, SubCategoryId, StockQuantity, EncodedImage)
VALUES 
    ('Brown Belt', 'A stylish brown leather belt.', 29.99, 9, 100, 'image1'),  -- Belt (Accessories)
    ('Ripped jeans', 'Fashionable ripped jeans for a casual look.', 49.99, 7, 200, 'image2'),  -- Pants (Clothing)
    ('Sun Hat', 'A wide-brimmed hat perfect for sunny days.', 15.99, 10, 150, 'image3'),  -- Hats (Accessories)
    ('T-shirt', 'A comfortable cotton T-shirt in various colors.', 19.99, 8, 300, 'image4');  -- T-shirts (Clothing)

-- Visa alla produkter
SELECT * FROM Products;

