IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AircraftTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_AircraftTypes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Aircraft] (
    [Id] int NOT NULL IDENTITY,
    [RegistrationNumber] nvarchar(20) NOT NULL,
    [Model] nvarchar(50) NOT NULL,
    [AircraftTypeId] int NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [LastMaintenanceDate] datetime2 NULL,
    CONSTRAINT [PK_Aircraft] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Aircraft_AircraftTypes_AircraftTypeId] FOREIGN KEY ([AircraftTypeId]) REFERENCES [AircraftTypes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FlightLogFields] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [DisplayText] nvarchar(50) NOT NULL,
    [Description] nvarchar(200) NOT NULL,
    [IsRequired] bit NOT NULL,
    [AircraftTypeId] int NOT NULL,
    CONSTRAINT [PK_FlightLogFields] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FlightLogFields_AircraftTypes_AircraftTypeId] FOREIGN KEY ([AircraftTypeId]) REFERENCES [AircraftTypes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FlightLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [AircraftId] int NOT NULL,
    [FlightDate] datetime2 NOT NULL,
    [DepartureTime] datetime2 NOT NULL,
    [ArrivalTime] datetime2 NULL,
    [DepartureLocation] nvarchar(50) NOT NULL,
    [ArrivalLocation] nvarchar(50) NOT NULL,
    [Status] int NOT NULL,
    [NumberOfLandings] int NULL,
    [TotalTime] time NULL,
    [Remarks] nvarchar(500) NOT NULL,
    [PassengerCount] int NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastModifiedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FlightLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FlightLogs_Aircraft_AircraftId] FOREIGN KEY ([AircraftId]) REFERENCES [Aircraft] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FlightLogs_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FlightLogValues] (
    [Id] int NOT NULL IDENTITY,
    [Value] nvarchar(max) NOT NULL,
    [FlightLogId] int NOT NULL,
    [FlightLogFieldId] int NOT NULL,
    CONSTRAINT [PK_FlightLogValues] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FlightLogValues_FlightLogFields_FlightLogFieldId] FOREIGN KEY ([FlightLogFieldId]) REFERENCES [FlightLogFields] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FlightLogValues_FlightLogs_FlightLogId] FOREIGN KEY ([FlightLogId]) REFERENCES [FlightLogs] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FlightReviews] (
    [Id] int NOT NULL IDENTITY,
    [FlightLogId] int NOT NULL,
    [ReviewerId] nvarchar(450) NOT NULL,
    [Status] int NOT NULL,
    [Comments] nvarchar(500) NOT NULL,
    [ReviewedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FlightReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FlightReviews_AspNetUsers_ReviewerId] FOREIGN KEY ([ReviewerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FlightReviews_FlightLogs_FlightLogId] FOREIGN KEY ([FlightLogId]) REFERENCES [FlightLogs] ([Id]) ON DELETE NO ACTION
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'1', N'545d7f95-54ef-49c3-be0b-22abd3bc4d46', N'Admin', N'ADMIN');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'FirstName', N'LastName', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] ON;
INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [FirstName], [LastName], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
VALUES (N'1', 0, N'fe624c4e-3913-447b-912c-a1579529a660', N'admin@kasc.com', CAST(1 AS bit), N'System', N'Administrator', CAST(0 AS bit), NULL, N'ADMIN@KASC.COM', N'ADMIN@KASC.COM', N'AQAAAAIAAYagAAAAEMG+xFI6gsrTbomS9TI4r7cIOATSJ1pSrAFb1p+rUDEkFX1Ehq/XufvLM2SwP04tjA==', NULL, CAST(0 AS bit), N'6e2c463e-2f5c-49da-ab5e-e8138f74f437', CAST(0 AS bit), N'admin@kasc.com');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'FirstName', N'LastName', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
    SET IDENTITY_INSERT [AspNetUserRoles] ON;
INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
VALUES (N'1', N'1');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
    SET IDENTITY_INSERT [AspNetUserRoles] OFF;
