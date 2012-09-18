-- ====================== DROP CONSTRAINTS =================


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserTaxi_Taxi]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserTaxi]'))
ALTER TABLE [dbo].[UserTaxi] DROP CONSTRAINT [FK_UserTaxi_Taxi]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserTaxi_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserTaxi]'))
ALTER TABLE [dbo].[UserTaxi] DROP CONSTRAINT [FK_UserTaxi_User]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserRole_Role]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserRole]'))
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Role]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserRole_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserRole]'))
ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_User]
GO


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

-- =====================  TAXI ========================

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

-- ====================== BOOKING ======================

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
	[WaitingTime] [int] NOT NULL DEFAULT(0),
	[Active] [bit] NOT NULL,
	[Confirmed] [bit] NOT NULL,
	[Accepted] bit NOT NULL DEFAULT(0),
	[TaxiId] [int] NULL,
	[LastModified] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Hash] varchar(128) ,
	[SuburbFromId] int NULL,
	[ReferenceCode] varchar(10) NULL
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- ====================== CONTACT DETAIL ================

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

-- ====================== USER ==========================

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]
GO

CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Password] [varchar](128) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastModified] [datetime] NOT NULL,
	[LastAccess] [datetime] NULL,
	[PhoneNumber] varchar(20) NULL
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
-- ====================== ROLE  =====================================

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND type in (N'U'))
DROP TABLE [dbo].[Role]
GO

CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](32) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- ====================== SUBURB  ====================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Suburb]') AND type in (N'U'))
DROP TABLE [dbo].[Suburb]
GO

CREATE TABLE [dbo].[Suburb](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[PostalCode] [varchar](10) NOT NULL,
 CONSTRAINT [PK_Suburb] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- ===================== USER TAXI  ========================================

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

-- ========================= User Role  ==============================

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRole]') AND type in (N'U'))
DROP TABLE [dbo].[UserRole]
GO

CREATE TABLE [dbo].[UserRole](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO

ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO

ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO

-- ============================== Review  ==============================


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

-- =============================== DATA ===================================

INSERT INTO [dbo].[Role] ([Name])
     VALUES ('Admin')
INSERT INTO [dbo].[Role] ([Name])
     VALUES ('Taxi')

INSERT INTO [dbo].[User] ([Name],[Password],[Email],[Created],[LastModified],[LastAccess],[PhoneNumber])
     VALUES
           ('ninex'
           ,'d70573a4a048cc1faf256a098f6ba9848792798fa5780ce5dd6c5c852dcf6909e76ef8a74519b2802644e8d8482a4ceeeb76bee3e3580710bbd6dadeb5d51ae2'
           ,'abriegreeff@gmail.com',GETDATE(),GETDATE(), NULL, '0825098233')

INSERT INTO [dbo].[User] ([Name],[Password],[Email],[Created],[LastModified],[LastAccess],[PhoneNumber])
     VALUES
           ('taxi'
           ,'34f99930fce400ed24fd714149f3c85010f50ea3ea0489954521d3571f0011f2d5e4ccff5ef56b659903e31b7ecf4808f3428f21761820041b4363d056ff8036'
           ,'abriegreeff@gmail.com',GETDATE(),GETDATE(), NULL, '0825098233')
INSERT INTO [dbo].UserRole (RoleId, UserId)
	VALUES (1,1)
INSERT INTO [dbo].UserRole (RoleId, UserId)
	VALUES (2,2)

	     
INSERT INTO dbo.Taxi (Name, PhoneNumber, RatePerKm, MinRate, Units, StartOfService, EndOfService, Is24HService, FleetSize)
	VALUES ('Taxi A', '02188888888', 900, 2000, 50, '2012-08-03 00:00:00.000','2012-08-03 00:00:00.000', 1, 50)
INSERT INTO dbo.Taxi (Name, PhoneNumber, RatePerKm, MinRate, Units, StartOfService, EndOfService, Is24HService, FleetSize)
	VALUES ('Taxi B', '0219999999', 950, 1800, 1, '2012-08-03 00:00:00.000','2012-08-03 00:00:00.000', 1, 40)
INSERT INTO [dbo].UserTaxi (UserId, TaxiId)
	VALUES (2, 1)
--City Bowl 
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Bo-Kaap') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Bo-Kaap','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Devil''s Peak') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Devil''s Peak','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'De Waterkant') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('De Waterkant','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Foreshore') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Foreshore','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Gardens') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Gardens','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Higgovale') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Higgovale','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Lower Vrede') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Lower Vrede','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Oranjezicht') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Oranjezicht','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Salt River') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Salt River','Cape Town','7925')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Schotse Kloof') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Schotse Kloof','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Tamboerskloof') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Tamboerskloof','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'University Estate') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('University Estate','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Vredehoek') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Vredehoek','Cape Town','8001')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Walmer Estate') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Walmer Estate','Cape Town','7925')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Woodstock') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Woodstock','Cape Town','7925')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Zonnebloem') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Zonnebloem','Cape Town','7925')
-- Atlantic Seaboard
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Bantry Bay') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Bantry Bay','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Camps Bay') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Camps Bay','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Clifton') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Clifton','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Fresnaye') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Fresnaye','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Green Point') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Green Point','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Hout Bay') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Hout Bay','Cape Town','7806')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Imizamo Yethu') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Imizamo Yethu','Cape Town','7806')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Llandudno') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Llandudno','Cape Town','7806')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Mouille Point') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Mouille Point','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Sea Point') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Sea Point','Cape Town','8005')
	
IF (Select COUNT(1) FROM dbo.Suburb WHERE Name like 'Three Anchor Bay') <= 0
	INSERT INTO [dbo].[Suburb] ([Name],[City],[PostalCode])
	VALUES ('Three Anchor Bay','Cape Town','8005')

