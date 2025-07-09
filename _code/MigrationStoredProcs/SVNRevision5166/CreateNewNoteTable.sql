/* Drop Note FK relationships and then drop Note table */
USE [sdox_nightlybuild_Mike]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_D_DocumentNote_D_Document]') AND parent_object_id = OBJECT_ID(N'[dbo].[D_DocumentNote]'))
ALTER TABLE [dbo].[D_DocumentNote] DROP CONSTRAINT [FK_D_DocumentNote_D_Document]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_D_DocumentNote_D_Note]') AND parent_object_id = OBJECT_ID(N'[dbo].[D_DocumentNote]'))
ALTER TABLE [dbo].[D_DocumentNote] DROP CONSTRAINT [FK_D_DocumentNote_D_Note]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[D_DocumentNote]') AND type in (N'U'))
DROP TABLE [dbo].[D_DocumentNote]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_D_Note_D_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[D_Note]'))
ALTER TABLE [dbo].[D_Note] DROP CONSTRAINT [FK_D_Note_D_User]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[D_Note]') AND type in (N'U'))
DROP TABLE [dbo].[D_Note]
GO
/*******************************************************/

/* Create new Note table */
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[D_Note](
	[NoteId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[NoteText] [text] NOT NULL,
	[LastModifiedDate] [datetime] NOT NULL,
	[TargetDoc] [varchar](128) NOT NULL,
	[TargetPtr] [varchar](256) NOT NULL,
 CONSTRAINT [PK_D_Note] PRIMARY KEY CLUSTERED 
(
	[NoteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
/*************************/

/* Create FK Relationships for new Note table */
ALTER TABLE [dbo].[D_Note]  WITH CHECK ADD  CONSTRAINT [FK_D_Note_D_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[D_User] ([UserId])
GO

ALTER TABLE [dbo].[D_Note] CHECK CONSTRAINT [FK_D_Note_D_User]
GO
/**********************************************/
