USE [master]
GO
/****** Object:  Database [AmazonStockTracking]    Script Date: 9/16/2020 10:24:44 AM ******/
CREATE DATABASE [AmazonStockTracking]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AmazonStockTracking', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\AmazonStockTracking.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AmazonStockTracking_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\AmazonStockTracking_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [AmazonStockTracking] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AmazonStockTracking].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
USE [AmazonStockTracking]
GO
/****** Object:  User [NT AUTHORITY\SYSTEM]    Script Date: 9/16/2020 10:24:44 AM ******/
CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [NT AUTHORITY\NETWORK SERVICE]    Script Date: 9/16/2020 10:24:44 AM ******/
CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\NETWORK SERVICE]
GO
/****** Object:  Schema [SHARJAH\svc_solarwind]    Script Date: 9/16/2020 10:24:44 AM ******/
CREATE SCHEMA [SHARJAH\svc_solarwind]
GO

USE [AmazonStockTracking]
GO
/****** Object:  UserDefinedTableType [dbo].[CSSellersObject]    Script Date: 19/10/2020 7:07:02 PM ******/
CREATE TYPE [dbo].[CSSellersObject] AS TABLE(
	[SellerName] [nvarchar](max) NULL,
	[Quantity] [nvarchar](10) NULL,
	[Price] [nvarchar](10) NULL,
	[Type] [nvarchar](10) NULL,
	[Rating] [nvarchar](20) NULL,
	[Condition] [nvarchar](20) NULL,
	[Reviews] [nvarchar](20) NULL
)
GO
/****** Object:  StoredProcedure [dbo].[USP_DisableASIN]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_DisableASIN]
	@ASIN as nvarchar(15)
AS
BEGIN
	UPDATE [dbo].[tbl_ASINs]
		SET [Enabled] = 0,
		UpdateDate = GetDate()
      WHERE [ASIN] = @ASIN
END

GO
/****** Object:  StoredProcedure [dbo].[USP_EnableASIN]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_EnableASIN]
	@ASIN as nvarchar(15)
AS
BEGIN
	UPDATE [dbo].[tbl_ASINs]
		SET [Enabled] = 1,
		UpdateDate = GetDate()
      WHERE [ASIN] = @ASIN
END

GO
/****** Object:  StoredProcedure [dbo].[USP_Insert_tbltest]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_Insert_tbltest]
	@testString as nvarchar(100)
AS
BEGIN
INSERT INTO [dbo].[tbl_test]
           ([testString])
     VALUES
           (@testString)
END

GO
/****** Object:  StoredProcedure [dbo].[USP_InsertASIN]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_InsertASIN]
	@ASIN as nvarchar(15)
AS
BEGIN
	INSERT INTO [dbo].[tbl_ASINs]
           ([ASIN])           
     VALUES
           (@ASIN)
END

GO
/****** Object:  StoredProcedure [dbo].[USP_InsertTrackingRecord]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[USP_InsertTrackingRecord]
	@ASIN nvarchar(15),
	@TotalStockCount nvarchar(10),
	@NoOfSellers nvarchar(10),
	@totalFBAStock nvarchar(10),
	@totalFBMStock nvarchar(10),
	@totalAMZStock nvarchar(10),
	@CSSellersObject [dbo].[CSSellersObject] READONLY
AS
BEGIN
	INSERT INTO [dbo].[tbl_ASINStocks]
           ([ASIN]
           ,[TotalCount]
           ,[FBACount]
           ,[FBMCount]
           ,[AMZCount]
           ,[NoOfSellers]
           )
     VALUES
           (@ASIN,
		   @TotalStockCount,
		   @totalFBAStock,
		   @totalFBMStock,
		   @totalAMZStock,
		   @NoOfSellers)

			INSERT INTO [dbo].[tbl_SellersStocks]
					   ([ASIN]
					   ,[SellerName]
					   ,[Quantity]
					   ,[Price]
					   ,[Type]
					   ,[Rating]
					   ,[Condition],
					   Reviews)
					Select
						@ASIN,
						SellerName,
						[Quantity],
						[Price],
						[Type],
						[Rating],
						[Condition],
						Reviews
					from @CSSellersObject

END





GO
/****** Object:  StoredProcedure [dbo].[USP_SelectEnabledASINs]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_SelectEnabledASINs]
(
	@groupId int
)
AS
BEGIN
	 -- declare @LastRun nvarchar (100)
		--set @LastRun = '2020-09-14 15:24:31.893'

	SELECT [ASIN]
		  ,[Enabled]
		  ,Convert(varchar, [CreatedDate], 120) CreatedDate
		  ,Convert(varchar, [UpdateDate], 120) UpdateDate
	  FROM [dbo].[tbl_ASINs]
	  where [group] = @groupId and [Enabled] = 1
	 -- and asin not in (		
		--SELECT ASIN
		--  FROM [AmazonStockTracking].[dbo].[tbl_ASINStocks]
		--  where CreatedDate > @LastRun
		--)
	  order by CreatedDate desc
END

GO
/****** Object:  StoredProcedure [dbo].[USP_SelectEnabledASINs_NoGroup]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_SelectEnabledASINs_NoGroup]
AS
BEGIN
	 -- declare @LastRun nvarchar (100)
		--set @LastRun = '2020-09-14 15:24:31.893'

	SELECT [ASIN]
		  ,[Enabled]
		  ,Convert(varchar, [CreatedDate], 120) CreatedDate
		  ,Convert(varchar, [UpdateDate], 120) UpdateDate
	  FROM [dbo].[tbl_ASINs]
	  where [Enabled] = 1
	 -- and asin not in (		
		--SELECT ASIN
		--  FROM [AmazonStockTracking].[dbo].[tbl_ASINStocks]
		--  where CreatedDate > @LastRun
		--)
	  order by CreatedDate desc
END

GO
/****** Object:  Table [dbo].[tbl_ASINs]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ASINs](
	[ASIN] [nvarchar](15) NOT NULL,
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_tbl_ASINs_Enable]  DEFAULT ((1)),
	[CreatedDate] [datetime] NOT NULL CONSTRAINT [DF_tbl_ASINs_CreatedDate]  DEFAULT (getdate()),
	[UpdateDate] [datetime] NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[Group] [int] NULL,
 CONSTRAINT [PK_tbl_ASINs] PRIMARY KEY CLUSTERED 
(
	[ASIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ASINStocks]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ASINStocks](
	[ASIN] [nvarchar](15) NOT NULL,
	[TotalCount] [nvarchar](10) NULL,
	[FBACount] [nvarchar](10) NULL,
	[FBMCount] [nvarchar](10) NULL,
	[AMZCount] [nvarchar](10) NULL,
	[NoOfSellers] [nvarchar](10) NULL,
	[CreatedDate] [datetime] NOT NULL CONSTRAINT [DF_tbl_ASINStocks_CreatedDate]  DEFAULT (getdate())
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_SellersStocks]    Script Date: 19/10/2020 7:07:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_SellersStocks](
	[ASIN] [nvarchar](15) NOT NULL,
	[SellerName] [nvarchar](max) NULL,
	[Quantity] [nvarchar](10) NULL,
	[Price] [nvarchar](10) NULL,
	[Type] [nvarchar](10) NULL,
	[Rating] [nvarchar](20) NULL,
	[Condition] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NOT NULL CONSTRAINT [DF_tbl_SellersStocks_CreatedDate]  DEFAULT (getdate()),
	[Reviews] [nvarchar](20) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[tbl_ASINStocks]  WITH CHECK ADD  CONSTRAINT [FK_tbl_ASINStocks_tbl_ASINs] FOREIGN KEY([ASIN])
REFERENCES [dbo].[tbl_ASINs] ([ASIN])
GO
ALTER TABLE [dbo].[tbl_ASINStocks] CHECK CONSTRAINT [FK_tbl_ASINStocks_tbl_ASINs]
GO
ALTER TABLE [dbo].[tbl_SellersStocks]  WITH CHECK ADD  CONSTRAINT [FK_tbl_SellersStocks_tbl_ASINs] FOREIGN KEY([ASIN])
REFERENCES [dbo].[tbl_ASINs] ([ASIN])
GO
ALTER TABLE [dbo].[tbl_SellersStocks] CHECK CONSTRAINT [FK_tbl_SellersStocks_tbl_ASINs]
GO
