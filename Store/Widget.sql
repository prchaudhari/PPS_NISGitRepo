
CREATE TABLE [NIS].[Widget] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [PageTypeId]      NVARCHAR (50)  NOT NULL,
    [Description]     NVARCHAR (MAX) NOT NULL,
    [WidgetName]      NVARCHAR (100) NOT NULL,
    [DisplayName]     NVARCHAR (50)  NOT NULL,
    [IsConfigurable]  BIT            NOT NULL,
    [TenantCode]      NVARCHAR (50)  NOT NULL,
    [IsDeleted]       BIT            NOT NULL,
    [IsActive]        BIT            NOT NULL,
    [LastUpdatedDate] DATETIME       NULL,
    [UpdateBy]        BIGINT         NOT NULL,
    [Instantiable]    BIT            NOT NULL,
    CONSTRAINT [PK__Widget__3214EC0775085A22] PRIMARY KEY CLUSTERED ([Id] ASC)
);



