INSERT INTO Products (Name, Description, Price, EncodedImage, SubCategoryId, StockQuantity)
VALUES ('T-shirt', 'A comfortable cotton T-shirt for daily wear', 199.00, 'encoded_string_here', 1, 50);
select * from products

-- Steg 1: Lägg till en ny order i Orders-tabellen
INSERT INTO Orders (UserId, TotalAmount, OrderDate, Status)
OUTPUT INSERTED.Id AS OrderId -- Hämta det genererade OrderId
VALUES (11, 100.00, GETDATE(), 0); -- '0' representerar 'Pending' status (förväntas vara ett integervärde för Status)

-- Steg 2: Hämta det genererade OrderId
DECLARE @OrderId INT;
SELECT @OrderId = SCOPE_IDENTITY(); -- Hämta det genererade OrderId

-- Steg 3: Lägg till orderartiklar för den nya ordern
INSERT INTO OrderItems (OrderId, ProductId, Quantity)
VALUES
    (@OrderId, 10, 2),  -- Lägg till produkt med ID 10 och kvantitet 2
    (@OrderId, 11, 3),  -- Lägg till produkt med ID 11 och kvantitet 3
    (@OrderId, 12, 1);  -- Lägg till produkt med ID 12 och kvantitet 1

-- Steg 4: Bekräfta att ordern och orderartiklar har lagts till
-- Visa den nyligen skapade ordern
SELECT * FROM Orders WHERE Id = @OrderId;

-- Visa de nyligen skapade orderartiklarna
SELECT * FROM OrderItems WHERE OrderId = @OrderId;
