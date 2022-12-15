USE [ReksadanaRekonTEST]
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertDataFund]    Script Date: 16/01/2022 20:37:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_InsertDataFund] 
	 @UserId nvarchar(128) 
AS
BEGIN
	DECLARE @Tahun nvarchar(4)
	DECLARE @NOREK nvarchar(20)
	DECLARE @NAMAREK nvarchar(50)
	DECLARE @IDREK int
	DECLARE @CountFile int = 0
	DECLARE @CountBefore int = 0
	DECLARE @CountAfter int = 0

	iF EXISTS(SELECT a.F1 FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT F1 From DataFund.csv') a
	where REPLACE(REPLACE(REPLACE(REPLACE(LOWER(a.F1),' ', ''),'\', ''),'\\', ''),',', '') like '%informasirekening-mutasirekening%')
	BEGIN

	iF NOT EXISTS(SELECT a.F1 FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT F1 From DataFund.csv') a
	where a.F1 like '%Tidak%')
	BEGIN	
	
CREATE TABLE #DataFundsVM (
	[CCY] [nvarchar](max) NULL,
	[Tanggal] [datetime] NOT NULL,
	[Keterangan] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[KeteranganDua] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[Jumlah] [bigint] NOT NULL,
	[Saldo] [bigint] NOT NULL,
	[MatchingId] [int] NOT NULL,
	[RekeningId] [int] NOT NULL,
	[UserId] [nvarchar](128) NULL,
	[CreateDate] [datetime] NOT NULL,
	[IsDelete] [bit] NOT NULL)
	
	SELECT @NOREK = SUBSTRING(a.F1, 16,20)
	FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT F1 From DataFund.csv') a
	where a.F1 like '%No. rekening%'
		
	SELECT @NAMAREK = SUBSTRING(a.F1, 8,50)
	FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT F1 From DataFund.csv') a
	where a.F1 like '%Nama%'
	
    IF NOT EXISTS( Select Id from Rekenings where NamaRek=@NAMAREK and NoRek=@NOREK)
	BEGIN
	INSERT INTO Rekenings ([NoRek]
           ,[NamaRek]
           ,[CreateDate]
		   ,[UserId])
		   values (@NOREK,@NAMAREK,GETDATE(),@UserId)
	END

	Select @IDREK=Id from Rekenings where NamaRek=@NAMAREK and NoRek=@NOREK
		 
	SELECT top 1 @Tahun =  RIGHT(a.F1,4)  
	FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT * From DataFund.csv') a where a.F1 like '%Periode%'

	INSERT INTO #DataFundsVM
			([CCY]
           ,[Tanggal]
           ,[Keterangan]
           ,[KeteranganDua]
           ,[Jumlah]
           ,[Saldo]
           ,[MatchingId]
           ,[RekeningId]
           ,[UserId]
           ,[CreateDate]
           ,[IsDelete])
	SELECT 'Rp'
	,convert(datetime, CONCAT(@Tahun,RIGHT(a.F1, 2),LEFT(a.F1,2)))
	,a.F2
	,REPLACE(a.F2, ' ', '')
	,CONVERT(decimal, REPLACE(REPLACE(a.F4, 'CR', ''), ',',''))
	,CONVERT(decimal,REPLACE(a.F5,',',''))
	,1
	,@IDREK
	,@UserId
	,GETDATE()
	,0
	FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT * From DataFund.csv') a
	where ISNUMERIC(a.F5) = 1 AND RIGHT(a.F4, 2) = 'CR'

	SELECT @CountFile = COUNT(CCY) FROM #DataFundsVM
	SELECT @CountBefore = COUNT(Id) FROM DataFunds

	INSERT INTO [dbo].[DataFunds]
           ([CCY]
           ,[Tanggal]
           ,[Keterangan]
           ,[KeteranganDua]
           ,[Jumlah]
           ,[Saldo]
           ,[MatchingId]
           ,[RekeningId]
           ,[UserId]
           ,[CreateDate]
           ,[IsDelete])
	SELECT [CCY]
           ,[Tanggal]
           ,[Keterangan]
           ,[KeteranganDua]
           ,[Jumlah]
           ,[Saldo]
           ,[MatchingId]
           ,[RekeningId]
           ,[UserId]
           ,[CreateDate]
           ,[IsDelete]
	FROM #DataFundsVM a
	where NOT EXISTS (SELECT * from [DataFunds] b where a.RekeningId = b.RekeningId 
	and a.Keterangan = b.Keterangan and a.Saldo = b.Saldo and a.Jumlah = b.Jumlah
	and a.Tanggal= b.Tanggal )

	SELECT @CountAfter = COUNT(Id) FROM DataFunds
	drop table #DataFundsVM
	
	END
	END
	SELECT @CountFile AS FileAwal, @CountBefore AS DBAwal, @CountAfter AS DBAkhir, (@CountAfter - @CountBefore) AS Success, (@CountFile - (@CountAfter - @CountBefore)) AS Fails

	END
