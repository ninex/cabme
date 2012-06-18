SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Taxi]') AND type in (N'U'))
DROP Table [dbo].Taxi
GO

CREATE TABLE [dbo].[Taxi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](60) NOT NULL,
	[PhoneNumber] [varchar](20) NOT NULL,
	[RatePerKm] [int] NOT NULL,
	[MinRate] [int] NOT NULL,
	[Units] [smallint] NOT NULL,
	[StartOfService] [datetime] NOT NULL,
	[EndOfService] [datetime] NOT NULL,
	[Is24HService] [bit] NOT NULL,
	[FleetSize] [smallint] NOT NULL,
 CONSTRAINT [PK_Taxi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


