USE [ReksadanaRekonTEST]
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertDataFundBNI]    Script Date: 16/01/2022 20:37:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_InsertDataFundBNI] 
	 @UserId nvarchar(128),
	 @NoRek nvarchar(20), 
	 @NaRek nvarchar(50) 

AS
BEGIN
 --   DECLARE @UserId nvarchar(128)= '04e46f39-cc75-4956-a337-23e817c5b672'
	--DECLARE @NoRek nvarchar(20)= 'BNI'
	--DECLARE @NaRek nvarchar(50) = 'BNI'
	DECLARE @Bulan nvarchar(10)
	DECLARE @Hari nvarchar(10)
	DECLARE @Tahun nvarchar(10)
	DECLARE @IDREK int
	DECLARE @CountFile int = 0
	DECLARE @CountBefore int = 0
	DECLARE @CountAfter int = 0
	DECLARE @error nvarchar(50)


	IF EXISTS(SELECT * FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 'TEXT; HDR=YES;  
	Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\', 'SELECT * From DataFund.csv') a  
	where a.[Post Date] IS NOT NULL and  a.[Value Date] IS NOT NULL)
		BEGIN
		
			EXEC dbo.SP_BNI @UserId,@NoRek,@NaRek; 
		END
		ELSE
		BEGIN

		
			SELECT @CountFile AS FileAwal, @CountBefore AS DBAwal, @CountAfter AS DBAkhir, (@CountAfter - @CountBefore) AS Success,
			 (@CountFile - (@CountAfter - @CountBefore)) AS Fails 
		END
	END

	