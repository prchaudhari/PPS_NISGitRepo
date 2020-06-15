DECLARE @SuperTenantIdentifier AS NVARCHAR(MAX) = N'00000000-0000-0000-0000-000000000000';
Declare @DefaultTenantIdentifier nvarchar(max) = N'0285D9B8-51F0-49E5-93E4-EF930182D246';

--For PRD Deployment
Declare @StorageAccount as nvarchar(max) = N'Data Source=nis.database.windows.net;Initial Catalog=NIS;User ID=websym;Password=Admin@123'
Declare @ConnectionString as nvarchar(max) = N'Data Source=nis.database.windows.net;Initial Catalog=NIS;User ID=websym;Password=Admin@123'

--- For Local (Test Server)
--Declare @StorageAccount as nvarchar(max) = N'Data Source=WSPL_LAP_031;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123'
--Declare @ConnectionString as nvarchar(max) = N'Data Source=WSPL_LAP_031;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123'

Insert into [TenantManager].[Tenant]
([TenantCode],[TenantName],[TenantDescription],[TenantType],[TenantImage],[TenantDomainName],[FirstName],[LastName],[ContactNumber],[EmailAddress]
,[SecondaryContactName],[SecondaryContactNumber],[SecondaryEmailAddress],[AddressLine1],[AddressLine2],[TenantCity],[TenantState],[TenantCountry]
,[PinCode],[StartDate],[EndDate],[StorageAccount],[AccessToken],[IsActive],[IsDeleted],[ApplicationURL],[ApplicationModules],[ManageType],[IsPrimaryTenant])
VALUES																																																																			 
(@SuperTenantIdentifier, N'nIS SuperAdmin',N'',N'',N'',N'default.com',N'Super',N'Admin',N'+91-1234567890',N'nvsuperadmin@nIS.com',
N'',N'',N'',N'Mumbai',N'',@MumbaiCityIdentifier,@MaharashtraStateIdentifier,@IndiaCountryIdentifier,
N'123456',N'2015-12-31 23:59:59.999',N'9999-12-31 23:59:59.999',@StorageAccount,N'',1,0,N'',N'','Self', 0),
(@DefaultTenantIdentifier, N'Customer Admin',N'',N'',N'',N'nIS.com',N'Customr',N'Admin',N'+91-1234567891',N'customeradmin@nIS.com',
N'',N'',N'',N'Mumbai',N'',@MumbaiCityIdentifier,@MaharashtraStateIdentifier,@IndiaCountryIdentifier,
N'123456',N'2015-12-31 23:59:59.999',N'9999-12-31 23:59:59.999',@StorageAccount,N'',1,0,N'',N'','Self', 0)