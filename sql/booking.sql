
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Booking]') AND type in (N'U'))
DROP TABLE [dbo].[Booking]
GO

CREATE TABLE [dbo].[Booking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](60) NULL,
	[PhoneNumber] [varchar](20) NOT NULL,
	[NumberOfPeople] [smallint] NOT NULL,
	[PickupTime] [datetime] NOT NULL,
	[AddrFrom] [varchar](400) NOT NULL,
	[LatitudeFrom] [int] NULL,
	[LongitudeFrom] [int] NULL,
	[AddrTo] [varchar](400) NULL,
	[LatitudeTo] [int] NULL,
	[LongitudeTo] [int] NULL,
	[ComputedDistance] [int] NOT NULL,
	[EstimatedPrice] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Confirmed] [bit] NOT NULL,
	[TaxiId] [int] NULL,
	[LastModified] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Hash] varchar(128) 
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


