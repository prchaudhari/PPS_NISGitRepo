CREATE TABLE [NIS].[Country] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Code]        NVARCHAR (50)  NULL,
    [DialingCode] NVARCHAR (50)  NULL,
    [IsActive]    BIT            NOT NULL,
    [IsDeleted]   BIT            NOT NULL,
    [TenantCode]  NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__Country__3214EC07D53F21D4] PRIMARY KEY CLUSTERED ([Id] ASC)
);


