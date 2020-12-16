CREATE VIEW [NIS].[View_StatementDefinition]
AS
SELECT s.*, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS OwnerName 
FROM NIS.Statement s 
LEFT JOIN NIS.[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN NIS.[User] usr2 ON s.Owner = usr2.Id