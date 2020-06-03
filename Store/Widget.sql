
CREATE TABLE [NIS].[Widget] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProductTypeId]     NVARCHAR (50)  NOT NULL,
    [WidgetTypeId]    NVARCHAR (50)  NOT NULL,
    [WidgetName]    NVARCHAR (100)  NOT NULL,
    [WidgetSetting] NVARCHAR (MAX) NOT NULL,
    [TenantCode]    NVARCHAR (50)  NOT NULL,
    [IsDeleted]     BIT            NOT NULL,
	[IsActive]      BIT            NOT NULL
	[LastUpdatedDate] DateTime  NULL,
	[UpdateBy] BIGINT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

