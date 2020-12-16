CREATE VIEW [NIS].[View_SourceData]
AS
SELECT ad.*, p.DisplayName AS PageName, w.DisplayName AS WidgetName, cm.FirstName+' '+cm.MiddleName+' '+cm.LastName AS CustomerName, pt.Name AS PageTypeName 
FROM NIS.AnalyticsData ad 
INNER JOIN NIS.CustomerMaster cm ON ad.CustomerId = cm.Id
LEFT JOIN NIS.Page p ON ad.PageId = p.Id
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.Widget w ON w.Id = ad.WidgetId