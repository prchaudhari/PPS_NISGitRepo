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

truncate table [ConfigurationManager].[ConfigurationManager];
truncate table [EntityManager].[DependentOperations];
truncate table [EntityManager].[Entities];
truncate table [EntityManager].[Operations];
truncate table [TenantManager].[Tenant];
truncate table [NIS].[AccountMaster];
truncate table [NIS].[AccountTransaction];
truncate table [NIS].[AnalyticsData];
truncate table [NIS].[Asset];
truncate table [NIS].[AssetLibrary];
truncate table [NIS].[AssetPathSetting];
truncate table [NIS].[AssetSetting];
truncate table [NIS].[BatchDetails];
truncate table [NIS].[BatchMaster];
truncate table [NIS].[City];
truncate table [NIS].[ContactType];
truncate table [NIS].[Country];
truncate table [NIS].[CustomerInfo];
truncate table [NIS].[CustomerMaster];
truncate table [NIS].[CustomerMedia];
truncate table [NIS].[DynamicWidget];
truncate table [NIS].[DynamicWidgetFilterDetail];
truncate table [NIS].[EntityFieldMap];
truncate table [NIS].[Image];
truncate table [NIS].[MultiTenantUserAccessMap];
truncate table [NIS].[NewsAlert];
truncate table [NIS].[Page];
truncate table [NIS].[PageType];
truncate table [NIS].[PageWidgetMap];
truncate table [NIS].[ReminderAndRecommendation];
truncate table [NIS].[RenderEngine];
truncate table [NIS].[Role];
truncate table [NIS].[RolePrivilege];
truncate table [NIS].[SavingTrend];
truncate table [NIS].[Schedule];
truncate table [NIS].[ScheduleLog];
truncate table [NIS].[ScheduleLogDetail];
truncate table [NIS].[ScheduleRunHistory];
truncate table [NIS].[State];
truncate table [NIS].[Statement];
truncate table [NIS].[StatementAnalytics];
truncate table [NIS].[StatementMetadata];
truncate table [NIS].[StatementPageMap];
truncate table [NIS].[TenantConfiguration];
truncate table [NIS].[TenantContact];
truncate table [NIS].[TenantEntity];
truncate table [NIS].[TenantUser];
truncate table [NIS].[Top4IncomeSources];
truncate table [NIS].[TransactionDetail];
truncate table [NIS].[TTD_CustomerMaster];
truncate table [NIS].[TTD_DataUsage];
truncate table [NIS].[TTD_EmailsBySubscription];
truncate table [NIS].[TTD_MeetingUsage];
truncate table [NIS].[TTD_SubscriptionMaster];
truncate table [NIS].[TTD_SubscriptionSpend];
truncate table [NIS].[TTD_SubscriptionSummary];
truncate table [NIS].[TTD_SubscriptionUsage];
truncate table [NIS].[TTD_UserSubscriptions];
truncate table [NIS].[TTD_VendorSubscription];
truncate table [NIS].[User];
truncate table [NIS].[UserCredentialHistory];
truncate table [NIS].[UserLogin];
truncate table [NIS].[UserLoginActivityHistory];
truncate table [NIS].[UserRoleMap];
truncate table [NIS].[Video];
truncate table [NIS].[Widget];
truncate table [NIS].[WidgetPageTypeMap];

SET IDENTITY_INSERT [ConfigurationManager].[ConfigurationManager] ON 

INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (1, N'nIS', N'nISConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (2, N'EntityManager', N'EntityManagerConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (3, N'EventManager', N'EventManagerConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (4, N'SubscriptionManager', N'SubscriptionManagerConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (5, N'TemplateManager', N'NotificationEngineConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (6, N'EmailConfiguration', N'EnableSSL', N'false')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (7, N'EmailConfiguration', N'PortNumber', N'587')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (8, N'EmailConfiguration', N'PrimaryFromEmail', N'nis@n4mative.net')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (9, N'EmailConfiguration', N'PrimaryPassword', N'Gauch022')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (10, N'EmailConfiguration', N'SMTPAddress', N'smtp.gmail.com')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (11, N'EmailConfiguration', N'SecondaryFromEmail', N'nis@n4mative.net')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (12, N'EmailConfiguration', N'SecondaryPassword', N'Gauch022')
SET IDENTITY_INSERT [ConfigurationManager].[ConfigurationManager] OFF
SET IDENTITY_INSERT [EntityManager].[Entities] ON 

INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (1, N'Dashboard', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Dashboard')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (2, N'User', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'User')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (3, N'Role', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Role')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (4, N'Asset Library', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Asset Library')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (5, N'Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Widget')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (6, N'Page', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Page')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (7, N'Statement Definition', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Statement Definition')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (8, N'Schedule Management', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Schedule Management')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (9, N'Log', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Log')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (10, N'Analytics', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Analytics')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (11, N'Statement Search', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Statement Search')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (12, N'Tenant', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Tenant')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (13, N'Dynamic Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Dynamic Widget')
SET IDENTITY_INSERT [EntityManager].[Entities] OFF
SET IDENTITY_INSERT [EntityManager].[Operations] ON 

INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (1, N'00000000-0000-0000-0000-000000000000', N'Dashboard', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (2, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (3, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (4, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (5, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (6, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (7, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (8, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (9, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (10, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (11, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (12, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (13, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (14, N'00000000-0000-0000-0000-000000000000', N'Widget', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (15, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (16, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (17, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (18, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (19, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Publish')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (20, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (21, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (22, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (23, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (24, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Publish')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (25, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (26, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (27, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (28, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (29, N'00000000-0000-0000-0000-000000000000', N'Log', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (30, N'00000000-0000-0000-0000-000000000000', N'Analytics', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (31, N'00000000-0000-0000-0000-000000000000', N'Statement Search', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (32, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (33, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (34, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (35, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (36, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Reset Password')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (37, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (38, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (39, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (40, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (41, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Publish')
SET IDENTITY_INSERT [EntityManager].[Operations] OFF

SET IDENTITY_INSERT [NIS].[ContactType] ON 
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'Primary', N'Teat', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'Secondary', N'Test', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'Primary', N'Teat', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'Secondary', N'Test', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'Primary', N'Teat', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (6, N'Secondary', N'Test', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
SET IDENTITY_INSERT [NIS].[ContactType] OFF

SET IDENTITY_INSERT [NIS].[Country] ON 

INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'India', N'IN', N'+91', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'India', N'IN', N'+91', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'India', N'IN', N'+91', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'India', N'IN', N'+91', 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'India', N'IN', N'+91', 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[Country] OFF

SET IDENTITY_INSERT [NIS].[EntityFieldMap] ON 

INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (1, N'CutomerCode', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (2, N'FirstName', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (3, N'LastName', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (4, N'RMName', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (5, N'RMContactNo', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (6, N'AccountNumber', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (7, N'AccountType', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (8, N'Balance', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (9, N'TotalDeposit', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (10, N'TotalSpend', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (11, N'TransactionDate', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'DateTime')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (12, N'TransactionType', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (13, N'Narration', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (14, N'AccountType', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (15, N'FCY', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (16, N'LCY', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (17, N'Month', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (18, N'SpendAmount', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (19, N'SpendPercentage', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (20, N'Income', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (21, N'IncomePercentage', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (22, N'VendorName', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (23, N'Subscription', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (24, N'EmployeeID', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (25, N'EmployeeName', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (26, N'Email', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (27, N'StartDate', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'DateTime')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (28, N'EndDate', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'DateTime')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (29, N'Vendor', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (30, N'Subscription', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (31, N'Total', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (32, N'AverageSpend', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (33, N'Month', 7, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (34, N'Microsoft', 7, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (35, N'Zoom', 7, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (36, N'UserName', 8, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (37, N'CountOfSubscription', 8, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (38, N'VenderName', 9, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (39, N'CountOfSubscription', 9, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (40, N'EmployeeID', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (41, N'Subscription', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (42, N'VendorName', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (43, N'EmployeeName', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (44, N'Email', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (45, N'Usage', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (46, N'Emails', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (47, N'Meetings', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (48, N'Month', 11, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (49, N'Microsoft', 11, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (50, N'Zoom', 11, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (51, N'Month', 12, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (52, N'Microsoft', 12, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (53, N'Zoom', 12, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (54, N'Subscription', 13, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (55, N'Emails', 13, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
SET IDENTITY_INSERT [NIS].[EntityFieldMap] OFF

SET IDENTITY_INSERT [NIS].[PageType] ON 
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (1, N'Home', N'Home pages', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (2, N'Saving Account', N'Saving Account Page Type', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (3, N'Current Account', N'Current Account Page Type', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (4, N'Home', N'Home pages', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (5, N'Saving Account', N'Saving Account Page Type', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (6, N'Current Account', N'Current Account Page Type', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (7, N'Usage', N'Home pages', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (8, N'Billing', N'Saving Account Page Type', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 0, 1)
SET IDENTITY_INSERT [NIS].[PageType] OFF

SET IDENTITY_INSERT [NIS].[Role] ON 
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (1, N'Super Admin', N'Super Admin Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (2, N'Tenant Admin', N'Tenant Admin Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (3, N'Group Manager', N'Group Manager Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (4, N'Instance Manager', N'Instance Manager', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (5, N'Group Manager', N'Group Manager Role', 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (6, N'Tenant Admin', N'Tenant Admin Role', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (7, N'Group Manager', N'Group Manager Role', 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (8, N'Tenant Admin', N'Tenant Admin Role', 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[Role] OFF

SET IDENTITY_INSERT [NIS].[RolePrivilege] ON 

INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, 1, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (2, 1, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (3, 1, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (4, 1, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (5, 1, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, 1, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (7, 1, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, 1, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (9, 1, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (10, 1, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (11, 1, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (12, 1, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (13, 1, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (14, 1, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (15, 1, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (16, 1, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (17, 1, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (18, 1, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (19, 1, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (20, 1, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (21, 1, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (22, 1, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (23, 1, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (24, 1, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (25, 1, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (26, 1, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (27, 1, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (28, 1, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (29, 1, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (30, 1, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (31, 1, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (32, 1, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (33, 1, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (34, 1, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (35, 1, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (36, 1, N'Dynamic Widget', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (37, 6, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (38, 6, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (39, 6, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (40, 6, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (41, 6, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (42, 6, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (43, 6, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (44, 6, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (45, 6, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (46, 6, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (47, 6, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (48, 6, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (49, 6, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (50, 6, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (51, 6, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (52, 6, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (53, 6, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (54, 6, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (55, 6, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (56, 6, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (57, 6, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (58, 6, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (59, 6, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (60, 6, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (61, 6, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (62, 6, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (63, 6, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (64, 6, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (65, 6, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (66, 6, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (67, 6, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (68, 6, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (69, 6, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (70, 6, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (71, 6, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (72, 6, N'Dynamic Widget', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (73, 8, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (74, 8, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (75, 8, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (76, 8, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (77, 8, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (78, 8, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (79, 8, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (80, 8, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (81, 8, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (82, 8, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (83, 8, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (84, 8, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (85, 8, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (86, 8, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (87, 8, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (88, 8, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (89, 8, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (90, 8, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (91, 8, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (92, 8, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (93, 8, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (94, 8, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (95, 8, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (96, 8, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (97, 8, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (98, 8, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (99, 8, N'Schedule Management', N'Delete', 1)
GO
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (100, 8, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (101, 8, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (102, 8, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (103, 8, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (104, 8, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (105, 8, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (106, 8, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (107, 8, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (108, 8, N'Dynamic Widget', N'Publish', 1)
SET IDENTITY_INSERT [NIS].[RolePrivilege] OFF

SET IDENTITY_INSERT [NIS].[TenantConfiguration] ON 
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (1, N'TEST', N'', N'', N'', N'', N'', N'', N'00000000-0000-0000-0000-000000000000', NULL, NULL, N'MM/dd/yyyy', N'Theme1', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman","HeaderColor":"#5ccc60","HeaderSize":14,"HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":12,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (2, N'TEST', N'', N'', N'', N'', N'', N'\\WSPL_LAP_012\NISAssets', N'fd51e101-35e5-49b4-ac29-1224d278e430', NULL, NULL, N'MM/dd/yyyy', N'Theme0', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman","HeaderColor":"#5ccc60","HeaderSize":14,"HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":12,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (3, N'TEST', N'', N'', N'', N'', N'', N'\\WSPL_LAP_012\NISAssets', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', NULL, NULL, N'MM/dd/yyyy', N'Theme0', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman","HeaderColor":"#5ccc60","HeaderSize":14,"HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":12,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
SET IDENTITY_INSERT [NIS].[TenantConfiguration] OFF

SET IDENTITY_INSERT [NIS].[TenantEntity] ON 
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (1, N'Customer Information', N'', CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_CustomerMasters', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (2, N'Account Balalnce', N'', CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_AccountMaster', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (3, N'Account Transaction', N'', CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_AccountTransaction', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (4, N'Saving Trend', N'', CAST(N'2020-11-09 11:30:14.500' AS DateTime), 5, CAST(N'2020-11-09 11:30:14.500' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_SavingTrend', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (5, N'Subscription Master', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionMasters', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (6, N'Subscription Summary', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionSummaries', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (7, N'Subscription Spend', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionSpends', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (8, N'User Subscriptions', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_UserSubscriptions', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (9, N'Vendor Subscription', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_VendorSubscriptions', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (10, N'Subscription Usage', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionUsages', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (11, N'Data Usage', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_DataUsages', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (12, N'Meeting Usage', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_MeetingUsages', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (13, N'Emails By Subscription', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_EmailsBySubscription', N'POST')
SET IDENTITY_INSERT [NIS].[TenantEntity] OFF
SET IDENTITY_INSERT [NIS].[User] ON 

INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (1, N'NIS', N'SuperAdmin', N'7878322333', N'instancemanager@nis.com', N'', 0, 0, 1, 0, N'00000000-0000-0000-0000-000000000000', 1, 1, 0, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (2, N'Tenant', N'UK Group', N'7878322334', N'pramod.shinde45123@gmail.com', N'', 0, 0, 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 2, 0, 1, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (3, N'UK', N'Tenant Admin', N'7878322335', N'tenantuk@demo.com', N'', 0, 0, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 3, 0, 0, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (4, N'SS', N'Tenant Group', N'7878322336', N'ss_group@mailinator.com', N'', 0, 0, 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 4, 0, 1, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (5, N'SS', N'Websym', N'7878322337', N'sswebsym@mailinator.com', N'', 0, 0, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 5, 0, 0, 0)
SET IDENTITY_INSERT [NIS].[User] OFF
SET IDENTITY_INSERT [NIS].[UserCredentialHistory] ON 

INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (1, N'instancemanager@nis.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-07-26 06:33:32.657' AS DateTime), N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (2, N'pramod.shinde45123@gmail.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:45:54.327' AS DateTime), N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (3, N'tenantuk@demo.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:51:03.803' AS DateTime), N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (4, N'ss_group@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:45:54.327' AS DateTime), N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (5, N'sswebsym@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:51:03.803' AS DateTime), N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[UserCredentialHistory] OFF
SET IDENTITY_INSERT [NIS].[UserLogin] ON 

INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (1, N'instancemanager@nis.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.420' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (2, N'pramod.shinde45123@gmail.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.423' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (3, N'tenantuk@demo.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.427' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (4, N'ss_group@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.427' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (5, N'sswebsym@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.427' AS DateTime))
SET IDENTITY_INSERT [NIS].[UserLogin] OFF
SET IDENTITY_INSERT [NIS].[UserLoginActivityHistory] ON 

INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'1', N'LogIn', CAST(N'2020-11-18 18:05:30.793' AS DateTime), 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'3', N'LogIn', CAST(N'2020-11-18 18:05:56.023' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'3', N'LogIn', CAST(N'2020-11-18 18:06:08.997' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'2', N'LogIn', CAST(N'2020-11-18 18:06:21.860' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'2', N'LogIn', CAST(N'2020-11-19 03:20:34.687' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (6, N'1', N'LogIn', CAST(N'2020-11-19 03:21:00.660' AS DateTime), 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (7, N'3', N'LogIn', CAST(N'2020-11-19 03:21:17.187' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (8, N'3', N'LogIn', CAST(N'2020-11-19 03:29:16.973' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (9, N'3', N'LogIn', CAST(N'2020-11-19 03:29:31.673' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (10, N'3', N'LogIn', CAST(N'2020-11-19 03:38:37.990' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (11, N'3', N'LogIn', CAST(N'2020-11-19 03:38:59.030' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (12, N'3', N'LogIn', CAST(N'2020-11-19 03:43:06.073' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (13, N'3', N'LogIn', CAST(N'2020-11-19 03:51:12.813' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (14, N'3', N'LogIn', CAST(N'2020-11-19 03:53:46.347' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (15, N'3', N'LogIn', CAST(N'2020-11-19 03:54:37.443' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (16, N'2', N'LogIn', CAST(N'2020-11-19 03:57:18.447' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (17, N'3', N'LogIn', CAST(N'2020-11-19 03:57:52.113' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (18, N'2', N'LogIn', CAST(N'2020-11-19 04:03:08.810' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (19, N'5', N'LogIn', CAST(N'2020-11-19 04:06:21.347' AS DateTime), 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (20, N'3', N'LogIn', CAST(N'2020-11-19 04:13:01.550' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (21, N'3', N'LogIn', CAST(N'2020-11-19 04:56:07.857' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (22, N'3', N'LogIn', CAST(N'2020-11-19 06:43:29.293' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (23, N'3', N'LogIn', CAST(N'2020-11-19 06:44:18.343' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (24, N'3', N'LogIn', CAST(N'2020-11-19 07:13:24.863' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (25, N'3', N'LogIn', CAST(N'2020-11-19 07:13:39.497' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (26, N'5', N'LogIn', CAST(N'2020-11-19 07:28:40.550' AS DateTime), 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (27, N'3', N'LogIn', CAST(N'2020-11-19 08:03:46.217' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (28, N'5', N'LogIn', CAST(N'2020-11-19 08:14:15.343' AS DateTime), 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (29, N'3', N'LogIn', CAST(N'2020-11-19 08:20:18.667' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
SET IDENTITY_INSERT [NIS].[UserLoginActivityHistory] OFF
SET IDENTITY_INSERT [NIS].[UserRoleMap] ON 

INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (1, 1, 1)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (2, 3, 6)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (3, 5, 8)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (4, 2, 3)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (5, 4, 3)
SET IDENTITY_INSERT [NIS].[UserRoleMap] OFF
SET IDENTITY_INSERT [NIS].[Widget] ON 

INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (1, N'4', N'Customer Information Details', N'CustomerInformation', N'Customer Information', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (2, N'4', N'Account Details', N'AccountInformation', N'Account Information', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (3, N'4', N'Summary at Glance Details', N'Summary', N'Summary at Glance', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (4, N'4,5,6', N'Marketing widget - Configuration for image and click through URL', N'Image', N'Image', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 1)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (5, N'4,4,6', N'Customer Information - Allowing to upload video', N'Video', N'Video', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 1)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (6, N'4', N'Customer Account Analytics Details', N'Analytics', N'Analytics', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (7, N'4', N'Saving Transaction Details', N'SavingTransaction', N'Saving Transaction', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (8, N'6', N'Current Transaction Details', N'CurrentTransaction', N'Current Transaction', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (9, N'4', N'Customer Saving Trend chart', N'SavingTrend', N'Saving Trend', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (10, N'4', N'Customer Top 4 Income Sources details', N'Top4IncomeSources', N'Top 4 Income Sources', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (11, N'6', N'Current : Available Balance Details', N'CurrentAvailableBalance', N'Current : Available Balance', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (12, N'4', N'Saving : Available Balance Details', N'SavingAvailableBalance', N'Saving : Available Balance', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (13, N'4', N'Reminder and Recommendation details', N'ReminderaAndRecommendation', N'Reminder & Recommendation', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (14, N'6', N'Customer Sprending trend chart', N'SpendingTrend', N'Spending Trend', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
SET IDENTITY_INSERT [NIS].[Widget] OFF
SET IDENTITY_INSERT [NIS].[WidgetPageTypeMap] ON 

INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (1, 1, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (2, 2, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (3, 3, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (4, 4, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (5, 4, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (6, 4, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (7, 5, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (8, 5, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (9, 5, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (10, 6, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (11, 7, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (12, 8, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (13, 9, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (14, 10, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (15, 11, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (16, 12, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (17, 13, 3, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (18, 14, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (19, 1, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (20, 2, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (21, 2, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (22, 3, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (23, 3, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (24, 3, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
SET IDENTITY_INSERT [NIS].[WidgetPageTypeMap] OFF


INSERT [TenantManager].[Tenant] VALUES ( N'00000000-0000-0000-0000-000000000000', N'nIS SuperAdmin', N'', N'Instance', N'', N'default.com', N'Super', N'Admin', N'+91-1234567890', N'instancemanager@nis.com', N'', N'', N'', N'Mumbai', N'', N'1', N'1', N'1', N'123456', CAST(N'2015-12-31' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123', N'', N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'Self', NULL, NULL, 1, 0, NULL, 1)
INSERT [TenantManager].[Tenant] VALUES ( N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', N'Tenant Group 05102020', N'', N'Group', N'', N'', N'pramod', N'shinde', N'+91-9876567834', N'pramod.shinde45123@gmail.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-10-05' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, NULL, 1)
INSERT [TenantManager].[Tenant] VALUES ( N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Tenant UK', N'', N'Tenant', N'', N'domain.com', N'tenant', N'UK', N'+44-7867868767', N'tenantuk@demo.com', N'', N'', N'', N'test tenant', N'', N'London', N'London', N'18', N'545342', CAST(N'2020-10-06' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 1)
INSERT [TenantManager].[Tenant] VALUES ( N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', N'SS_Group', N'Group Created for Testing', N'Group', N'', N'', N'SSGroup', N'manager', N'+91-1254632589', N'ss_group@mailinator.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'', 1)
INSERT [TenantManager].[Tenant] VALUES ( N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'SS_Websym1', N'', N'Tenant', N'', N'websym1.com', N'ss', N'websym', N'+91-2342342321', N'sswebsym@mailinator.com', N'', N'', N'', N'123', N'', N'PN', N'MH', N'36', N'57', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NISTest;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 1)

USE [master]
GO
ALTER DATABASE [NIS] SET  READ_WRITE 
GO



