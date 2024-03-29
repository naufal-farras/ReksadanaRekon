USE [ReksadanaRekonTEST]
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertDataFundMANDIRI]    Script Date: 16/01/2022 20:38:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_InsertDataFundMANDIRI] 
	 @UserId nvarchar(128),
	 @NoRek nvarchar(20), 
	 @NaRek nvarchar(50) 

AS
BEGIN
 --   DECLARE @UserId nvarchar(128)= '04e46f39-cc75-4956-a337-23e817c5b672'
	--DECLARE @NoRek nvarchar(20)= '612550234'
	--DECLARE @NaRek nvarchar(50) = '612550234'
	DECLARE @Bulan nvarchar(2)
	DECLARE @Hari nvarchar(2)
	DECLARE @Tahun nvarchar(2)
	DECLARE @IDREK int
	DECLARE @CountFile int = 0
	DECLARE @CountBefore int = 0
	DECLARE @CountAfter int = 0

	iF EXISTS(SELECT * FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 
	'TEXT; HDR=NO;  Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 
    'SELECT * From DataFund.csv') a
	where REPLACE(REPLACE(REPLACE(REPLACE(LOWER(a.F5),' ', ''),'\', ''),'\\', ''),',', '') like '%description%' 
	and REPLACE(REPLACE(REPLACE(REPLACE(LOWER(a.F6),' ', ''),'\', ''),'\\', ''),',', '') like '%description%')

	BEGIN

			EXEC SP_MANDIRI @UserId,@NoRek,@NaRek;

		END
		ELSE
		BEGIN
			SELECT @CountFile AS FileAwal, @CountBefore AS DBAwal, @CountAfter AS DBAkhir, (@CountAfter - @CountBefore) AS Success, (@CountFile - (@CountAfter - @CountBefore)) AS Fails
		END
	END

	