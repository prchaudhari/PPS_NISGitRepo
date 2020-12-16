CREATE VIEW [NIS].[View_PageWidgetMap]
AS

SELECT pw.*, CASE WHEN pw.IsDynamicWidget = 0 THEN w.WidgetName ELSE dw.WidgetName END AS WidgetName
FROM [NIS].[PageWidgetMap] pw 
LEFT JOIN [NIS].[Widget] w ON pw.ReferenceWidgetid = w.id AND pw.IsDynamicWidget = 0
LEFT JOIN [NIS].[DynamicWidget] dw ON pw.ReferenceWidgetid = dw.id AND pw.IsDynamicWidget = 1