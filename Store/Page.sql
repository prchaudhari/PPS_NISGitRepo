CREATE TABLE [NIS].[Page] (
    [Id]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [DisplayName]            NVARCHAR (100) NOT NULL,
    [PageTypeId]             BIGINT         NOT NULL,
    [PublishedBy]            BIGINT         NULL,
    [Owner]                  BIGINT         NOT NULL,
    [Version]                NVARCHAR (100) NOT NULL,
    [Status]                 NVARCHAR (50)  NOT NULL,
    [CreatedDate]            DATETIME       NULL,
    [PublishedOn]            DATETIME       NULL,
    [IsActive]               BIT            NOT NULL,
    [IsDeleted]              BIT            NOT NULL,
    [TenantCode]             NVARCHAR (50)  NOT NULL,
    [LastUpdatedDate]        DATETIME       NULL,
    [UpdateBy]               BIGINT         NULL,
    [BackgroundImageAssetId] BIGINT         NULL,
    [BackgroundImageURL]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__Page__3214EC0778D5B88B] PRIMARY KEY CLUSTERED ([Id] ASC)
);


