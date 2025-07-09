USE [sdox_nightlybuild_Mike]
GO

/* InsertSavedSearch stored proc */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[D_InsertSavedSearch]
(
  	@UserId uniqueidentifier,
	@Name varchar(128),
	@SearchCriteria text,
	@LastModifiedDate datetime
)
AS

INSERT INTO D_Search (UserId, [Name], SearchCriteria, LastModifiedDate)
VALUES (@UserId, @Name, @SearchCriteria, @LastModifiedDate);
SELECT @@IDENTITY
GO

/* UpdateSavedSearch stored proc */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[D_UpdateSavedSearch]
(
	@SearchId int,
  	@UserId uniqueidentifier,
	@Name varchar(128),
	@SearchCriteria text,
	@LastModifiedDate datetime
)
AS

UPDATE D_Search
SET UserId = @UserId, [Name] = @Name, SearchCriteria = @SearchCriteria, LastModifiedDate = @LastModifiedDate
WHERE SearchId = @SearchId;
GO

