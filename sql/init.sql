-- Criar banco apenas se não existir
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EcommerceDb')
BEGIN
    CREATE DATABASE EcommerceDb;
END
GO

USE EcommerceDb;
GO

-- Produtos
IF OBJECT_ID('Product', 'U') IS NULL
BEGIN
    CREATE TABLE Product (
        ProductId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0)
    );
END
GO

-- Carrinhos
IF OBJECT_ID('Cart', 'U') IS NULL
BEGIN
    CREATE TABLE Cart (
        CartId INT PRIMARY KEY IDENTITY(1,1),
        UserId NVARCHAR(50) NOT NULL
    );
END
GO

-- Itens do Carrinho
IF OBJECT_ID('CartItem', 'U') IS NULL
BEGIN
    CREATE TABLE CartItem (
        CartItemId INT PRIMARY KEY IDENTITY(1,1),
        CartId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantidade INT NOT NULL DEFAULT(1) CHECK (Quantidade > 0),
        CONSTRAINT FK_CartItem_Cart FOREIGN KEY (CartId) REFERENCES Cart(CartId),
        CONSTRAINT FK_CartItem_Product FOREIGN KEY (ProductId) REFERENCES Product(ProductId)
    );
END
GO

-- Descontos
IF OBJECT_ID('Discount', 'U') IS NULL
BEGIN
    CREATE TABLE Discount (
        DiscountId INT PRIMARY KEY IDENTITY(1,1),
        CartId INT NOT NULL,
        Percentual DECIMAL(5,2) NOT NULL CHECK (Percentual BETWEEN 0 AND 100),
        CONSTRAINT FK_Discount_Cart FOREIGN KEY (CartId) REFERENCES Cart(CartId)
    );
END
GO