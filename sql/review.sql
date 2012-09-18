
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Review_Booking]') AND parent_object_id = OBJECT_ID(N'[dbo].[Review]'))
ALTER TABLE [dbo].[Review] DROP CONSTRAINT [FK_Review_Booking]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Review_Taxi]') AND parent_object_id = OBJECT_ID(N'[dbo].[Review]'))
ALTER TABLE [dbo].[Review] DROP CONSTRAINT [FK_Review_Taxi]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Review_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[Review]'))
ALTER TABLE [dbo].[Review] DROP CONSTRAINT [FK_Review_User]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Review_Active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Review] DROP CONSTRAINT [DF_Review_Active]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Review]') AND type in (N'U'))
DROP TABLE [dbo].[Review]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Review](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[BookingId] [int] NOT NULL,
	[TaxiId] [int] NOT NULL,
	[Rating] [tinyint] NOT NULL,
	[Comment] [varchar](300) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateLastModified] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO

ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_Review_Booking]
GO

ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_Taxi] FOREIGN KEY([TaxiId])
REFERENCES [dbo].[Taxi] ([Id])
GO

ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_Review_Taxi]
GO

ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_Review_User]
GO

ALTER TABLE [dbo].[Review] ADD  CONSTRAINT [DF_Review_Active]  DEFAULT ((1)) FOR [Active]
GO


