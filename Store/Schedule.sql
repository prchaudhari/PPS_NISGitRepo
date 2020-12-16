CREATE TABLE [NIS].[Schedule] (
    [Id]                         BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]                       NVARCHAR (100) NOT NULL,
    [StatementId]                BIGINT         NOT NULL,
    [Description]                NVARCHAR (250) NOT NULL,
    [DayOfMonth]                 BIGINT         NOT NULL,
    [HourOfDay]                  BIGINT         NOT NULL,
    [MinuteOfDay]                BIGINT         NOT NULL,
    [StartDate]                  DATETIME       NULL,
    [EndDate]                    DATETIME       NULL,
    [Status]                     NVARCHAR (50)  NOT NULL,
    [IsActive]                   BIT            NOT NULL,
    [IsDeleted]                  BIT            NOT NULL,
    [TenantCode]                 NVARCHAR (50)  NOT NULL,
    [LastUpdatedDate]            DATETIME       NULL,
    [UpdateBy]                   BIGINT         NOT NULL,
    [IsExportToPDF]              BIT            NOT NULL,
    [RecurrancePattern]          NVARCHAR (50)  NULL,
    [RepeatEveryDayMonWeekYear]  BIGINT         NULL,
    [WeekDays]                   NVARCHAR (200) NULL,
    [IsEveryWeekDay]             BIT            NULL,
    [MonthOfYear]                NVARCHAR (50)  NULL,
    [IsEndsAfterNoOfOccurrences] BIT            NULL,
    [NoOfOccurrences]            BIGINT         NULL,
    CONSTRAINT [PK__Schedule__3214EC07DE2298BC] PRIMARY KEY CLUSTERED ([Id] ASC)
);


