USE [ReksadanaRekonTEST]
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertDataAplikasi]    Script Date: 16/01/2022 20:36:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_InsertDataAplikasi] 
	 @UserId nvarchar(128) 	
AS
BEGIN

	DECLARE @CountFile int = 0
	DECLARE @CountBefore int = 0
	DECLARE @CountAfter int = 0

	
	iF EXISTS(SELECT a.F1 FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0',
	'Excel 8.0;HDR=NO;Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\DataAplikasi.xls',
	'select * from [sheet1$]') a
	where  REPLACE(LOWER(a.F9),' ', '') like '%saname%' 
	AND REPLACE(LOWER(a.F37),' ', '') like '%sareferenceno.%')
	BEGIN
	
CREATE TABLE #DataAplikasisVM(
	[TransactionDate] [datetime] NOT NULL,
	[TransactionType] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[ReferenceNo] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[Status] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[IMFeeAmendment] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[IMPaymentDate] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[SACode] [nvarchar](max)collate SQL_Latin1_General_CP1_CI_AS NULL ,
	[SAName] [nvarchar](max)collate SQL_Latin1_General_CP1_CI_AS NULL ,
	[InvestorFundUnitNo] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[InvestorFundUnitName] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[SID] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[FundCode] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL ,
	[FundName] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL ,
	[IMCode] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[IMName] [nvarchar](max)collate SQL_Latin1_General_CP1_CI_AS NULL ,
	[CBCode] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[CBName] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[FundCCY] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[AmountNominal] [bigint] NOT NULL,
	[AmountUnit] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[AmountAll] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[FeeNominal] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[FeeUnit] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[FeePercent] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[TransferPath] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[REDMSequentialCode] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[REDMBICCode] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[REDMBIMemberCode] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[REDMBankName] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[REDMNo] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[REDMName] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[PaymentDate] [datetime] NULL,
	[TransferType] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[InputDate] [datetime] NULL,
	[UploadReference] [nvarchar](max) NULL,
	[SAReference] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[IMStatus] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[CBStatus] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[CBCompletionStatus] [nvarchar](max) collate SQL_Latin1_General_CP1_CI_AS NULL,
	[MatchingId] [int] NOT NULL
	)

INSERT INTO #DataAplikasisVM
	SELECT CONVERT(datetime, a.F2)
	,a.F3
	,a.F4
	,a.F5
	,a.F6
	,a.F7
	,a.F8
	,a.F9
	,a.F10
	,a.F11
	,a.F12
	,a.F13
	,a.F14
	,a.F15
	,a.F16
	,a.F17
	,a.F18
	,a.F19
	,a.F20
	,a.F21
	,a.F22
	,a.F23
	,a.F24
	,a.F25
	,a.F26
	,a.F27
	,a.F28
	,a.F29
	,a.F30
	,a.F31
	,a.F32
	,CONVERT(datetime, a.F33)
	,a.F34
	,CONVERT(datetime, a.F35)
	,a.F36
	,a.F37
	,a.F38
	,a.F39
	,a.F40
	,1
	FROM OPENROWSET(
	'Microsoft.ACE.OLEDB.12.0',
	'Excel 8.0;HDR=NO;Database=C:\inetpub\wwwroot\ReksadanaRekon-2\Content\Files\DataAplikasi.xls',
	'select * from [sheet1$]') a
	WHERE a.F1 is not null			  

INSERT INTO [Funds]
	([Code]
	,[Nama]
	,[UserId]
	,[CreateDate]
	,[IsDelete])
	SELECT DISTINCT a.[FundCode], a.[FundName], @UserId, GETDATE(), 0 
	FROM #DataAplikasisVM a
	WHERE NOT EXISTS (
	SELECT Id FROM [Funds] b WHERE a.[FundCode] = b.Code AND a.[FundName] = b.Nama)

INSERT INTO [MIs]
	([Code]
	,[Nama]
	,[UserId]
	,[CreateDate]
	,[IsDelete])
	SELECT DISTINCT a.[IMCode], a.[IMName], @UserId, GETDATE(), 0 
	FROM #DataAplikasisVM a
	WHERE NOT EXISTS (
	SELECT Id FROM [MIs] b WHERE a.[IMCode] = b.Code AND a.[IMName] = b.Nama)