GO

CREATE INDEX [IX_Aircraft_AircraftTypeId] ON [Aircraft] ([AircraftTypeId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_FlightLogFields_AircraftTypeId] ON [FlightLogFields] ([AircraftTypeId]);
GO

CREATE INDEX [IX_FlightLogs_AircraftId] ON [FlightLogs] ([AircraftId]);
GO

CREATE INDEX [IX_FlightLogs_UserId] ON [FlightLogs] ([UserId]);
GO

CREATE INDEX [IX_FlightLogValues_FlightLogFieldId] ON [FlightLogValues] ([FlightLogFieldId]);
GO

CREATE INDEX [IX_FlightLogValues_FlightLogId] ON [FlightLogValues] ([FlightLogId]);
GO

CREATE INDEX [IX_FlightReviews_FlightLogId] ON [FlightReviews] ([FlightLogId]);
GO

CREATE INDEX [IX_FlightReviews_ReviewerId] ON [FlightReviews] ([ReviewerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241119082440_InitialCreate', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [Role] int NOT NULL DEFAULT 0;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'49000331-a03e-4efb-b676-8a48574a4338'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'534715ce-97e1-450d-870d-3dcde510ce13', [PasswordHash] = N'AQAAAAIAAYagAAAAEIN7dHNRHERHL/CUfW4slIxuj/qF0dPq+JiNocjeBXzqnDcQrY9EmBwTXz8iW50BKA==', [Role] = 0, [SecurityStamp] = N'aa6f6709-ba44-4e78-9e0c-bfbc2688bfc7'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241119150832_RolesUpdate', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[FlightLogFields].[IsRequired]', N'Required', N'COLUMN';
GO

ALTER TABLE [FlightLogFields] ADD [Order] int NOT NULL DEFAULT 0;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Aircraft]') AND [c].[name] = N'Description');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Aircraft] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Aircraft] ALTER COLUMN [Description] nvarchar(200) NULL;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'32549c4f-9c63-47f6-8b92-5bb11d41e6a8'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'f79061e8-c413-4c6b-88ab-6e33001438b4', [PasswordHash] = N'AQAAAAIAAYagAAAAEDyXT4sf4oIG6sa252exdHLbb5Q/JDVrYuthP/74C9ib5GeSXwJUrC2pgEMM3Rj5PA==', [SecurityStamp] = N'5ab90a70-a883-4373-9f21-5d92856f5603'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241120135543_FlightLogFieldUpdate', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'72c5a5c3-e61c-4266-b173-0eeb33f60f7c'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'126d0e76-f5a5-4674-91b1-ee11e22dfcad', [PasswordHash] = N'AQAAAAIAAYagAAAAEGe/dMQUz+Lev7lPCzlAsysKFgEfFYo2GurduoOA4WJikoGn+Nzj8QT67ZDoTwKriQ==', [SecurityStamp] = N'1fa142cd-a192-45f8-a1d4-64c010f76c9c'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241121082121_UpdateAircraft', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Aircraft]') AND [c].[name] = N'Model');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Aircraft] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Aircraft] ALTER COLUMN [Model] nvarchar(50) NULL;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'7126457a-cab4-40b9-8722-0e06dbd91e4b'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'f9df5156-f8fa-4b52-ae5f-c33160d9d2fb', [PasswordHash] = N'AQAAAAIAAYagAAAAEAYPVlMEOhrbdkBxBStSMVbF+CyrjqFiNJi6pzlCcWw+AV3q0RvB4JFw31mDh8MXfw==', [SecurityStamp] = N'994be0b9-dfe2-48bc-9c37-9de0c85d165b'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123103415_UpdateFlightLogField', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'204eff63-f0d0-45b8-b0e3-93ab7fcd58f2'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'b33cd85a-7e6a-4e47-8242-28650153c4e1', [PasswordHash] = N'AQAAAAIAAYagAAAAEFZ+PCUjrR125FAOUpvQypIZ5CiVk8YqfcU2vslutTzMq57xOek0YcGG957j/DGSZg==', [SecurityStamp] = N'676a4ead-fc87-4c47-b5ed-e6b99c5f93d4'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123133450_UpdateFlightLogAndFieldModels', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'dbe8a04f-0724-4780-805c-8d332048381b'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'8be4464a-8ca0-48e1-bf47-e8658d292cc8', [PasswordHash] = N'AQAAAAIAAYagAAAAEAlPuU5iv6aKWmWRdi8/xIejFBnCZis/WDDagJwgxjWjv8mctRCH/c0qjQnXuteTyw==', [SecurityStamp] = N'630d3e4f-5afb-41fe-8501-4e1eb03106c2'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123151639_FlightLogFieldUpdateFlightLogUpbdate', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [FlightLogFields] ADD [FieldType] nvarchar(20) NOT NULL DEFAULT N'';
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'67430181-8198-465f-8f30-c5f4916a19b6'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'ab727e26-603f-4192-b55a-968b7502cc33', [PasswordHash] = N'AQAAAAIAAYagAAAAEN8uvPJzL7UjLu7AolkiGET0C5l7z4yhkcL5YJ3aPeRg1tXgn4aHRQ739/kMywsgcA==', [SecurityStamp] = N'70a21632-7180-456d-a588-c3b1b78c0fa7'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123153231_FlightLogFieldAddFieldType', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'7a0077dd-4c6b-4d79-9ebf-03ded37149bb'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'b4799c51-17c1-4ef5-8582-9c0ab8db3330', [PasswordHash] = N'AQAAAAIAAYagAAAAELwUBl261b1Gvx0hWRL2DjHHwYywcMTkVMnoENcgVAquesCId4os4rcFVY4T7zWgfw==', [SecurityStamp] = N'96605d4a-60d1-4784-b673-5ccce019b783'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123165547_UpdateFlightLogNullableFields', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'719c1764-d691-45e1-936f-63ae895ea472'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'86b1fb65-c294-4df4-ab8c-74093e9fc898', [PasswordHash] = N'AQAAAAIAAYagAAAAEOmZ9LChnZ+B0uW/EawmWSGRqJa2IkOyOepwo9NnVr1xFV0P7RMiPmrVkfLFDDcn5A==', [SecurityStamp] = N'8b198d6b-3067-4d86-abb3-fbf9c15124dd'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123170443_UpdateDbcontextFlightLogNullableField', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'e3b40e90-e81b-4431-bd06-21de42dcadde'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'29cea22d-8390-4634-bc46-8cba38972411', [PasswordHash] = N'AQAAAAIAAYagAAAAEARnJ/OmBu+kcIcr20XYGGDiX7J4e3EV0fzteWGGs45OnBAG/ixumfUCgpMoR99aAg==', [SecurityStamp] = N'bd88bdac-b61f-4727-bba6-581930130a2d'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241123173538_UpdateRemarksNulls', N'8.0.11');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FlightLogs]') AND [c].[name] = N'Remarks');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [FlightLogs] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [FlightLogs] ALTER COLUMN [Remarks] nvarchar(500) NULL;
GO

UPDATE [AspNetRoles] SET [ConcurrencyStamp] = N'82f79931-1529-40e5-9a39-c5bc09c9df23'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [AspNetUsers] SET [ConcurrencyStamp] = N'eb425416-b1f9-40c8-8600-3e31b0264f39', [PasswordHash] = N'AQAAAAIAAYagAAAAEE+VNyNh9IpW6GsiAT7gimAMPRL05F2gkxOQnmH9Q+EHgkEKFRHtVehBHfG5GBoZLw==', [SecurityStamp] = N'eca59a9b-30af-4e39-a135-a517f2ee415c'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241124044028_MakeRemarksNullableInDb', N'8.0.11');
GO

COMMIT;
GO

