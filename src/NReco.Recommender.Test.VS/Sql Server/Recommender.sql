
IF NOT EXISTS(SELECT 1
				FROM sys.databases
				WHERE name = 'Recommender')
BEGIN
    CREATE DATABASE Recommender
END
ELSE
BEGIN
    DROP DATABASE Recommender
END
GO

USE Recommender
GO

IF NOT EXISTS(SELECT *
				FROM sys.objects
				WHERE object_id = OBJECT_ID('Customer')
					AND type = 'U')
BEGIN
	CREATE TABLE Customer
	(
		SysNo INT PRIMARY KEY IDENTITY(1,1),
		CustomerName VARCHAR(10),
		CellPhone VARCHAR(15)
	)
END
GO

IF NOT EXISTS(SELECT 1
				FROM sys.objects
				WHERE object_id = OBJECT_ID('Product')
					AND type = 'U')
BEGIN
	CREATE TABLE Product
	(
		SysNo INT PRIMARY KEY IDENTITY(1,1),
		ProductID VARCHAR(64)
	)
END
GO

IF NOT EXISTS(SELECT 1
                FROM sys.objects
                WHERE object_id = OBJECT_ID('ItemRate')
                    AND type = 'U')
BEGIN
    CREATE TABLE ItemRate
    (
        SysNo BIGINT PRIMARY KEY IDENTITY(1,1),
        Click DECIMAL(15,3),
        Buy DECIMAL(15,3),
        Comment DECIMAL(15,3),
    )
END
GO

IF NOT EXISTS(SELECT 1
				FROM sys.objects
				WHERE object_id = OBJECT_ID('ProductClickFrequency')
					AND type = 'U')
BEGIN
	CREATE TABLE ProductClickFrequency
	(
		SysNo BIGINT PRIMARY KEY IDENTITY(1,1),
		CustomerSysNo INT,
		ProductSysNo INT,
		Frequency INT,
		Timespan BIGINT
	)
END
GO

IF NOT EXISTS(SELECT 1
				FROM sys.objects
				WHERE object_id = OBJECT_ID('ProductBuyFrequency')
					AND type = 'U')
BEGIN
	CREATE TABLE ProductBuyFrequency
	(
		SysNo BIGINT PRIMARY KEY IDENTITY(1,1),
		CustomerSysNo INT,
		ProductSysNo INT,
		Frequency INT,
		Timespan BIGINT
	)
END
GO

IF NOT EXISTS(SELECT 1
				FROM sys.objects
				WHERE object_id = OBJECT_ID('ProductComment')
					AND type = 'U')
BEGIN
	CREATE TABLE ProductComment
	(
		SysNo BIGINT PRIMARY KEY IDENTITY(1,1),
		CustomerSysNo INT,
		ProductSysNo INT,
		Frequency INT,
		Timespan BIGINT
	)
END
GO

--init ItemRate table
INSERT INTO ItemRate(Click, Buy, Comment) VALUES(0.1, 0.6, 0.3)
GO

--init Customer table
DECLARE @customerCount INT = 2000
DECLARE @index INT = 1
DECLARE @name VARCHAR(10)
WHILE(@index <= @customerCount)
BEGIN
	SET @name = 'LCK' + CAST(@index AS VARCHAR)
	INSERT INTO dbo.Customer( CustomerName, CellPhone ) VALUES(@name, @index)
	SET @index = @index + 1
END
GO

--init Product table
DECLARE @index INT = 1
DECLARE @productCount INT =10000
WHILE(@index <= @productCount)
BEGIN
	INSERT INTO dbo.Product(ProductID ) VALUES(@index)
	SET @index = @index + 1
END
GO

--init ProductBuyFrequency table
DECLARE @index INT = 1
DECLARE @rndProductSysNo INT
DECLARE @rndCustomerBuyProductCount INT
DECLARE @rndCustomerSysNo INT
WHILE(@index <= 20000)
BEGIN

	SET @rndCustomerSysNo = CAST(FLOOR(RAND() * 2000) AS INT)
	IF (@rndCustomerSysNo = 0)
		SET @rndCustomerSysNo = 2

	SET @rndProductSysNo = CAST(FLOOR(RAND() * 10000) AS INT)
	IF (@rndProductSysNo = 0)
		SET @rndProductSysNo = 10

	SET @rndCustomerBuyProductCount = CAST(FLOOR(RAND() * 10) AS INT)
	IF (@rndCustomerBuyProductCount = 0)
		SET @rndCustomerBuyProductCount = 5

	INSERT INTO dbo.ProductBuyFrequency( CustomerSysNo ,ProductSysNo ,Frequency, Timespan)
	VALUES(@rndCustomerSysNo, @rndProductSysNo, @rndCustomerBuyProductCount, DATEDIFF(SECOND,'1969-01-01', GETDATE()))

	SET @index = @index + 1
END
GO
--init ProductClickFrequency table
DECLARE @index INT = 1
DECLARE @rndProductSysNo INT
DECLARE @rndCustomerBuyProductCount INT
DECLARE @rndCustomerSysNo INT
WHILE(@index <= 20000)
BEGIN

	SET @rndCustomerSysNo = CAST(FLOOR(RAND() * 2000) AS INT)
	IF (@rndCustomerSysNo = 0)
		SET @rndCustomerSysNo = 2

	SET @rndProductSysNo = CAST(FLOOR(RAND() * 10000) AS INT)
	IF (@rndProductSysNo = 0)
		SET @rndProductSysNo = 10

	SET @rndCustomerBuyProductCount = CAST(FLOOR(RAND() * 10) AS INT)
	IF (@rndCustomerBuyProductCount = 0)
		SET @rndCustomerBuyProductCount = 5

	INSERT INTO dbo.ProductClickFrequency( CustomerSysNo ,ProductSysNo ,Frequency, Timespan)
	VALUES(@rndCustomerSysNo, @rndProductSysNo, @rndCustomerBuyProductCount, DATEDIFF(SECOND,'1969-01-01', GETDATE()))

	SET @index = @index + 1
END
GO

--init ProductClickFrequency table
DECLARE @index INT = 1
DECLARE @rndProductSysNo INT
DECLARE @rndCustomerBuyProductCount INT
DECLARE @rndCustomerSysNo INT
WHILE(@index <= 20000)
BEGIN

	SET @rndCustomerSysNo = CAST(FLOOR(RAND() * 2000) AS INT)
	IF (@rndCustomerSysNo = 0)
		SET @rndCustomerSysNo = 2

	SET @rndProductSysNo = CAST(FLOOR(RAND() * 10000) AS INT)
	IF (@rndProductSysNo = 0)
		SET @rndProductSysNo = 10

	SET @rndCustomerBuyProductCount = CAST(FLOOR(RAND() * 10) AS INT)
	IF (@rndCustomerBuyProductCount = 0)
		SET @rndCustomerBuyProductCount = 5

	INSERT INTO dbo.ProductComment( CustomerSysNo ,ProductSysNo ,Frequency, Timespan)
	VALUES(@rndCustomerSysNo, @rndProductSysNo, @rndCustomerBuyProductCount, DATEDIFF(SECOND,'1969-01-01', GETDATE()))

	SET @index = @index + 1
END
