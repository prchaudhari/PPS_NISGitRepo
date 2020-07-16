CREATE TABLE [NIS].[ScheduleRunHistory]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [ScheduleId] BIGINT NOT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NOT NULL, 
    [TenantCode] NVARCHAR(50) NULL
)
