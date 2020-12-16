CREATE TABLE [NIS].[TenantContact] (
    [Id]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [FirstName]            NVARCHAR (100) NOT NULL,
    [LastName]             NVARCHAR (100) NOT NULL,
    [ContactNumber]        NVARCHAR (20)  NOT NULL,
    [ContactType]          NVARCHAR (50)  NOT NULL,
    [EmailAddress]         NVARCHAR (50)  NOT NULL,
    [Image]                NVARCHAR (MAX) NULL,
    [IsActive]             BIT            NOT NULL,
    [IsDeleted]            BIT            NOT NULL,
    [IsActivationLinkSent] BIT            NOT NULL,
    [TenantCode]           NVARCHAR (50)  NOT NULL,
    [CountryId]            BIGINT         NOT NULL,
    CONSTRAINT [PK__TenantCo__3214EC077E43916F] PRIMARY KEY CLUSTERED ([Id] ASC)
);

