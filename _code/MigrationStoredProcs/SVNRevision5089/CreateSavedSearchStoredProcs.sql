USE [sdox_nightlybuild_Mike]
GO

/* DeleteSavedSearch stored proc */
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[D_DeleteSavedSearch]
(
	@SearchId int
)
AS

DELETE FROM D_Search WHERE SearchId = @SearchId;
GO
