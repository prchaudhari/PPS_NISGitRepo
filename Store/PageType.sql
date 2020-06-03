
CREATE TABLE [NIS].[PageType] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (50)  NOT NULL,
    [Description]    NVARCHAR (250)  NOT NULL,
    [TenantCode]    NVARCHAR (50)  NOT NULL,
    [IsDeleted]     BIT            NOT NULL,
	[IsActive]      BIT            NOT NULL
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

