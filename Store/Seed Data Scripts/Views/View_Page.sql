CREATE VIEW [NIS].[View_Page]
AS
SELECT p.*, pt.Name, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS PageOwnerName, ast.AssetLibraryId AS BackgroundImageAssetLibraryId
FROM NIS.Page p 
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.[User] usr1 ON p.PublishedBy = usr1.Id
LEFT JOIN NIS.[User] usr2 ON p.Owner = usr2.Id
LEFT JOIN NIS.[Asset] ast ON p.BackgroundImageAssetId = ast.Id