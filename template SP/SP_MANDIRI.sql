USE [ReksadanaRekonTEST]
GO
/****** Object:  StoredProcedure [dbo].[SP_MANDIRI]    Script Date: 16/01/2022 20:38:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_MANDIRI]
	 @UserId nvarchar(128),
	 @NoRek nvarchar(20), 
	 @NaRek nvarchar(50) 
AS
BEGIN
DECLARE @Bulan nvarchar(2)
	DECLARE @Hari nvarchar(2)
	DECLARE @Tahun nvarchar(2)
	DECLARE @IDREK int
	DECLARE @CountFile int = 0
	DECLARE @CountBefore int = 0
	DECLARE @CountAfter int = 0

	IF NOT EXISTS( Select Id from Rekenings where NoRek=@NoRek)
			BEGIN
				INSERT INTO Rekenings ([NoRek]
					   ,[NamaRek]
					   ,[CreateDate]
					   ,[UserId])
					   values (@NoRek,@NaRek,GETDATE(),@UserId)
			END

		Select top 1 @IDREK=Id from Rekenings where NoRek=@NoRek 
	
		--SELECT   top 1 @Hari = SUBSTRING(a.F1,1,2) ,  @Bulan = SUBSTRING(a.F1,4,2) ,  @Tahun = SUBSTRING(a.F1,7,2)	
		-- FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
		--'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
		--'SELECT * From DataFund.csv')a where REPLACE(REPLACE(REPLACE(REPLACE(LOWER(a.F1),' ', ''),'\', ''),'\\', ''),',', '') not like '%postdate%'
	
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
		SELECT 
		'Rp'
		,convert(datetime,a.F2)
		,a.F5
		,REPLACE(REPLACE(REPLACE(UPPER(a.F5),' ', ''),',', ''),'.', '')
		,CONVERT(decimal,REPLACE(a.F9,',',''))
		,CONVERT(decimal,REPLACE(a.F9,',',''))
		,1
		,@IDREK
		,@UserId
		,GETDATE()
		,0
		 FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
		'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
		'SELECT * From DataFund.csv') a where a.f2 is not null 
		and REPLACE(REPLACE(REPLACE(REPLACE(LOWER(a.F5),' ', ''),'\', ''),'\\', ''),',', '') not like '%description%'
	

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
			SELECT @CountFile AS FileAwal, @CountBefore AS DBAwal, @CountAfter AS DBAkhir, (@CountAfter - @CountBefore) AS Success, (@CountFile - (@CountAfter - @CountBefore)) AS Fails

