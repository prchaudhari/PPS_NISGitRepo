CREATE TABLE [NIS].[MultiTenantUserAccessMap] (
    [Id]                      BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserId]                  BIGINT        NOT NULL,
    [AssociatedTenantCode]    NVARCHAR (50) NOT NULL,
    [OtherTenantCode]         NVARCHAR (50) NOT NULL,
    [OtherTenantAccessRoleId] BIGINT        NOT NULL,
    [ParentTenantCode]        NVARCHAR (50) NOT NULL,
    [IsActive]                BIT           NOT NULL,
    [IsDeleted]               BIT           NOT NULL,
    [LastUpdatedBy]           BIGINT        NOT NULL,
    [LastUpdatedDate]         DATETIME      NOT NULL,
    CONSTRAINT [PK__MultiTen__3214EC076B0070F9] PRIMARY KEY CLUSTERED ([Id] ASC)
);


