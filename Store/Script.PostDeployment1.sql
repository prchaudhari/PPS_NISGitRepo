/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

Declare @IndiaCountryIdentifier as bigint
Insert into NIS.Country values('India','IN','+91',1,0)
Set @IndiaCountryIdentifier = @@IDENTITY

Declare @USACountryIdentifier as bigint
Insert into NIS.Country values('USA','US','+1',1,0)
Set @USACountryIdentifier = @@IDENTITY

Declare @UnitedArabEmiratesCountryIdentifier as bigint
Insert into NIS.Country values('UAE','UAE','+971',1,0)
Set @UnitedArabEmiratesCountryIdentifier = @@IDENTITY


--- India States

Declare @MaharashtraStateIdentifier as bigint
Insert into NIS.State values('Maharashtra',@IndiaCountryIdentifier,1,0)
Set @MaharashtraStateIdentifier = @@IDENTITY

Declare @GujaratStateIdentifier as bigint
Insert into NIS.State values('Gujarat',@IndiaCountryIdentifier,1,0)
Set @GujaratStateIdentifier = @@IDENTITY

Declare @TamilNaduStateIdentifier as bigint
Insert into NIS.State values('Tamil Nadu',@IndiaCountryIdentifier,1,0)
Set @TamilNaduStateIdentifier = @@IDENTITY

Declare @UttarPradeshStateIdentifier as bigint
Insert into NIS.State values('Uttar Pradesh',@IndiaCountryIdentifier,1,0)
Set @UttarPradeshStateIdentifier = @@IDENTITY

--- USA Stetes

Declare @AlabamaStateIdentifier as bigint
Insert into NIS.State values('Alabama',@USACountryIdentifier,1,0)
Set @AlabamaStateIdentifier = @@IDENTITY

Declare @AlaskaStateIdentifier as bigint
Insert into NIS.State values('Alaska',@USACountryIdentifier,1,0)
Set @AlaskaStateIdentifier = @@IDENTITY

Declare @CaliforniaStateIdentifier as bigint
Insert into NIS.State values('California',@USACountryIdentifier,1,0)
Set @CaliforniaStateIdentifier = @@IDENTITY

Declare @FloridaStateIdentifier as bigint
Insert into NIS.State values('Florida',@USACountryIdentifier,1,0)
Set @FloridaStateIdentifier = @@IDENTITY

--- UAE

