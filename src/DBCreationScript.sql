use [master];
go

-- Drop database
if DB_ID('MyLocations') is not null drop database MyLocations;

-- If database could not be created due to open connections, abort
if @@ERROR = 3702 
   RAISERROR('Database cannot be dropped because there are still open connections.', 127, 127) WITH NOWAIT, LOG;
go

create database [MyLocations];
go

use [MyLocations];
go

------------------------------------------------------
-- Create tables
------------------------------------------------------

-- We define an in-house table for location category (Not necessarily associated with FoursquareApi)
-- If we use google api in the future, this table will hold categories from google api
-- Currently, foursquare places api does not provide an endpoint for retrieving the categories
-- Thus we get the list of categories from here : https://docs.foursquare.com/data-products/docs/categories
-- We insert only parent categories to keep the list short
-- As an improvement, we can eventually insert child-categories for more accurate results
create table [dbo].[LocationCategory](
	[Id]   int  -- Integer Id from Foursquare api
   ,[Name] nvarchar(100) not null
   ,constraint PK_LocationCategory primary key ([Id])
);

create table [dbo].[Location](
	[Id]         int           identity(1,1)
   ,[Keyword]    nvarchar(100)  not null
   ,[Name]       nvarchar(100)  null
   ,[Region]     nvarchar(100)  null
   ,[Address]    nvarchar(100)  null
   ,[CategoryId] int            not null
   ,[CreatedAt]  datetime       not null
   ,[Latitude]   float          not null
   ,[Longitude]  float          not null
   ,constraint PK_Location primary key ([Id])
   ,constraint UK_Location unique ([Latitude], [Longitude])
   ,constraint FK_Location_LocationCategory_CategoryId foreign key ([CategoryId]) references [LocationCategory]([Id])
);

-- Store the image detail
create table [dbo].[Image](
	[Id]          nvarchar(100) not null
   ,[Description] nvarchar(100) null
   ,[Url]         nvarchar(100) not null
   ,constraint PK_Image primary key ([Id])
   ,constraint UK_Image unique ([Url])
);

-- Many to many relationship table
-- In our system, a location can have multiple images
-- An image can also be associated with multiple locations 
-- For example image for Anfield football stadium can be associated with Liverpool location and England location
create table [dbo].[LocationImage](
	[LocationId] int          not null
   ,[ImageId]    nvarchar(100) not null
   ,constraint PK_LocationImage primary key ([LocationId], [ImageId])
   ,constraint FK_LocationImage_Location_LocationId foreign key ([LocationId]) references [Location]([Id])
   ,constraint FK_LocationImage_Image_ImageId foreign key ([ImageId]) references [Image]([Id])
);

create table [dbo].[Setting](
	[Key]   nvarchar(100) not null
   ,[Value] nvarchar(100) not null
   ,constraint PK_Setting primary key ([Key])
);

--create table [dbo].[NonAuthenticatedUserLocation](
--	[Id] int 
--	,constraint pk_DefaultLocation primary key ([Id])
--	,constraint fk_DefaultLocation_Location foreign key ([Id]) references [Location]([Id])
--);

--create table [dbo].[User](
--	[Id] int identity(1,1)
--   ,[FirstName] varchar(100) not null
--   ,[LastName]  varchar(100) not null
--   ,constraint pk_User primary key ([Id])
--);

--create table [dbo].[AuthenticatedUserLocation](
--	 [LocationId] int not null
--	,[UserId]     int not null
--	,constraint pk_UserDefinedLocation primary key ([LocationId], [UserId])
--	,constraint fk_UserDefinedLocation_Location foreign key ([LocationId]) references [Location]([Id])
--	,constraint fk_UserDefinedLocation_User foreign key ([UserId]) references [User]([Id])
--);

--create table [dbo].[UserCredentials](
--	[UserId] int not null
--   ,[Username] varchar(255) not null
--   ,[Password] varchar(100) not null
--   ,constraint pk_UserCredentials primary key ([Username])
--   ,constraint fk_UserCredentials_User foreign key ([UserId]) references [User]([Id])
--);

------------------------------------------------------
-- Populate tables
------------------------------------------------------

-- From foursquare : https://docs.foursquare.com/data-products/docs/categories
Insert into [dbo].[LocationCategory] ([Id], [Name])
values
	('10000', 'Arts and Entertainment')
   ,('11000', 'Business and Professional Services')
   ,('12000', 'Community and Government')
   ,('13000', 'Dining and Drinking')
   ,('14000', 'Event')
   ,('15000', 'Health and Medicine')
   ,('16000', 'Landmarks and Outdoors')
   ,('17000', 'Retail')
   ,('18000', 'Sports and Recreation')
   ,('19000', 'Travel and Transportation');
go

insert into [dbo].[Location] ([Keyword],[Name],[Region],[Address],[CategoryId],[CreatedAt],[Latitude],[Longitude])
values 
  ('anfield stadium', 'Anfield', 'liverpool ', 'Anfield Rd, Liverpool, L4 0TH', '10000', GETUTCDATE(), 53.430299, -2.961604)
