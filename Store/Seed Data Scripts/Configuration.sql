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
(N'EmailConfiguration', N'PrimaryFromEmail','nvidyo@n4mative.net')

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