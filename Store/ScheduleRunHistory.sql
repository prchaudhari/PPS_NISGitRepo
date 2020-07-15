CREATE TABLE [NIS].[ScheduleRunHistory]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ScheduleId] BIGINT NOT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NOT NULL, 
    [TenantCode] NVARCHAR(50) NULL
)
