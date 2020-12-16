CREATE TABLE [NIS].[CustomerInfo] (
    [Id]           INT            NOT NULL,
    [FirstName]    NVARCHAR (100) NOT NULL,
    [MiddleName]   NVARCHAR (100) NULL,
    [Lastname]     NVARCHAR (100) NOT NULL,
    [AddressLine1] NVARCHAR (500) NOT NULL,
    [AddressLine2] NVARCHAR (500) NULL,
    [City]         BIGINT         NOT NULL,
    [State]        BIGINT         NOT NULL,
    [Country]      BIGINT         NOT NULL,
    [Zip]          NVARCHAR (500) NULL,
    CONSTRAINT [PK__Customer__3214EC0774BA5E09] PRIMARY KEY CLUSTERED ([Id] ASC)
);


