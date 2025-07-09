USE [sdox_commondev_main]
GO

/****** Object:  StoredProcedure [dbo].[D_Report_GetDocumentsAccessedByBook]    Script Date: 07/19/2010 09:59:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Michael Murray
-- Create date: 16 Feb 2010
-- Description:	Reporting stored procedure to find
--	number of documents accessed by book for a given
--	date range.
-- =============================================
CREATE PROCEDURE [dbo].[D_Report_GetDocumentsAccessedByBook]
(
	@beginDate datetime,
	@endDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT YEAR([EventTime]) AS [Year],
	MONTH([EventTime]) AS [Month],
	DATENAME(month, [EventTime]) AS [MonthStr],
	DAY([EventTime]) AS [Day],
	SUBSTRING([Description],
		CHARINDEX('=', [Description]) + 1,
		CHARINDEX(';', [Description]) -
		(CHARINDEX('=', [Description]) + 1)) AS [BookName],
		COUNT(SUBSTRING([Description],
		CHARINDEX('=', [Description]) + 1,
		CHARINDEX(';', [Description]) -
		(CHARINDEX('=', [Description]) + 1))) AS [Total]
  FROM [D_EventLog]
  WHERE [EventTime] > @beginDate AND [EventTime] < @endDate
	AND [Name] = 'Content Accessed'
  GROUP BY YEAR([EventTime]), MONTH([EventTime]), DATENAME(month, [EventTime]), DAY([EventTime]),
	SUBSTRING([Description],
		CHARINDEX('=', [Description]) + 1,
		CHARINDEX(';', [Description]) -
		(CHARINDEX('=', [Description]) + 1))
  ORDER BY [Year], [Month], [MonthStr], [Day], [BookName]
END

GO

/****** Object:  StoredProcedure [dbo].[D_Report_GetLicenseAgreementCountsForFAFUsers]    Script Date: 07/19/2010 09:59:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Michael Murray
-- Create date: 16 Jul 2010
-- Description:	Reporting stored procedure to count
--	number of FAF users grouped by License Agreement
--  Status.
-- =============================================
CREATE PROCEDURE [dbo].[D_Report_GetLicenseAgreementCountsForFAFUsers]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT (CASE [D_User].[LicenseAgreement]
			WHEN 0 THEN 'Not Shown Yet' 
			WHEN 1 THEN 'Accepted'
			WHEN 2 THEN 'Declined'
			ELSE 'Error' END) AS [LicenseAgreement],
		 COUNT([D_User].[LicenseAgreement]) AS [Total]
  FROM
  (
	  SELECT DISTINCT [UserId], CONVERT(xml, [Description]).value('/userinfo[1]/permissions[1]/@domains', 'nvarchar(256)') AS [SubscriptionName]
	  FROM [D_EventLog]
	  WHERE [Name] = 'C2B Web Service Response'
  ) AS [subquery]
  JOIN [D_User]
	ON [D_User].UserId = [subquery].UserId
  WHERE [subquery].[SubscriptionName] LIKE '%fasb%' OR [subquery].[SubscriptionName] LIKE 'archive'
  GROUP BY [D_User].[LicenseAgreement]
  ORDER BY [D_User].[LicenseAgreement]
END

GO

/****** Object:  StoredProcedure [dbo].[D_Report_GetMaxConcurrentUsers]    Script Date: 07/19/2010 09:59:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Michael Murray
-- Create date: 23 Feb 2010
-- Description:	Reporting stored procedure to find
--	max number of concurrent users on the site per day
--	for a given date range.
-- =============================================
CREATE PROCEDURE [dbo].[D_Report_GetMaxConcurrentUsers]
(
	@beginDate datetime,
	@endDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#UserLoginLogouts') IS NULL
		CREATE TABLE #UserLoginLogouts
		(
			UserId UNIQUEIDENTIFIER,
			LoginTime DATETIME,
			LogoutTime DATETIME
		);
	ELSE 
		DELETE FROM #UserLoginLogouts;

	WITH [SuccessfulLogins] AS
	(
		SELECT [EventTime], [UserId]
		FROM [D_EventLog]
		WHERE [Module] = 'User' AND [Name] = 'User Logon'
			AND [EventTime] > @beginDate AND [EventTime] < @endDate
	)
	INSERT INTO #UserLoginLogouts(UserId, LoginTime, LogoutTime)
	SELECT [LoginNextLogin].[UserId], [LoginNextLogin].[LoginTime],
		MAX([UserActivities].[EventTime]) AS [LogoutTime]
	FROM
	(
		SELECT [Logins].[UserId], [Logins].[EventTime] AS [LoginTime],
			MIN([NextLogins].[EventTime]) AS [NextLogin]
		FROM [SuccessfulLogins] AS [Logins]
		INNER JOIN [SuccessfulLogins] AS [NextLogins]
			ON [Logins].[UserId] = [NextLogins].[UserId]
		WHERE [Logins].[EventTime] < [NextLogins].[EventTime]
		GROUP BY [Logins].[UserId], [Logins].[EventTime]
	) AS [LoginNextLogin]
	CROSS APPLY
	(
		SELECT [Events].[UserId], [Events].[EventTime]
		FROM [D_EventLog] AS [Events]
		WHERE [Name] <> 'C2B Web Service Response'
			AND [LoginNextLogin].[UserId] = [Events].[UserId]
			AND [Events].[EventTime] >= [LoginNextLogin].[LoginTime]
			AND [Events].[EventTime] < [LoginNextLogin].[NextLogin]
	) AS [UserActivities]
	GROUP BY [LoginNextLogin].[UserId], [LoginNextLogin].[LoginTime];

	SELECT YEAR([ConcurrentUsersAtTime].[LoginTime]) AS [Year],
			MONTH([ConcurrentUsersAtTime].[LoginTime]) AS [Month],
			DATENAME(month, [ConcurrentUsersAtTime].[LoginTime]) AS [MonthStr],
			DAY([ConcurrentUsersAtTime].[LoginTime]) AS [Day],
			MAX([ConcurrentUsersAtTime].[Total]) AS [MaxConcurrentUsers]
	FROM
	(
		SELECT [UserLogin].[LoginTime],
				COUNT(*) AS [Total]
		FROM #UserLoginLogouts AS [UserLogin]
		CROSS APPLY
		(
			SELECT [OtherLogins].[UserId], [OtherLogins].[LoginTime], [OtherLogins].[LogoutTime]
			FROM #UserLoginLogouts AS [OtherLogins]
			WHERE [OtherLogins].[LoginTime] <= [UserLogin].[LoginTime] AND
				[OtherLogins].[LogoutTime] > [UserLogin].[LoginTime]
		) AS [ConcurrentUsers]
		GROUP BY [UserLogin].[LoginTime]
	) AS [ConcurrentUsersAtTime]
	GROUP BY YEAR([ConcurrentUsersAtTime].[LoginTime]),
		MONTH([ConcurrentUsersAtTime].[LoginTime]),
		DATENAME(month, [ConcurrentUsersAtTime].[LoginTime]),
		DAY([ConcurrentUsersAtTime].[LoginTime])
	ORDER BY [Year], [Month], [MonthStr], [Day], [MaxConcurrentUsers];
END

GO

/****** Object:  StoredProcedure [dbo].[D_Report_GetMaxConcurrentUsersByFirm]    Script Date: 07/19/2010 09:59:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Michael Murray
-- Create date: 23 Feb 2010
-- Description:	Reporting stored procedure to find
--	max number of concurrent users by firm on the site
--	per day for a given date range.
-- =============================================
CREATE PROCEDURE [dbo].[D_Report_GetMaxConcurrentUsersByFirm]
(
	@beginDate datetime,
	@endDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#FirmLoginLogouts') IS NULL
		CREATE TABLE #FirmLoginLogouts
		(
			UserId UNIQUEIDENTIFIER,
			LoginTime DATETIME,
			LogoutTime DATETIME,
			FirmACA NVARCHAR(256),
			FirmCode INT
		);
	ELSE 
		DELETE FROM #FirmLoginLogouts;

	WITH [SuccessfulLogins] AS
	(
		SELECT [EventTime], [UserId],
			SUBSTRING([Description],
				CHARINDEX('=', [Description]) + 1,
				CHARINDEX(';', [Description]) -
				(CHARINDEX('=', [Description]) + 1)) AS FirmACA,
			SUBSTRING(REVERSE([Description]),
				CHARINDEX(';', REVERSE([Description])) + 1,
				CHARINDEX('=', REVERSE([Description])) -
				(CHARINDEX(';', REVERSE([Description])) + 1)) AS FirmCode
		FROM [D_EventLog]
		WHERE [Module] = 'User' AND [Name] = 'Firm Logon'
			AND [EventTime] > @beginDate AND [EventTime] < @endDate
	)
	INSERT INTO #FirmLoginLogouts(UserId, LoginTime, LogoutTime, FirmACA, FirmCode)
	SELECT [LoginNextLogin].[UserId], [LoginNextLogin].[LoginTime],
		MAX([UserActivities].[EventTime]) AS [LogoutTime],
		[LoginNextLogin].[FirmACA], [LoginNextLogin].[FirmCode]
	FROM
	(
		SELECT [Logins].[UserId], [Logins].[EventTime] AS [LoginTime],
			MIN([NextLogins].[EventTime]) AS [NextLogin],
			[Logins].[FirmACA], [Logins].[FirmCode]
		FROM [SuccessfulLogins] AS [Logins]
		INNER JOIN [SuccessfulLogins] AS [NextLogins]
			ON [Logins].[UserId] = [NextLogins].[UserId]
		WHERE [Logins].[EventTime] < [NextLogins].[EventTime]
		GROUP BY [Logins].[UserId], [Logins].[EventTime], [Logins].[FirmACA], [Logins].[FirmCode]
	) AS [LoginNextLogin]
	CROSS APPLY
	(
		SELECT [Events].[UserId], [Events].[EventTime]
		FROM [D_EventLog] AS [Events]
		WHERE [Name] <> 'C2B Web Service Response' AND [Name] <> 'User Logon'
			AND [LoginNextLogin].[UserId] = [Events].[UserId]
			AND [Events].[EventTime] >= [LoginNextLogin].[LoginTime]
			AND [Events].[EventTime] < [LoginNextLogin].[NextLogin]
	) AS [UserActivities]
	GROUP BY [LoginNextLogin].[UserId], [LoginNextLogin].[LoginTime],
		[LoginNextLogin].[FirmACA], [LoginNextLogin].[FirmCode];

	SELECT YEAR([ConcurrentUsersAtTime].[LoginTime]) AS [Year],
			MONTH([ConcurrentUsersAtTime].[LoginTime]) AS [Month],
			DATENAME(month, [ConcurrentUsersAtTime].[LoginTime]) AS [MonthStr],
			DAY([ConcurrentUsersAtTime].[LoginTime]) AS [Day],
			(CAST([ConcurrentUsersAtTime].[FirmCode] AS NVARCHAR(32)) + ' (' +
				[ConcurrentUsersAtTime].[FirmACA] + ')') AS [Firm],
			[ConcurrentUsersAtTime].[FirmCode],
			MAX([ConcurrentUsersAtTime].[Total]) AS [MaxConcurrentUsers]
	FROM
	(
		SELECT [UserLogin].[LoginTime], [UserLogin].[FirmCode], [UserLogin].[FirmACA],
				COUNT(*) AS [Total]
		FROM #FirmLoginLogouts AS [UserLogin]
		CROSS APPLY
		(
			SELECT [OtherLogins].[UserId], [OtherLogins].[LoginTime], [OtherLogins].[LogoutTime]
			FROM #FirmLoginLogouts AS [OtherLogins]
			WHERE [OtherLogins].[LoginTime] <= [UserLogin].[LoginTime] AND
				[OtherLogins].[LogoutTime] > [UserLogin].[LoginTime] AND
				[OtherLogins].[FirmACA] = [UserLogin].[FirmACA] AND
				[OtherLogins].[FirmCode] = [UserLogin].[FirmCode]
		) AS [ConcurrentUsers]
		GROUP BY [UserLogin].[LoginTime], [UserLogin].[FirmCode], [UserLogin].[FirmACA]
	) AS [ConcurrentUsersAtTime]
	GROUP BY YEAR([ConcurrentUsersAtTime].[LoginTime]),
		MONTH([ConcurrentUsersAtTime].[LoginTime]),
		DATENAME(month, [ConcurrentUsersAtTime].[LoginTime]),
		DAY([ConcurrentUsersAtTime].[LoginTime]),
		[ConcurrentUsersAtTime].[FirmCode],
		[ConcurrentUsersAtTime].[FirmACA]
	ORDER BY [Year], [Month], [MonthStr], [Day], [FirmCode], [MaxConcurrentUsers];
END

GO

/****** Object:  StoredProcedure [dbo].[D_Report_GetUserLogins]    Script Date: 07/19/2010 09:59:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Michael Murray
-- Create date: 19 Feb 2010
-- Description:	Reporting stored procedure to find
--	number of user logins for a given date range.
-- =============================================
CREATE PROCEDURE [dbo].[D_Report_GetUserLogins]
(
	@beginDate datetime,
	@endDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT [SuccessfulLogins].[Year], [SuccessfulLogins].[Month],
	[SuccessfulLogins].[MonthStr], [SuccessfulLogins].[Day],
	[UniqueUsers].[ReferringSite], COUNT(*) AS [Total]
  FROM
  (
	  SELECT DISTINCT CAST(UPPER(REVERSE(SUBSTRING(REVERSE([Description]),
			CHARINDEX('}', REVERSE([Description])) + 1,
			CHARINDEX('{', REVERSE([Description])) -
			CHARINDEX('}', REVERSE([Description])) - 1))) AS UNIQUEIDENTIFIER) AS [UserGuid],
		SUBSTRING([Description],
			CHARINDEX('=', [Description]) + 2,
			(CHARINDEX(',', [Description]) - 1) -
			(CHARINDEX('=', [Description]) + 2)) AS [ReferringSite]
	  FROM [D_EventLog]
	  WHERE [Module] = 'ResourceSeamlessLogin.aspx'
		AND [Name] <> 'Error'
		AND [EventTime] > @beginDate AND [EventTime] < @endDate
  ) AS [UniqueUsers]
  JOIN
  (
	  SELECT YEAR([EventTime]) AS [Year],
		MONTH([EventTime]) AS [Month],
		DATENAME(month, [EventTime]) AS [MonthStr],
		DAY([EventTime]) AS [Day],
		[UserId]
	  FROM [D_EventLog]
	  WHERE [Module] = 'User' AND [Name] = 'User Logon'
		AND [EventTime] > @beginDate AND [EventTime] < @endDate
  ) AS [SuccessfulLogins]
	ON [SuccessfulLogins].[UserId] = [UniqueUsers].[UserGuid]
  GROUP BY [SuccessfulLogins].[Year], [SuccessfulLogins].[Month],
	[SuccessfulLogins].[MonthStr], [SuccessfulLogins].[Day],
	[UniqueUsers].[ReferringSite]
  ORDER BY [SuccessfulLogins].[Year], [SuccessfulLogins].[Month],
	[SuccessfulLogins].[MonthStr], [SuccessfulLogins].[Day],
	[UniqueUsers].[ReferringSite]
END

GO

/****** Object:  StoredProcedure [dbo].[D_Report_GetUserLoginsBySubscription]    Script Date: 07/19/2010 10:00:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Michael Murray
-- Create date: 16 Feb 2010
-- Description:	Reporting stored procedure to find
--	number of users logging in by subscriptions for
--	a given date range.
-- =============================================
CREATE PROCEDURE [dbo].[D_Report_GetUserLoginsBySubscription]
(
	@beginDate datetime,
	@endDate datetime
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  SELECT subquery.[Year], subquery.[Month], subquery.[MonthStr], subquery.[Day],
	subquery.[SubscriptionName], COUNT(subquery.[SubscriptionName]) AS Total
  FROM
  (
	  SELECT YEAR([EventTime]) AS [Year],
		MONTH([EventTime]) AS [Month],
		DATENAME(month, [EventTime]) AS [MonthStr],
		DAY([EventTime]) AS [Day],
		CONVERT(xml, [Description]).value('/userinfo[1]/permissions[1]/@domains', 'nvarchar(256)') AS [SubscriptionName]
	  FROM [D_EventLog]
	  WHERE [EventTime] > @beginDate AND [EventTime] < @endDate
		AND [Name] = 'C2B Web Service Response'
  ) AS subquery
  GROUP BY subquery.[Year], subquery.[Month], subquery.[MonthStr], subquery.[Day],
	subquery.[SubscriptionName]
  ORDER BY subquery.[Year], subquery.[Month], subquery.[MonthStr], subquery.[Day],
	subquery.[SubscriptionName]
END

GO