INSERT INTO [SAs]
	([Code]
	,[Nama]
	,[UserId]
	,[CreateDate]
	,[IsDelete])
	SELECT DISTINCT a.[SACode], a.[SAName], @UserId, GETDATE(), 0 
	FROM #DataAplikasisVM a
	WHERE NOT EXISTS (
	SELECT Id FROM [SAs] b WHERE a.[SACode] = b.Code AND a.[SAName] = b.Nama)
	
	SELECT @CountFile = COUNT(AmountNominal) FROM #DataAplikasisVM
	SELECT @CountBefore = COUNT(Id) FROM DataAplikasis

INSERT INTO [DataAplikasis]
		([TransactionDate]
		,[TransactionType]
		,[ReferenceNo]
		,[Status]
		,[IMFeeAmendment]
		,[IMPaymentDate]
		,[SACode]
		,[SAName]
		,[InvestorFundUnitNo]
		,[InvestorFundUnitName]
		,[SID]
		,[FundCode]
		,[FundName]
		,[IMCode]
		,[IMName]
		,[CBCode]
		,[CBName]
		,[FundCCY]
		,[AmountNominal]
		,[AmountUnit]
		,[AmountAll]
		,[FeeNominal]
		,[FeeUnit]
		,[FeePercent]
		,[TransferPath]
		,[REDMSequentialCode]
		,[REDMBICCode]
		,[REDMBIMemberCode]
		,[REDMBankName]
		,[REDMNo]
		,[REDMName]
		,[PaymentDate]
		,[TransferType]
		,[InputDate]
		,[UploadReference]
		,[SAReference]
		,[IMStatus]
		,[CBStatus]
		,[CBCompletionStatus]
		,[MatchingId]
		,[SAId]
		,[FundId]
		,[MIId]
		,[UserId]
		,[CreateDate]
		,[IsDelete])
	SELECT [TransactionDate]
		,[TransactionType]
		,[ReferenceNo]
		,[Status]
		,[IMFeeAmendment]
		,[IMPaymentDate]
		,[SACode]
		,[SAName]
		,[InvestorFundUnitNo]
		,[InvestorFundUnitName]
		,[SID]
		,[FundCode]
		,[FundName]
		,[IMCode]
		,[IMName]
		,[CBCode]
		,[CBName]
		,[FundCCY]
		,[AmountNominal]
		,[AmountUnit]
		,[AmountAll]
		,[FeeNominal]
		,[FeeUnit]
		,[FeePercent]
		,[TransferPath]
		,[REDMSequentialCode]
		,[REDMBICCode]
		,[REDMBIMemberCode]
		,[REDMBankName]
		,[REDMNo]
		,[REDMName]
		,[PaymentDate]
		,[TransferType]
		,[InputDate]
		,[UploadReference]
		,[SAReference]
		,[IMStatus]
		,[CBStatus]
		,[CBCompletionStatus]
		,[MatchingId]
		,d.Id
		,b.Id
		,c.Id
		,@UserId
		,GETDATE()
		,0
		FROM #DataAplikasisVM a
		RIGHT JOIN [Funds] b on a.FundName like b.Nama
		RIGHT JOIN [MIs] c on a.IMName like c.Nama
		RIGHT JOIN [SAs] d on a.SAName like d.Nama
		WHERE TransactionDate is not null AND NOT EXISTS (
		SELECT SAId FROM [DataAplikasis] e WHERE d.Id = e.SAId AND c.Id = e.MIId AND b.Id = e.FundId 
		AND a.InvestorFundUnitNo = e.InvestorFundUnitNo AND a.AmountNominal = e.AmountNominal 
		AND a.TransactionDate = e.TransactionDate AND a.SAReference = e.SAReference AND a.ReferenceNo = e.ReferenceNo);

SELECT @CountAfter = COUNT(Id) FROM DataAplikasis		
DROP TABLE #DataAplikasisVM

END
	SELECT @CountFile AS FileAwal, @CountBefore AS DBAwal, @CountAfter AS DBAkhir, (@CountAfter - @CountBefore) AS Success, (@CountFile - (@CountAfter - @CountBefore)) AS Fails

END
