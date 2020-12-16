CREATE TABLE [NIS].[User] (
    [Id]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [FirstName]              NVARCHAR (100) NOT NULL,
    [LastName]               NVARCHAR (100) NOT NULL,
    [ContactNumber]          NVARCHAR (20)  NOT NULL,
    [EmailAddress]           NVARCHAR (50)  NOT NULL,
    [Image]                  NVARCHAR (MAX) NULL,
    [IsLocked]               BIT            NOT NULL,
    [NoofAttempts]           INT            NOT NULL,
    [IsActive]               BIT            NOT NULL,
    [IsDeleted]              BIT            NOT NULL,
    [TenantCode]             NVARCHAR (50)  NOT NULL,
    [CountryId]              BIGINT         NULL,
    [IsInstanceManager]      BIT            NOT NULL,
    [IsGroupManager]         BIT            NOT NULL,
    [IsPasswordResetByAdmin] BIT            NULL,
    CONSTRAINT [PK__User__3214EC07CA7430B6] PRIMARY KEY CLUSTERED ([Id] ASC)
);

