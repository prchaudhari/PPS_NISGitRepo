CREATE TABLE [NIS].[PageWidgetMap] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ReferenceWidgetId] BIGINT         NOT NULL,
    [Height]            INT            NOT NULL,
    [Width]             INT            NOT NULL,
    [Xposition]         INT            NOT NULL,
    [Yposition]         INT            NOT NULL,
    [PageId]            BIGINT         NOT NULL,
    [WidgetSetting]     NVARCHAR (MAX) NOT NULL,
    [TenantCode]        NVARCHAR (50)  NOT NULL,
    [IsDynamicWidget]   BIT            NOT NULL,
    CONSTRAINT [PK__PageWidg__3214EC0793EB5877] PRIMARY KEY CLUSTERED ([Id] ASC)
);


