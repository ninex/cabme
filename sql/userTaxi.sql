IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserTaxi_Taxi]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserTaxi]'))
ALTER TABLE [dbo].[UserTaxi] DROP CONSTRAINT [FK_UserTaxi_Taxi]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserTaxi_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserTaxi]'))
ALTER TABLE [dbo].[UserTaxi] DROP CONSTRAINT [FK_UserTaxi_User]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserTaxi]') AND type in (N'U'))
DROP TABLE [dbo].[UserTaxi]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserTaxi](
	[UserId] [int] NOT NULL,
	[TaxiId] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserTaxi]  WITH CHECK ADD  CONSTRAINT [FK_UserTaxi_Taxi] FOREIGN KEY([TaxiId])
REFERENCES [dbo].[Taxi] ([Id])
GO

ALTER TABLE [dbo].[UserTaxi] CHECK CONSTRAINT [FK_UserTaxi_Taxi]
GO

ALTER TABLE [dbo].[UserTaxi]  WITH CHECK ADD  CONSTRAINT [FK_UserTaxi_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[UserTaxi] CHECK CONSTRAINT [FK_UserTaxi_User]
GO


