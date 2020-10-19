--2020-08-12 13:32:22.787

declare @LastRun nvarchar (100)
set @LastRun = '2020-10-13 03:32:05.890'


SELECT *--count(1)
  FROM [AmazonStockTracking].[dbo].[tbl_ASINStocks]
  where CreatedDate > @LastRun
  --order by CreatedDte desc

/****** Script for SelectTopNRows command from SSMS  ******/
SELECT count(1) CurrentCount, 

(Select count(1) from tbl_ASINs where Enabled = 1) ExpectedCount

,
  (SELECT ISNULL(sum(Convert(int, [NoOfSellers])),0) 
  FROM [AmazonStockTracking].[dbo].[tbl_ASINStocks]
  where CreatedDate > @LastRun) ASINStock

,(SELECT count(1)
  FROM [AmazonStockTracking].[dbo].[tbl_SellersStocks]
  where CreatedDate > @LastRun--'2020-08-03 09:10:31.467'
  ) SellerStocks
  
, (select max(createddate) FROM [AmazonStockTracking].[dbo].[tbl_SellersStocks]) LastRun
  FROM [AmazonStockTracking].[dbo].[tbl_ASINStocks]
  where CreatedDate > @LastRun
  --order by CreatedDate desc