Declare @AbuDhabiStateIdentifier as bigint
Insert into NIS.State values('Abu dhabi',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @AbuDhabiStateIdentifier = @@IDENTITY

Declare @DubaiStateIdentifier as bigint
Insert into NIS.State values('Dubai',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @DubaiStateIdentifier = @@IDENTITY

Declare @AjmanStateIdentifier as bigint
Insert into NIS.State values('Ajman',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @AjmanStateIdentifier = @@IDENTITY

Declare @FujairahStateIdentifier as bigint
Insert into NIS.State values('Fujairah',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @FujairahStateIdentifier = @@IDENTITY


--- India 
 
Declare @MumbaiCityIdentifier as bigint
Insert into NIS.City values('Mumbai',@MaharashtraStateIdentifier,1,0)
Set @MumbaiCityIdentifier = @@IDENTITY

Declare @PuneCityIdentifier as bigint
Insert into NIS.City values('Pune',@MaharashtraStateIdentifier,1,0)
Set @PuneCityIdentifier = @@IDENTITY

Declare @AhmedabadCityIdentifier as bigint
Insert into NIS.City values('Ahmedabad',@GujaratStateIdentifier,1,0)
Set @AhmedabadCityIdentifier = @@IDENTITY

Declare @ChennaiCityIdentifier as bigint
Insert into NIS.City values('Chennai',@TamilNaduStateIdentifier,1,0)
Set @ChennaiCityIdentifier = @@IDENTITY

Declare @GhaziabadCityIdentifier as bigint
Insert into NIS.City values('Ghaziabad',@UttarPradeshStateIdentifier,1,0)
Set @GhaziabadCityIdentifier = @@IDENTITY

--- USA

Declare @PhenixCityIdentifier as bigint
Insert into NIS.City values('Phenix', @AlabamaStateIdentifier,1,0)
Set @PhenixCityIdentifier = @@IDENTITY

Declare @AnchorageCityIdentifier as bigint
Insert into NIS.City values('Anchorage', @AlaskaStateIdentifier,1,0)
Set @AnchorageCityIdentifier = @@IDENTITY

Declare @AdelantoCityIdentifier as bigint
Insert into NIS.City values('Adelanto', @CaliforniaStateIdentifier,1,0)
Set @AdelantoCityIdentifier = @@IDENTITY

Declare @PascoCityIdentifier as bigint
Insert into NIS.City values('Pasco', @FloridaStateIdentifier,1,0)
Set @PascoCityIdentifier = @@IDENTITY

--- UAE

Declare @DubaiCityIdentifier as bigint
Insert into NIS.City values('Dubai', @DubaiStateIdentifier,1,0)
Set @DubaiCityIdentifier = @@IDENTITY


DECLARE @SuperTenantIdentifier AS NVARCHAR(MAX) = N'00000000-0000-0000-0000-000000000000';
Declare @DefaultTenantIdentifier nvarchar(max) = N'0285D9B8-51F0-49E5-93E4-EF930182D246';

--For PRD Deployment
Declare @StorageAccount as nvarchar(max) = N'Data Source=nis.database.windows.net;Initial Catalog=nisProd;User ID=nis_admin;Password=Gauch022$'
Declare @ConnectionString as nvarchar(max) = N'Data Source=nis.database.windows.net;Initial Catalog=nisProd;User ID=nis_admin;Password=Gauch022$'

--- For Local (Test Server)
--Declare @StorageAccount as nvarchar(max) = N'Data Source=WSPL_LAP_031;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123'
--Declare @ConnectionString as nvarchar(max) = N'Data Source=WSPL_LAP_031;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123'

Insert into [TenantManager].[Tenant]
([TenantCode],[TenantName],[TenantDescription],[TenantType],[TenantImage],[TenantDomainName],[FirstName],[LastName],[ContactNumber],[EmailAddress]
,[SecondaryContactName],[SecondaryContactNumber],[SecondaryEmailAddress],[AddressLine1],[AddressLine2],[TenantCity],[TenantState],[TenantCountry]
,[PinCode],[StartDate],[EndDate],[StorageAccount],[AccessToken],[IsActive],[IsDeleted],[ApplicationURL],[ApplicationModules],[ManageType],[IsPrimaryTenant])
VALUES																																																																			 
(@SuperTenantIdentifier, N'nIS SuperAdmin',N'',N'',N'',N'default.com',N'Super',N'Admin',N'+91-1234567890',N'superadmin@nis.com',
N'',N'',N'',N'Mumbai',N'',@MumbaiCityIdentifier,@MaharashtraStateIdentifier,@IndiaCountryIdentifier,
N'123456',N'2015-12-31 23:59:59.999',N'9999-12-31 23:59:59.999',@StorageAccount,N'',1,0,N'',N'','Self', 0),
(@DefaultTenantIdentifier, N'Customer Admin',N'',N'',N'',N'nIS.com',N'Customr',N'Admin',N'+91-1234567891',N'customeradmin@nis.com',
N'',N'',N'',N'Mumbai',N'',@MumbaiCityIdentifier,@MaharashtraStateIdentifier,@IndiaCountryIdentifier,
N'123456',N'2015-12-31 23:59:59.999',N'9999-12-31 23:59:59.999',@StorageAccount,N'',1,0,N'',N'','Self', 0)

DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='superadmin@nis.com');

INSERT INTO [NIS].[TenantConfiguration]([Name],[Description],[InputDataSourcePath],[OutputHTMLPath],[OutputPDFPath],[ArchivalPath],[AssetPath],[TenantCode])
VALUES('Default Tenant Config','','','','','','',@SuperTenantCode);

INSERT INTO [NIS].[PageType]([Name],[Description],[TenantCode],[IsDeleted],[IsActive])
VALUES
('Home','',@SuperTenantIdentifier,0,1),
('Saving Account','',@SuperTenantIdentifier,0,1),
('Current Account','',@SuperTenantIdentifier,0,1)

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'nIS', N'nISConnectionString',@ConnectionString)

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EntityManager', N'EntityManagerConnectionString',@ConnectionString)

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EventManager', N'EventManagerConnectionString',@ConnectionString)

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'SubscriptionManager', N'SubscriptionManagerConnectionString',@ConnectionString)

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'TemplateManager', N'NotificationEngineConnectionString',@ConnectionString)

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'EnableSSL','false')

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'PortNumber','587')

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'PrimaryFromEmail','nis.n4mative@gmail.com')

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'PrimaryPassword','Gauch022')

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'SMTPAddress','smtp.gmail.com')

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'SecondaryFromEmail','nIS@n4mative.net')

Insert into [ConfigurationManager].[ConfigurationManager]
([PartionKey],[RowKey],[Value])
VALUES																																																																			 
(N'EmailConfiguration', N'SecondaryPassword','Gauch022')

