INSERT INTO [NIS].[PageType]([Name],[Description],[TenantCode],[IsDeleted],[IsActive])
VALUES
('Home','',@SuperTenantIdentifier,0,1),
('Saving Account','',@SuperTenantIdentifier,0,1),
('Current Account','',@SuperTenantIdentifier,0,1)
GO

