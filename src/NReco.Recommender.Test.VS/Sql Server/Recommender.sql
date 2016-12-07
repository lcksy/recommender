
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
				WHERE object_id = OBJECT_ID('ProductClickFrequency')
					AND type = 'U')
BEGIN
	CREATE TABLE ProductClickFrequency
	(
		SysNo BIGINT PRIMARY KEY IDENTITY(1,1),
		CustomerSysNo INT,
		ProductSysNo INT,
		Frequency INT
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
		Frequency INT
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
		Frequency INT
	)
END
GO

--init Customer table
DECLARE @customerCount INT = 1000
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
DECLARE @index INT = 100
DECLARE @productCount INT =100100
WHILE(@index <= @productCount)
BEGIN
	INSERT INTO dbo.Product(ProductID ) VALUES(@index)
	SET @index = @index + 1
END
GO

--select cast( floor(rand()*10) as int)
SELECT * FROM dbo.Product --100000

