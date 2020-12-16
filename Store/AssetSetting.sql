CREATE TABLE [NIS].[AssetSetting] (
    [Id]                 BIGINT          IDENTITY (1, 1) NOT NULL,
    [ImageHeight]        DECIMAL (18, 2) NOT NULL,
    [ImageWidth]         DECIMAL (18, 2) NOT NULL,
    [ImageSize]          DECIMAL (18, 2) NOT NULL,
    [ImageFileExtension] NVARCHAR (50)   NOT NULL,
    [VideoSize]          DECIMAL (18, 2) NOT NULL,
    [VideoFileExtension] NVARCHAR (50)   NOT NULL,
    [TenantCode]         NVARCHAR (50)   NOT NULL,
    CONSTRAINT [PK__AssetSet__3214EC07141A2A67] PRIMARY KEY CLUSTERED ([Id] ASC)
);


