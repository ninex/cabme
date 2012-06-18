
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactDetail]') AND type in (N'U'))
DROP TABLE [dbo].[ContactDetail]
GO

CREATE TABLE [dbo].[ContactDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaxiId] [int] NOT NULL,
	[PhoneNumber] [varchar](20) NULL,
	[BookingEmail] [varchar](255) NULL,
	[BookingSMS] [varchar](20) NULL,
	[UseEmail] [bit] NOT NULL,
 CONSTRAINT [PK_ContactDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ContactDetail]  WITH CHECK ADD  CONSTRAINT [FK_ContactDetail_ContactDetail] FOREIGN KEY([TaxiId])
REFERENCES [dbo].[Taxi] ([Id])
GO

ALTER TABLE [dbo].[ContactDetail] CHECK CONSTRAINT [FK_ContactDetail_ContactDetail]
GO


