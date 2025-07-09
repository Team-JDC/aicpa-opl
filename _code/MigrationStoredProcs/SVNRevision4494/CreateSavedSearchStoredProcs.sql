USE [sdox_nightlybuild_Mike]
GO

/* GetSavedSearch stored proc*/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[D_GetSavedSearch]
(
  @SearchId int
)

AS

SELECT	SearchId, UserId, [Name], SearchCriteria, LastModifiedDate
FROM 	D_Search
WHERE 	SearchId = @SearchId;
GO

/* GetSavedSearchesForUser stored proc */
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[D_GetSavedSearchesForUser]
(
  @UserId uniqueidentifier
)

AS

SELECT	SearchId, UserId, [Name], SearchCriteria, LastModifiedDate
FROM 	D_Search
WHERE 	UserId = @UserId;
GO

/* InsertSavedSearch stored proc */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[D_InsertSavedSearch]
(
  	@UserId uniqueidentifier,
	@Name varchar(128),
	@SearchCriteria text,
	@LastModifiedDate datetime
)
AS

INSERT INTO D_Search (UserId, [Name], SearchCriteria, LastModifiedDate)
VALUES (@UserId, @Name, @SearchCriteria, @LastModifiedDate);
GO
