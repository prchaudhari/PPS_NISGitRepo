--For User Grid
IF OBJECT_ID (N'NIS.View_User', N'V') IS NOT NULL  
    DROP view NIS.View_User;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_User
AS
SELECT usr.*, r.Name 
FROM NIS.[User] usr 
INNER JOIN NIS.UserRoleMap urm ON usr.Id = urm.UserId 
INNER JOIN NIS.Role r ON urm.RoleId = r.Id
Go

--For Page Grid
IF OBJECT_ID (N'NIS.View_Page', N'V') IS NOT NULL  
    DROP view NIS.View_Page;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_Page
AS
SELECT p.*, pt.Name, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS PageOwnerName 
FROM NIS.Page p 
INNER JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
INNER JOIN NIS.[User] usr1 ON p.PublishedBy = usr1.Id
INNER JOIN NIS.[User] usr2 ON p.Owner = usr2.Id
Go

--For Statement Definition grid
IF OBJECT_ID (N'NIS.View_StatementDefinition', N'V') IS NOT NULL  
    DROP view NIS.View_StatementDefinition;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_StatementDefinition
AS
SELECT s.*, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS OwnerName 
FROM NIS.Statement s 
INNER JOIN NIS.[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN NIS.[User] usr2 ON s.Owner = usr2.Id
Go

--For Schedule Management Grid
IF OBJECT_ID (N'NIS.View_Schedule', N'V') IS NOT NULL  
    DROP view NIS.View_Schedule;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_Schedule
AS
SELECT sc.*, s.Name As StatementName
FROM NIS.Schedule sc 
INNER JOIN NIS.Statement s ON sc.StatementId = s.Id
Go

--For Source Data grid
IF OBJECT_ID (N'NIS.View_SourceData', N'V') IS NOT NULL  
    DROP view NIS.View_SourceData;  
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW NIS.View_SourceData
AS
SELECT sc.*, s.Name As StatementName
FROM NIS.Schedule sc 
INNER JOIN NIS.Statement s ON sc.StatementId = s.Id
Go
SELECT ad.*, p.DisplayName AS PageName, w.DisplayName AS WidgetName, cm.FirstName+' '+cm.MiddleName+' '+cm.LastName AS CustomerName  
FROM NIS.AnalyticsData ad 
INNER JOIN NIS.CustomerMaster cm ON ad.CustomerId = cm.Id
LEFT JOIN NIS.Page p ON ad.PageId = p.Id
LEFT JOIN NIS.Widget w ON w.Id = ad.WidgetId
Go