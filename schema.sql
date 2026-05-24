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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Classrooms] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Seats] int NOT NULL,
        CONSTRAINT [PK_Classrooms] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Courses] (
        [Id] int NOT NULL IDENTITY,
        [Category] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [PrerequisiteId] int NULL,
        [Duration] int NOT NULL,
        [Capacity] int NOT NULL,
        [Fee] float NOT NULL,
        CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Courses_Courses_PrerequisiteId] FOREIGN KEY ([PrerequisiteId]) REFERENCES [Courses] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Equipments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Equipments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Tracks] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Tracks] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Role] int NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [RegistrationDate] datetime2 NOT NULL,
        [CPR] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [ClassroomEquipment] (
        [ClassroomsId] int NOT NULL,
        [EquipmentsId] int NOT NULL,
        CONSTRAINT [PK_ClassroomEquipment] PRIMARY KEY ([ClassroomsId], [EquipmentsId]),
        CONSTRAINT [FK_ClassroomEquipment_Classrooms_ClassroomsId] FOREIGN KEY ([ClassroomsId]) REFERENCES [Classrooms] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ClassroomEquipment_Equipments_EquipmentsId] FOREIGN KEY ([EquipmentsId]) REFERENCES [Equipments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [CourseTrack] (
        [CoursesId] int NOT NULL,
        [TracksId] int NOT NULL,
        CONSTRAINT [PK_CourseTrack] PRIMARY KEY ([CoursesId], [TracksId]),
        CONSTRAINT [FK_CourseTrack_Courses_CoursesId] FOREIGN KEY ([CoursesId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CourseTrack_Tracks_TracksId] FOREIGN KEY ([TracksId]) REFERENCES [Tracks] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Feedbacks] (
        [Id] int NOT NULL IDENTITY,
        [TraineeId] int NOT NULL,
        [InstructorId] int NOT NULL,
        [CourseId] int NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(500) NULL,
        [SubmittedAt] datetime2 NOT NULL,
        [ContentRating] int NOT NULL,
        [InstructorRating] int NOT NULL,
        [OrganizationRating] int NOT NULL,
        [RecommendCourse] bit NOT NULL,
        CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Feedbacks_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Feedbacks_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Feedbacks_Users_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [InstructorAvailabilities] (
        [Id] int NOT NULL IDENTITY,
        [InstructorId] int NOT NULL,
        [DayStart] int NOT NULL,
        [DayEnd] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        CONSTRAINT [PK_InstructorAvailabilities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InstructorAvailabilities_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [InstructorExpertises] (
        [InstructorId] int NOT NULL,
        [CourseId] int NOT NULL,
        CONSTRAINT [PK_InstructorExpertises] PRIMARY KEY ([InstructorId], [CourseId]),
        CONSTRAINT [FK_InstructorExpertises_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InstructorExpertises_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Sessions] (
        [Id] int NOT NULL IDENTITY,
        [CourseId] int NOT NULL,
        [InstructorId] int NOT NULL,
        [ClassroomId] int NOT NULL,
        [SessionDate] datetime2 NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sessions_Classrooms_ClassroomId] FOREIGN KEY ([ClassroomId]) REFERENCES [Classrooms] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Sessions_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Sessions_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [TraineeCertifications] (
        [Id] int NOT NULL IDENTITY,
        [TraineeId] int NOT NULL,
        [TrackId] int NOT NULL,
        [Status] int NOT NULL,
        [CertificateReferenceNumber] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_TraineeCertifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TraineeCertifications_Tracks_TrackId] FOREIGN KEY ([TrackId]) REFERENCES [Tracks] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TraineeCertifications_Users_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Enrollments] (
        [Id] int NOT NULL IDENTITY,
        [TraineeId] int NOT NULL,
        [SessionId] int NOT NULL,
        [Status] int NOT NULL,
        [EnrollmentDate] datetime2 NOT NULL,
        [CompletionDate] datetime2 NULL,
        [PaymentDueDate] datetime2 NULL,
        [UserId] int NULL,
        CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Enrollments_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Sessions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Enrollments_Users_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Enrollments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Assessments] (
        [Id] int NOT NULL IDENTITY,
        [EnrollmentId] int NOT NULL,
        [Status] int NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [CompletedBy] datetime2 NULL,
        CONSTRAINT [PK_Assessments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Assessments_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Balances] (
        [Id] int NOT NULL IDENTITY,
        [EnrollmentId] int NOT NULL,
        [AmountDue] decimal(10,2) NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Balances] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Balances_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [EnrollmentId] int NOT NULL,
        [Amount] decimal(10,2) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Seats') AND [object_id] = OBJECT_ID(N'[Classrooms]'))
        SET IDENTITY_INSERT [Classrooms] ON;
    EXEC(N'INSERT INTO [Classrooms] ([Id], [Name], [Seats])
    VALUES (1, N''Room A'', 30),
    (2, N''Lab 1'', 20)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Seats') AND [object_id] = OBJECT_ID(N'[Classrooms]'))
        SET IDENTITY_INSERT [Classrooms] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
        SET IDENTITY_INSERT [Courses] ON;
    EXEC(N'INSERT INTO [Courses] ([Id], [Capacity], [Category], [Description], [Duration], [Fee], [PrerequisiteId], [Title])
    VALUES (1, 2, 1, N''Introduction to C# programming'', 20, 120.0E0, NULL, N''C# Basics''),
    (3, 2, 2, N''Introduction to SQL Server'', 25, 150.0E0, NULL, N''SQL Fundamentals'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
        SET IDENTITY_INSERT [Courses] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Equipments]'))
        SET IDENTITY_INSERT [Equipments] ON;
    EXEC(N'INSERT INTO [Equipments] ([Id], [Name])
    VALUES (1, N''Projector''),
    (2, N''Lab Computers'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Equipments]'))
        SET IDENTITY_INSERT [Equipments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Tracks]'))
        SET IDENTITY_INSERT [Tracks] ON;
    EXEC(N'INSERT INTO [Tracks] ([Id], [Description], [Name])
    VALUES (1, N''Backend programming track'', N''Backend Development''),
    (2, N''Database management track'', N''Database Administration'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Tracks]'))
        SET IDENTITY_INSERT [Tracks] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CPR', N'Email', N'FirstName', N'LastName', N'Password', N'Phone', N'RegistrationDate', N'Role') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] ON;
    EXEC(N'INSERT INTO [Users] ([Id], [CPR], [Email], [FirstName], [LastName], [Password], [Phone], [RegistrationDate], [Role])
    VALUES (1, N''123456789'', N''ali@mail.com'', N''Ali'', N''Ahmad'', N''$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq'', N''99999991'', ''2026-06-15T00:00:00.0000000'', 0),
    (2, N''987654321'', N''sara@mail.com'', N''Sara'', N''Mohamed'', N''$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq'', N''99999992'', ''2026-06-15T00:00:00.0000000'', 1),
    (3, N''112233445'', N''dana@mail.com'', N''Dana'', N''Albanki'', N''$2a$12$ZPzIhfjkDv3uc/4fEkhAfuAM/hYixvISLMEhyBYk7dxrsGJdw15Rq'', N''99999993'', ''2026-06-15T00:00:00.0000000'', 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CPR', N'Email', N'FirstName', N'LastName', N'Password', N'Phone', N'RegistrationDate', N'Role') AND [object_id] = OBJECT_ID(N'[Users]'))
        SET IDENTITY_INSERT [Users] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClassroomsId', N'EquipmentsId') AND [object_id] = OBJECT_ID(N'[ClassroomEquipment]'))
        SET IDENTITY_INSERT [ClassroomEquipment] ON;
    EXEC(N'INSERT INTO [ClassroomEquipment] ([ClassroomsId], [EquipmentsId])
    VALUES (1, 1),
    (2, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClassroomsId', N'EquipmentsId') AND [object_id] = OBJECT_ID(N'[ClassroomEquipment]'))
        SET IDENTITY_INSERT [ClassroomEquipment] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
        SET IDENTITY_INSERT [CourseTrack] ON;
    EXEC(N'INSERT INTO [CourseTrack] ([CoursesId], [TracksId])
    VALUES (1, 1),
    (3, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
        SET IDENTITY_INSERT [CourseTrack] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
        SET IDENTITY_INSERT [Courses] ON;
    EXEC(N'INSERT INTO [Courses] ([Id], [Capacity], [Category], [Description], [Duration], [Fee], [PrerequisiteId], [Title])
    VALUES (2, 2, 1, N''Advanced concepts in C#'', 30, 180.0E0, 1, N''Advanced C#'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
        SET IDENTITY_INSERT [Courses] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DayEnd', N'DayStart', N'EndTime', N'InstructorId', N'StartTime') AND [object_id] = OBJECT_ID(N'[InstructorAvailabilities]'))
        SET IDENTITY_INSERT [InstructorAvailabilities] ON;
    EXEC(N'INSERT INTO [InstructorAvailabilities] ([Id], [DayEnd], [DayStart], [EndTime], [InstructorId], [StartTime])
    VALUES (1, 4, 0, ''16:00:00'', 2, ''08:00:00'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DayEnd', N'DayStart', N'EndTime', N'InstructorId', N'StartTime') AND [object_id] = OBJECT_ID(N'[InstructorAvailabilities]'))
        SET IDENTITY_INSERT [InstructorAvailabilities] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
        SET IDENTITY_INSERT [InstructorExpertises] ON;
    EXEC(N'INSERT INTO [InstructorExpertises] ([CourseId], [InstructorId])
    VALUES (1, 2),
    (3, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
        SET IDENTITY_INSERT [InstructorExpertises] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Message', N'Status', N'UserId') AND [object_id] = OBJECT_ID(N'[Notifications]'))
        SET IDENTITY_INSERT [Notifications] ON;
    EXEC(N'INSERT INTO [Notifications] ([Id], [CreatedDate], [Message], [Status], [UserId])
    VALUES (1, ''2026-06-15T00:00:00.0000000'', N''Welcome to the platform'', 0, 1),
    (2, ''2026-06-15T00:00:00.0000000'', N''You have a scheduling conflict'', 0, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Message', N'Status', N'UserId') AND [object_id] = OBJECT_ID(N'[Notifications]'))
        SET IDENTITY_INSERT [Notifications] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
        SET IDENTITY_INSERT [Sessions] ON;
    EXEC(N'INSERT INTO [Sessions] ([Id], [ClassroomId], [CourseId], [EndTime], [InstructorId], [SessionDate], [StartTime])
    VALUES (1, 1, 1, ''11:00:00'', 2, ''2026-06-22T00:00:00.0000000'', ''09:00:00''),
    (3, 1, 3, ''14:00:00'', 2, ''2026-06-22T00:00:00.0000000'', ''12:00:00''),
    (4, 2, 1, ''12:00:00'', 2, ''2026-06-22T00:00:00.0000000'', ''10:00:00'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
        SET IDENTITY_INSERT [Sessions] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CertificateReferenceNumber', N'Status', N'TrackId', N'TraineeId') AND [object_id] = OBJECT_ID(N'[TraineeCertifications]'))
        SET IDENTITY_INSERT [TraineeCertifications] ON;
    EXEC(N'INSERT INTO [TraineeCertifications] ([Id], [CertificateReferenceNumber], [Status], [TrackId], [TraineeId])
    VALUES (1, N''CERT-1001'', 1, 1, 1),
    (2, N''CERT-1002'', 0, 2, 1)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CertificateReferenceNumber', N'Status', N'TrackId', N'TraineeId') AND [object_id] = OBJECT_ID(N'[TraineeCertifications]'))
        SET IDENTITY_INSERT [TraineeCertifications] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
        SET IDENTITY_INSERT [CourseTrack] ON;
    EXEC(N'INSERT INTO [CourseTrack] ([CoursesId], [TracksId])
    VALUES (2, 1)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
        SET IDENTITY_INSERT [CourseTrack] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
        SET IDENTITY_INSERT [Enrollments] ON;
    EXEC(N'INSERT INTO [Enrollments] ([Id], [CompletionDate], [EnrollmentDate], [PaymentDueDate], [SessionId], [Status], [TraineeId], [UserId])
    VALUES (1, NULL, ''2026-06-15T00:00:00.0000000'', NULL, 1, 1, 1, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
        SET IDENTITY_INSERT [Enrollments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
        SET IDENTITY_INSERT [InstructorExpertises] ON;
    EXEC(N'INSERT INTO [InstructorExpertises] ([CourseId], [InstructorId])
    VALUES (2, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
        SET IDENTITY_INSERT [InstructorExpertises] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
        SET IDENTITY_INSERT [Sessions] ON;
    EXEC(N'INSERT INTO [Sessions] ([Id], [ClassroomId], [CourseId], [EndTime], [InstructorId], [SessionDate], [StartTime])
    VALUES (2, 2, 2, ''11:00:00'', 2, ''2026-06-22T00:00:00.0000000'', ''09:00:00'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
        SET IDENTITY_INSERT [Sessions] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
        SET IDENTITY_INSERT [Assessments] ON;
    EXEC(N'INSERT INTO [Assessments] ([Id], [CompletedBy], [DueDate], [EnrollmentId], [Status])
    VALUES (1, ''2026-06-21T00:00:00.0000000'', ''2026-06-20T00:00:00.0000000'', 1, 1)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
        SET IDENTITY_INSERT [Assessments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
        SET IDENTITY_INSERT [Balances] ON;
    EXEC(N'INSERT INTO [Balances] ([Id], [AmountDue], [DueDate], [EnrollmentId], [Status])
    VALUES (1, 0.0, ''2026-06-15T00:00:00.0000000'', 1, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
        SET IDENTITY_INSERT [Balances] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
        SET IDENTITY_INSERT [Enrollments] ON;
    EXEC(N'INSERT INTO [Enrollments] ([Id], [CompletionDate], [EnrollmentDate], [PaymentDueDate], [SessionId], [Status], [TraineeId], [UserId])
    VALUES (2, NULL, ''2026-06-15T00:00:00.0000000'', NULL, 2, 4, 1, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
        SET IDENTITY_INSERT [Enrollments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
        SET IDENTITY_INSERT [Payments] ON;
    EXEC(N'INSERT INTO [Payments] ([Id], [Amount], [EnrollmentId], [PaymentDate], [Status])
    VALUES (1, 120.0, 1, ''2026-06-15T00:00:00.0000000'', 1)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
        SET IDENTITY_INSERT [Payments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
        SET IDENTITY_INSERT [Assessments] ON;
    EXEC(N'INSERT INTO [Assessments] ([Id], [CompletedBy], [DueDate], [EnrollmentId], [Status])
    VALUES (2, ''2026-06-21T00:00:00.0000000'', ''2026-06-20T00:00:00.0000000'', 2, 2)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
        SET IDENTITY_INSERT [Assessments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
        SET IDENTITY_INSERT [Balances] ON;
    EXEC(N'INSERT INTO [Balances] ([Id], [AmountDue], [DueDate], [EnrollmentId], [Status])
    VALUES (2, 120.0, ''2026-06-22T00:00:00.0000000'', 2, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
        SET IDENTITY_INSERT [Balances] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
        SET IDENTITY_INSERT [Payments] ON;
    EXEC(N'INSERT INTO [Payments] ([Id], [Amount], [EnrollmentId], [PaymentDate], [Status])
    VALUES (2, 60.0, 2, ''2026-06-15T00:00:00.0000000'', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
        SET IDENTITY_INSERT [Payments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Assessments_EnrollmentId] ON [Assessments] ([EnrollmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Balances_EnrollmentId] ON [Balances] ([EnrollmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ClassroomEquipment_EquipmentsId] ON [ClassroomEquipment] ([EquipmentsId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Courses_PrerequisiteId] ON [Courses] ([PrerequisiteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CourseTrack_TracksId] ON [CourseTrack] ([TracksId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Enrollments_SessionId] ON [Enrollments] ([SessionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Enrollments_TraineeId] ON [Enrollments] ([TraineeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Enrollments_UserId] ON [Enrollments] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Feedbacks_CourseId] ON [Feedbacks] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Feedbacks_InstructorId] ON [Feedbacks] ([InstructorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Feedbacks_TraineeId] ON [Feedbacks] ([TraineeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InstructorAvailabilities_InstructorId] ON [InstructorAvailabilities] ([InstructorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InstructorExpertises_CourseId] ON [InstructorExpertises] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_EnrollmentId] ON [Payments] ([EnrollmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sessions_ClassroomId_SessionDate] ON [Sessions] ([ClassroomId], [SessionDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sessions_CourseId] ON [Sessions] ([CourseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sessions_InstructorId_SessionDate] ON [Sessions] ([InstructorId], [SessionDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TraineeCertifications_TrackId] ON [TraineeCertifications] ([TrackId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TraineeCertifications_TraineeId] ON [TraineeCertifications] ([TraineeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523214143_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260523214143_InitialCreate', N'9.0.14');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'DELETE FROM [Assessments]
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'DELETE FROM [TraineeCertifications]
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Assessments] SET [CompletedBy] = ''2026-06-22T00:00:00.0000000''
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Balances] SET [AmountDue] = 0.0, [DueDate] = ''2026-06-23T00:00:00.0000000''
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Enrollments] SET [CompletionDate] = ''2026-06-22T00:00:00.0000000'', [Status] = 3
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Enrollments] SET [EnrollmentDate] = ''2026-06-23T00:00:00.0000000'', [SessionId] = 3, [Status] = 1
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
        SET IDENTITY_INSERT [Enrollments] ON;
    EXEC(N'INSERT INTO [Enrollments] ([Id], [CompletionDate], [EnrollmentDate], [PaymentDueDate], [SessionId], [Status], [TraineeId], [UserId])
    VALUES (3, NULL, ''2026-06-15T00:00:00.0000000'', NULL, 2, 4, 1, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
        SET IDENTITY_INSERT [Enrollments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Comment', N'ContentRating', N'CourseId', N'InstructorId', N'InstructorRating', N'OrganizationRating', N'Rating', N'RecommendCourse', N'SubmittedAt', N'TraineeId') AND [object_id] = OBJECT_ID(N'[Feedbacks]'))
        SET IDENTITY_INSERT [Feedbacks] ON;
    EXEC(N'INSERT INTO [Feedbacks] ([Id], [Comment], [ContentRating], [CourseId], [InstructorId], [InstructorRating], [OrganizationRating], [Rating], [RecommendCourse], [SubmittedAt], [TraineeId])
    VALUES (1, N''Excellent instructor and very clear explanations'', 5, 1, 2, 5, 4, 5, CAST(1 AS bit), ''2026-06-22T00:00:00.0000000'', 1)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Comment', N'ContentRating', N'CourseId', N'InstructorId', N'InstructorRating', N'OrganizationRating', N'Rating', N'RecommendCourse', N'SubmittedAt', N'TraineeId') AND [object_id] = OBJECT_ID(N'[Feedbacks]'))
        SET IDENTITY_INSERT [Feedbacks] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Payments] SET [Amount] = 180.0, [PaymentDate] = ''2026-06-23T00:00:00.0000000'', [Status] = 1
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Sessions] SET [CourseId] = 3
    WHERE [Id] = 2;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Sessions] SET [CourseId] = 2, [SessionDate] = ''2026-06-25T00:00:00.0000000''
    WHERE [Id] = 3;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    EXEC(N'UPDATE [Sessions] SET [SessionDate] = ''2026-06-25T00:00:00.0000000''
    WHERE [Id] = 4;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
        SET IDENTITY_INSERT [Balances] ON;
    EXEC(N'INSERT INTO [Balances] ([Id], [AmountDue], [DueDate], [EnrollmentId], [Status])
    VALUES (3, 90.0, ''2026-06-22T00:00:00.0000000'', 3, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
        SET IDENTITY_INSERT [Balances] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
        SET IDENTITY_INSERT [Payments] ON;
    EXEC(N'INSERT INTO [Payments] ([Id], [Amount], [EnrollmentId], [PaymentDate], [Status])
    VALUES (3, 60.0, 3, ''2026-06-15T00:00:00.0000000'', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
        SET IDENTITY_INSERT [Payments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523220741_FixedData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260523220741_FixedData', N'9.0.14');
END;

COMMIT;
GO