, ('paris', 'La Grande Epicerie Paris', 'france ', '38 rue de Sèvres, 75007 Paris', '17000', GETUTCDATE(), 48.850447, 2.323939)
, ('caudan waterfront', 'Cafe LUX* Caudan Waterfront', 'port-louis', 'Caudan Waterfront, Port Louis', '13000', GETUTCDATE(), -20.161142, 57.499194)
, ('bagatelle', 'bagatelle', 'mauritius', '', '10000', GETUTCDATE(), -20.225672, 57.470806)
, ('caudan', 'Dias Pier Parking (Caudan)', 'port-louis', 'M2, Port Louis', '19000', GETUTCDATE(), -20.161401, 57.501301);

insert into [dbo].[Image] ([Id], [Description], [Url])
values
	('1746-40815616290-25bcf946e6', 'Anfield Stadium', 'https://live.staticflickr.com/1746/40815616290_25bcf946e6.jpg')
   ,('4200-34865586665-52a8c536a2', 'Anfield', 'https://live.staticflickr.com/4200/34865586665_52a8c536a2.jpg')
   ,('4244-34734338181-0df24268ab', 'Anfield', 'https://live.staticflickr.com/4244/34734338181_0df24268ab.jpg')
   ,('4842-32167521438-affe751274', 'Port-Louis : Charlot au Caudan-Waterfront', 'https://live.staticflickr.com/4842/32167521438_affe751274.jpg')
   ,('4868-32422633428-f6e688a0b3', 'Port-Louis : l''Alerre devant le Caudan Waterfront 1', 'https://live.staticflickr.com/4868/32422633428_f6e688a0b3.jpg')
   ,('4893-32154776988-9dc0379d00', 'Port-Louis : parapluies au Caudan Waterfront', 'https://live.staticflickr.com/4893/32154776988_9dc0379d00.jpg')
   ,('4911-32167456758-4b91d431f2', 'Port-Louis : parapluies au Caudan Waterfront', 'https://live.staticflickr.com/4911/32167456758_4b91d431f2.jpg')
   ,('65535-48130548756-3b35efb353', 'Domes of Sacré-Cœur in the soft evening light, Paris', 'https://live.staticflickr.com/65535/48130548756_3b35efb353.jpg')
   ,('65535-49675956921-766a211f36', 'Mauritius, Moka, Bagatelle IMG_20200312_170253', 'https://live.staticflickr.com/65535/49675956921_766a211f36.jpg')
   ,('65535-49675958061-612983c19d', 'Mauritius, Moka, Bagatelle Mall IMG_20200312_144015', 'https://live.staticflickr.com/65535/49675958061_612983c19d.jpg')
   ,('65535-49675959516-64c7d3ef34', 'Mauritius, Moka, Bagatelle IMG_5457', 'https://live.staticflickr.com/65535/49675959516_64c7d3ef34.jpg')
   ,('65535-50881769216-6e629b7a04', 'Arc de Triomphe, Paris, France 2020', 'https://live.staticflickr.com/65535/50881769216_6e629b7a04.jpg')
   ,('65535-52589545962-20fbe35804', 'L''église de la Madeleine', 'https://live.staticflickr.com/65535/52589545962_20fbe35804.jpg')
   ,('7869-46534012624-f11c29427c', 'Port Louis.Mauritius', 'https://live.staticflickr.com/7869/46534012624_f11c29427c.jpg')
   ,('7894-40291051193-61ff5d9e43', 'Port Louis.Mauritius', 'https://live.staticflickr.com/7894/40291051193_61ff5d9e43.jpg')
   ,('7907-47268732161-585b7ab750', 'Port Louis.PEM', 'https://live.staticflickr.com/7907/47268732161_585b7ab750.jpg')
   ,('874-40216895535-45801fa62a', 'Anfield', 'https://live.staticflickr.com/874/40216895535_45801fa62a.jpg')
   ,('933-43139542024-69c981f3d1', 'Catacombes, Paris, France', 'https://live.staticflickr.com/933/43139542024_69c981f3d1.jpg')
   ,('961-41568892954-93bd9fba7e', 'Bagatelle Mall', 'https://live.staticflickr.com/961/41568892954_93bd9fba7e.jpg');
	
insert into [dbo].[LocationImage] ([LocationId], [ImageId])
values 
	(1, '1746-40815616290-25bcf946e6')
	,(1, '4200-34865586665-52a8c536a2')
	,(1, '4244-34734338181-0df24268ab')
	,(1, '874-40216895535-45801fa62a')
	,(2, '65535-48130548756-3b35efb353')
	,(2, '65535-50881769216-6e629b7a04')
	,(2, '65535-52589545962-20fbe35804')
	,(2, '933-43139542024-69c981f3d1')
	,(3, '4842-32167521438-affe751274')
    ,(3, '4868-32422633428-f6e688a0b3')
	,(3, '4893-32154776988-9dc0379d00')
	,(3, '4911-32167456758-4b91d431f2')
	,(4, '65535-49675956921-766a211f36')
	,(4, '65535-49675958061-612983c19d')
	,(4, '65535-49675959516-64c7d3ef34')
	,(4, '961-41568892954-93bd9fba7e')
	,(5, '7869-46534012624-f11c29427c')
	,(5, '7894-40291051193-61ff5d9e43')
	,(5, '7907-47268732161-585b7ab750');

insert into [dbo].[Setting] ([Key], [Value])
values 
	('FoursquareApiKey', '')
   ,('FlickApiKey', '')
   ,('FlickApiSecret', '');
go


