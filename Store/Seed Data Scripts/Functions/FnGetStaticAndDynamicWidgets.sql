CREATE FUNCTION [NIS].[FnGetStaticAndDynamicWidgets] (@PageTypeId BIGINT, @TenantCode VARCHAR(50))
RETURNS TABLE
AS
RETURN
(

 SELECT dw.ID, dw.WidgetName, wpt.PageTypeId, dw.Title AS DisplayName, 0 AS Instantiable, dw.WidgetType 
 FROM  [NIS].[DynamicWidget] dw 
 INNER JOIN [NIS].[WidgetPageTypeMap] wpt ON wpt.WidgetId = dw.Id AND wpt.IsDynamicWidget = 1
 WHERE wpt.PageTypeId = @PageTypeId AND dw.IsActive = 1 AND dw.IsDeleted = 0 AND Status = 'Published' AND dw.TenantCode = @TenantCode
 UNION
 SELECT w.Id, w.WidgetName, @PageTypeId AS PageTypeId, w.DisplayName, w.Instantiable, 'Static' AS WidgetType
 FROM [NIS].[Widget] w
 WHERE w.PageTypeId LIKE '%'+CAST(@PageTypeId AS VARCHAR)+'%' AND w.IsActive = 1 AND w.IsDeleted = 0 AND w.TenantCode = @TenantCode
)