DECLARE @SuperAdminUserIdentifier AS BIGINT
DECLARE @SuperAdminEmailAddress AS NVARCHAR(100) = N'superadmin@nis.com';
INSERT INTO [NIS].[User] VALUES ('NIS', 'SuperAdmin', '+91-1234567890', @SuperAdminEmailAddress, '', 0, 0, 1,0, @SuperTenantIdentifier)
SET @SuperAdminUserIdentifier = @@IDENTITY

INSERT INTO [NIS].[UserLogin] VALUES (@SuperAdminEmailAddress, 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())

DECLARE @RoleIdentifier AS BIGINT
INSERT [NIS].[Role] ([Name], [Description], [IsDeleted], [TenantCode]) VALUES (N'Super Admin', N'Super Admin Role', 0, @SuperTenantIdentifier)
SET @RoleIdentifier = @@IDENTITY

INSERT INTO [NIS].[UserRoleMap] ([UserId], [RoleId]) VALUES (@SuperAdminUserIdentifier, @RoleIdentifier)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Dashboard', N'View', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Widget', N'View', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Publish', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Publish', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Search', N'View', 1)

--DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
--DECLARE @SuperTenantId AS NVARCHAR(MAX) = (select id from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @DateTime as DateTime =(SELECT GETDATE());


--1-Home
--2-Saving Account
--3-Current Account


INSERT INTO [NIS].[Widget] ([PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable])
VALUES
('2,3', 'Customer Information Details', 'CustomerInformation', 'Customer Information', 0, @SuperTenantCode, 0,1, @DateTime, @SuperAdminUserIdentifier, 0),
('1', 'Account Details', 'AccountInformation','Account Information',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('1', 'Summary at Glance Details', 'Summary','Summary at Glance',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('1,2,3', 'Marketing widget - Configuration for image and click through URL', 'Image','Image',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,1),
('1,2,3','Customer Information - Allowing to upload video','Video','Video',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,1),
('1','Customer Account Analytics Details', 'Analytics','Analytics',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('2','Saving Transaction Details','SavingTransaction','Saving Transaction',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('3', 'Current Transaction Details','CurrentTransaction','Current Transaction',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('2','Customer Saving Trend chart','SavingTrend','Saving Trend',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('2', 'Customer Top 4 Income Sources details','Top4IncomeSources','Top 4 Income Sources',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('3', 'Current : Available Balance Details', 'CurrentAvailableBalance','Current : Available Balance',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('2', 'Saving : Available Balance Details', 'SavingAvailableBalance','Saving : Available Balance',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('1', 'Reminder and Recommendation details', 'ReminderaAndRecommendation','Reminder & Recommendation',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0),
('3', 'Customer Sprending trend chart', 'SpendingTrend','Spending Trend',0,@SuperTenantCode,0,1,@DateTime,@SuperAdminUserIdentifier,0)



SET IDENTITY_INSERT EntityManager.Entities ON 
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (1, N'Dashboard', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Dashboard')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (2, N'User', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'User')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (3, N'Role', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Role')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (4, N'Asset Library', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Asset Library')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (5, N'Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Widget')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (6, N'Page', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Page')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (7, N'Statement Definition', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Statement Definition')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (8, N'Schedule Management', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Schedule Management')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (9, N'Log', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Log')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (10, N'Analytics', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Analytics')
INSERT EntityManager.Entities ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (11, N'Statement Search', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Statement Search')
SET IDENTITY_INSERT EntityManager.Entities OFF

SET IDENTITY_INSERT EntityManager.Operations ON 

INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (1, N'00000000-0000-0000-0000-000000000000', N'Dashboard', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (2, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Create')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (3, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Edit')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (4, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Delete')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (5, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (6, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Create')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (7, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Edit')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (8, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Delete')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (9, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (10, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Create')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (11, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Edit')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (12, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Delete')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (13, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (14, N'00000000-0000-0000-0000-000000000000', N'Widget', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (15, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Create')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (16, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Edit')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (17, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Delete')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (18, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (19, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Publish')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (20, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Create')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (21, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Edit')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (22, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Delete')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (23, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (24, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Publish')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (25, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Create')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (26, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Edit')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (27, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Delete')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (28, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (29, N'00000000-0000-0000-0000-000000000000', N'Log', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (30, N'00000000-0000-0000-0000-000000000000', N'Analytics', N'nIS', N'View')
INSERT EntityManager.Operations ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (31, N'00000000-0000-0000-0000-000000000000', N'Statement Search', N'nIS', N'View')

SET IDENTITY_INSERT EntityManager.Operations OFF


INSERT INTO [NIS].[AssetSetting]
([ImageHeight],[ImageWidth],[ImageSize],[ImageFileExtension],[VideoSize],[VideoFileExtension],[TenantCode])
VALUES(300,400,1,'png,jpeg',2,'mp4',@SuperTenantCode);
