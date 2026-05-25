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
CREATE TABLE [Classrooms] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Seats] int NOT NULL,
    CONSTRAINT [PK_Classrooms] PRIMARY KEY ([Id])
);

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

CREATE TABLE [Equipments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Equipments] PRIMARY KEY ([Id])
);

CREATE TABLE [Tracks] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Tracks] PRIMARY KEY ([Id])
);

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

CREATE TABLE [ClassroomEquipment] (
    [ClassroomsId] int NOT NULL,
    [EquipmentsId] int NOT NULL,
    CONSTRAINT [PK_ClassroomEquipment] PRIMARY KEY ([ClassroomsId], [EquipmentsId]),
    CONSTRAINT [FK_ClassroomEquipment_Classrooms_ClassroomsId] FOREIGN KEY ([ClassroomsId]) REFERENCES [Classrooms] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassroomEquipment_Equipments_EquipmentsId] FOREIGN KEY ([EquipmentsId]) REFERENCES [Equipments] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CourseTrack] (
    [CoursesId] int NOT NULL,
    [TracksId] int NOT NULL,
    CONSTRAINT [PK_CourseTrack] PRIMARY KEY ([CoursesId], [TracksId]),
    CONSTRAINT [FK_CourseTrack_Courses_CoursesId] FOREIGN KEY ([CoursesId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseTrack_Tracks_TracksId] FOREIGN KEY ([TracksId]) REFERENCES [Tracks] ([Id]) ON DELETE CASCADE
);

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

CREATE TABLE [InstructorExpertises] (
    [InstructorId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_InstructorExpertises] PRIMARY KEY ([InstructorId], [CourseId]),
    CONSTRAINT [FK_InstructorExpertises_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_InstructorExpertises_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

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

CREATE TABLE [Assessments] (
    [Id] int NOT NULL IDENTITY,
    [EnrollmentId] int NOT NULL,
    [Status] int NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [CompletedBy] datetime2 NULL,
    CONSTRAINT [PK_Assessments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Assessments_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Balances] (
    [Id] int NOT NULL IDENTITY,
    [EnrollmentId] int NOT NULL,
    [AmountDue] decimal(10,2) NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Balances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Balances_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [EnrollmentId] int NOT NULL,
    [Amount] decimal(10,2) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Seats') AND [object_id] = OBJECT_ID(N'[Classrooms]'))
    SET IDENTITY_INSERT [Classrooms] ON;
INSERT INTO [Classrooms] ([Id], [Name], [Seats])
VALUES (1, N'Room A101', 20),
(2, N'Lab B202', 15),
(3, N'Room C303', 25);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'Seats') AND [object_id] = OBJECT_ID(N'[Classrooms]'))
    SET IDENTITY_INSERT [Classrooms] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
    SET IDENTITY_INSERT [Courses] ON;
INSERT INTO [Courses] ([Id], [Capacity], [Category], [Description], [Duration], [Fee], [PrerequisiteId], [Title])
VALUES (1, 15, 1, N'Core concepts of C# programming including variables, control flow, and OOP.', 20, 120.0E0, NULL, N'C# Fundamentals'),
(4, 15, 2, N'Introduction to relational databases, T-SQL queries, and SQL Server.', 25, 150.0E0, NULL, N'SQL Server Fundamentals'),
(6, 20, 5, N'Common vulnerabilities, OWASP Top 10, secure coding practices.', 20, 140.0E0, NULL, N'Web Security Essentials');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
    SET IDENTITY_INSERT [Courses] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Equipments]'))
    SET IDENTITY_INSERT [Equipments] ON;
INSERT INTO [Equipments] ([Id], [Name])
VALUES (1, N'Projector'),
(2, N'Lab Computers'),
(3, N'Whiteboard');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Equipments]'))
    SET IDENTITY_INSERT [Equipments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Tracks]'))
    SET IDENTITY_INSERT [Tracks] ON;
INSERT INTO [Tracks] ([Id], [Description], [Name])
VALUES (1, N'Covers C#, Advanced C#, and ASP.NET Core MVC to build complete web applications.', N'Full-Stack .NET Developer'),
(2, N'Covers SQL fundamentals and advanced SQL for database management and optimization.', N'Database Administrator');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Tracks]'))
    SET IDENTITY_INSERT [Tracks] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CPR', N'Email', N'FirstName', N'LastName', N'Password', N'Phone', N'RegistrationDate', N'Role') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [CPR], [Email], [FirstName], [LastName], [Password], [Phone], [RegistrationDate], [Role])
VALUES (1, N'860412345', N'ahmed@mail.com', N'Ahmed', N'AlMansouri', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39001001', '2026-04-01T00:00:00.0000000', 0),
(2, N'920815678', N'fatima@mail.com', N'Fatima', N'Ali', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39001002', '2026-04-01T00:00:00.0000000', 0),
(3, N'950322901', N'khalid@mail.com', N'Khalid', N'AlDosari', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39001003', '2026-04-06T00:00:00.0000000', 0),
(4, N'780610234', N'sara@mail.com', N'Sara', N'AlZayani', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39002001', '2026-04-01T00:00:00.0000000', 1),
(5, N'820905567', N'hassan@mail.com', N'Hassan', N'Ali', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39002002', '2026-04-01T00:00:00.0000000', 1),
(6, N'750318890', N'noor@mail.com', N'Noor', N'AlHammadi', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39002003', '2026-04-01T00:00:00.0000000', 1),
(7, N'810724112', N'dana@mail.com', N'Dana', N'AlBanki', N'$2a$12$Ys7YXxI9M7EqQY60T8aNFe31SwSs8IGXjfAYsFNp55NcGyzL4cIym', N'39003001', '2026-04-01T00:00:00.0000000', 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CPR', N'Email', N'FirstName', N'LastName', N'Password', N'Phone', N'RegistrationDate', N'Role') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClassroomsId', N'EquipmentsId') AND [object_id] = OBJECT_ID(N'[ClassroomEquipment]'))
    SET IDENTITY_INSERT [ClassroomEquipment] ON;
INSERT INTO [ClassroomEquipment] ([ClassroomsId], [EquipmentsId])
VALUES (1, 1),
(1, 3),
(2, 1),
(2, 2),
(3, 1),
(3, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClassroomsId', N'EquipmentsId') AND [object_id] = OBJECT_ID(N'[ClassroomEquipment]'))
    SET IDENTITY_INSERT [ClassroomEquipment] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
    SET IDENTITY_INSERT [CourseTrack] ON;
INSERT INTO [CourseTrack] ([CoursesId], [TracksId])
VALUES (1, 1),
(4, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
    SET IDENTITY_INSERT [CourseTrack] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
    SET IDENTITY_INSERT [Courses] ON;
INSERT INTO [Courses] ([Id], [Capacity], [Category], [Description], [Duration], [Fee], [PrerequisiteId], [Title])
VALUES (2, 12, 1, N'Deep dive into LINQ, async/await, generics, and design patterns.', 30, 180.0E0, 1, N'Advanced C# & .NET'),
(5, 12, 2, N'Stored procedures, indexing, query optimization, and database design.', 30, 175.0E0, 4, N'Advanced SQL & Performance');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
    SET IDENTITY_INSERT [Courses] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Comment', N'ContentRating', N'CourseId', N'InstructorId', N'InstructorRating', N'OrganizationRating', N'Rating', N'RecommendCourse', N'SubmittedAt', N'TraineeId') AND [object_id] = OBJECT_ID(N'[Feedbacks]'))
    SET IDENTITY_INSERT [Feedbacks] ON;
INSERT INTO [Feedbacks] ([Id], [Comment], [ContentRating], [CourseId], [InstructorId], [InstructorRating], [OrganizationRating], [Rating], [RecommendCourse], [SubmittedAt], [TraineeId])
VALUES (1, N'Sara explains concepts very clearly. Highly recommended.', 5, 1, 4, 5, 4, 5, CAST(1 AS bit), '2026-04-21T00:00:00.0000000', 1),
(2, N'Hassan is a great instructor. The hands-on labs were very helpful.', 4, 4, 5, 5, 5, 5, CAST(1 AS bit), '2026-04-23T00:00:00.0000000', 2),
(3, N'Good introduction. Would have liked more exercises.', 4, 1, 4, 4, 3, 4, CAST(1 AS bit), '2026-04-21T00:00:00.0000000', 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Comment', N'ContentRating', N'CourseId', N'InstructorId', N'InstructorRating', N'OrganizationRating', N'Rating', N'RecommendCourse', N'SubmittedAt', N'TraineeId') AND [object_id] = OBJECT_ID(N'[Feedbacks]'))
    SET IDENTITY_INSERT [Feedbacks] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DayEnd', N'DayStart', N'EndTime', N'InstructorId', N'StartTime') AND [object_id] = OBJECT_ID(N'[InstructorAvailabilities]'))
    SET IDENTITY_INSERT [InstructorAvailabilities] ON;
INSERT INTO [InstructorAvailabilities] ([Id], [DayEnd], [DayStart], [EndTime], [InstructorId], [StartTime])
VALUES (1, 4, 0, '16:00:00', 4, '08:00:00'),
(2, 3, 0, '17:00:00', 5, '09:00:00'),
(3, 4, 1, '18:00:00', 6, '10:00:00');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DayEnd', N'DayStart', N'EndTime', N'InstructorId', N'StartTime') AND [object_id] = OBJECT_ID(N'[InstructorAvailabilities]'))
    SET IDENTITY_INSERT [InstructorAvailabilities] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
    SET IDENTITY_INSERT [InstructorExpertises] ON;
INSERT INTO [InstructorExpertises] ([CourseId], [InstructorId])
VALUES (1, 4),
(4, 5),
(6, 6);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
    SET IDENTITY_INSERT [InstructorExpertises] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Message', N'Status', N'UserId') AND [object_id] = OBJECT_ID(N'[Notifications]'))
    SET IDENTITY_INSERT [Notifications] ON;
INSERT INTO [Notifications] ([Id], [CreatedDate], [Message], [Status], [UserId])
VALUES (1, '2026-04-01T00:00:00.0000000', N'Welcome to the Training Platform, Ahmed!', 0, 1),
(2, '2026-04-01T00:00:00.0000000', N'Welcome to the Training Platform, Fatima!', 0, 2),
(3, '2026-04-06T00:00:00.0000000', N'Welcome to the Training Platform, Khalid!', 0, 3),
(4, '2026-04-15T00:00:00.0000000', N'Your enrollment in C# Fundamentals has been confirmed.', 1, 1),
(5, '2026-05-20T00:00:00.0000000', N'You are now enrolled in ASP.NET Core MVC starting June 15.', 0, 1),
(6, '2026-04-15T00:00:00.0000000', N'Your enrollment in SQL Server Fundamentals has been confirmed.', 1, 2),
(7, '2026-05-22T00:00:00.0000000', N'Reminder: Your balance for Web Security is pending.', 0, 2),
(8, '2026-05-20T00:00:00.0000000', N'Reminder: Your balance for Advanced C# is overdue.', 0, 3),
(9, '2026-04-01T00:00:00.0000000', N'You have 3 sessions scheduled this month.', 0, 4),
(10, '2026-04-01T00:00:00.0000000', N'You have 2 sessions scheduled this month.', 0, 5);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Message', N'Status', N'UserId') AND [object_id] = OBJECT_ID(N'[Notifications]'))
    SET IDENTITY_INSERT [Notifications] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
    SET IDENTITY_INSERT [Sessions] ON;
INSERT INTO [Sessions] ([Id], [ClassroomId], [CourseId], [EndTime], [InstructorId], [SessionDate], [StartTime])
VALUES (1, 1, 1, '12:00:00', 4, '2026-04-20T00:00:00.0000000', '09:00:00'),
(4, 3, 4, '13:00:00', 5, '2026-04-22T00:00:00.0000000', '10:00:00'),
(6, 1, 6, '13:00:00', 6, '2026-07-01T00:00:00.0000000', '10:00:00');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
    SET IDENTITY_INSERT [Sessions] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CertificateReferenceNumber', N'Status', N'TrackId', N'TraineeId') AND [object_id] = OBJECT_ID(N'[TraineeCertifications]'))
    SET IDENTITY_INSERT [TraineeCertifications] ON;
INSERT INTO [TraineeCertifications] ([Id], [CertificateReferenceNumber], [Status], [TrackId], [TraineeId])
VALUES (1, N'CERT-NET-2026-001', 1, 1, 1),
(2, N'CERT-DBA-2026-001', 1, 2, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CertificateReferenceNumber', N'Status', N'TrackId', N'TraineeId') AND [object_id] = OBJECT_ID(N'[TraineeCertifications]'))
    SET IDENTITY_INSERT [TraineeCertifications] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
    SET IDENTITY_INSERT [CourseTrack] ON;
INSERT INTO [CourseTrack] ([CoursesId], [TracksId])
VALUES (2, 1),
(5, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
    SET IDENTITY_INSERT [CourseTrack] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
    SET IDENTITY_INSERT [Courses] ON;
INSERT INTO [Courses] ([Id], [Capacity], [Category], [Description], [Duration], [Fee], [PrerequisiteId], [Title])
VALUES (3, 12, 3, N'Building web applications using ASP.NET Core MVC and Entity Framework.', 35, 200.0E0, 2, N'ASP.NET Core MVC');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'Category', N'Description', N'Duration', N'Fee', N'PrerequisiteId', N'Title') AND [object_id] = OBJECT_ID(N'[Courses]'))
    SET IDENTITY_INSERT [Courses] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
    SET IDENTITY_INSERT [Enrollments] ON;
INSERT INTO [Enrollments] ([Id], [CompletionDate], [EnrollmentDate], [PaymentDueDate], [SessionId], [Status], [TraineeId], [UserId])
VALUES (1, '2026-04-20T00:00:00.0000000', '2026-04-15T00:00:00.0000000', NULL, 1, 3, 1, NULL),
(4, '2026-04-22T00:00:00.0000000', '2026-04-15T00:00:00.0000000', NULL, 4, 3, 2, NULL),
(6, NULL, '2026-05-22T00:00:00.0000000', NULL, 6, 0, 2, NULL),
(7, '2026-04-20T00:00:00.0000000', '2026-04-16T00:00:00.0000000', NULL, 1, 3, 3, NULL),
(9, NULL, '2026-04-16T00:00:00.0000000', NULL, 4, 4, 3, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
    SET IDENTITY_INSERT [Enrollments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
    SET IDENTITY_INSERT [InstructorExpertises] ON;
INSERT INTO [InstructorExpertises] ([CourseId], [InstructorId])
VALUES (2, 4),
(5, 5);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
    SET IDENTITY_INSERT [InstructorExpertises] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
    SET IDENTITY_INSERT [Sessions] ON;
INSERT INTO [Sessions] ([Id], [ClassroomId], [CourseId], [EndTime], [InstructorId], [SessionDate], [StartTime])
VALUES (2, 2, 2, '12:00:00', 4, '2026-05-18T00:00:00.0000000', '09:00:00'),
(5, 3, 5, '13:00:00', 5, '2026-05-20T00:00:00.0000000', '10:00:00');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
    SET IDENTITY_INSERT [Sessions] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
    SET IDENTITY_INSERT [Assessments] ON;
INSERT INTO [Assessments] ([Id], [CompletedBy], [DueDate], [EnrollmentId], [Status])
VALUES (1, '2026-04-20T00:00:00.0000000', '2026-04-20T00:00:00.0000000', 1, 1),
(4, '2026-04-22T00:00:00.0000000', '2026-04-22T00:00:00.0000000', 4, 1),
(6, NULL, '2026-07-10T00:00:00.0000000', 6, 0),
(7, '2026-04-20T00:00:00.0000000', '2026-04-20T00:00:00.0000000', 7, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
    SET IDENTITY_INSERT [Assessments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
    SET IDENTITY_INSERT [Balances] ON;
INSERT INTO [Balances] ([Id], [AmountDue], [DueDate], [EnrollmentId], [Status])
VALUES (1, 0.0, '2026-04-15T00:00:00.0000000', 1, 1),
(4, 0.0, '2026-04-15T00:00:00.0000000', 4, 1),
(6, 70.0, '2026-06-05T00:00:00.0000000', 6, 0),
(7, 0.0, '2026-04-16T00:00:00.0000000', 7, 1),
(9, 90.0, '2026-05-10T00:00:00.0000000', 9, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
    SET IDENTITY_INSERT [Balances] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
    SET IDENTITY_INSERT [CourseTrack] ON;
INSERT INTO [CourseTrack] ([CoursesId], [TracksId])
VALUES (3, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CoursesId', N'TracksId') AND [object_id] = OBJECT_ID(N'[CourseTrack]'))
    SET IDENTITY_INSERT [CourseTrack] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
    SET IDENTITY_INSERT [Enrollments] ON;
INSERT INTO [Enrollments] ([Id], [CompletionDate], [EnrollmentDate], [PaymentDueDate], [SessionId], [Status], [TraineeId], [UserId])
VALUES (2, NULL, '2026-05-10T00:00:00.0000000', NULL, 2, 2, 1, NULL),
(5, NULL, '2026-05-12T00:00:00.0000000', NULL, 5, 2, 2, NULL),
(8, NULL, '2026-05-11T00:00:00.0000000', NULL, 2, 2, 3, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
    SET IDENTITY_INSERT [Enrollments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
    SET IDENTITY_INSERT [InstructorExpertises] ON;
INSERT INTO [InstructorExpertises] ([CourseId], [InstructorId])
VALUES (3, 4);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourseId', N'InstructorId') AND [object_id] = OBJECT_ID(N'[InstructorExpertises]'))
    SET IDENTITY_INSERT [InstructorExpertises] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] ON;
INSERT INTO [Payments] ([Id], [Amount], [EnrollmentId], [PaymentDate], [Status])
VALUES (1, 120.0, 1, '2026-04-15T00:00:00.0000000', 1),
(4, 150.0, 4, '2026-04-15T00:00:00.0000000', 1),
(6, 70.0, 6, '2026-05-22T00:00:00.0000000', 0),
(7, 120.0, 7, '2026-04-16T00:00:00.0000000', 1),
(9, 60.0, 9, '2026-04-16T00:00:00.0000000', 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
    SET IDENTITY_INSERT [Sessions] ON;
INSERT INTO [Sessions] ([Id], [ClassroomId], [CourseId], [EndTime], [InstructorId], [SessionDate], [StartTime])
VALUES (3, 2, 3, '12:00:00', 4, '2026-06-15T00:00:00.0000000', '09:00:00');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClassroomId', N'CourseId', N'EndTime', N'InstructorId', N'SessionDate', N'StartTime') AND [object_id] = OBJECT_ID(N'[Sessions]'))
    SET IDENTITY_INSERT [Sessions] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
    SET IDENTITY_INSERT [Assessments] ON;
INSERT INTO [Assessments] ([Id], [CompletedBy], [DueDate], [EnrollmentId], [Status])
VALUES (2, NULL, '2026-05-30T00:00:00.0000000', 2, 0),
(5, NULL, '2026-05-30T00:00:00.0000000', 5, 0),
(8, NULL, '2026-05-30T00:00:00.0000000', 8, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
    SET IDENTITY_INSERT [Assessments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
    SET IDENTITY_INSERT [Balances] ON;
INSERT INTO [Balances] ([Id], [AmountDue], [DueDate], [EnrollmentId], [Status])
VALUES (2, 0.0, '2026-05-10T00:00:00.0000000', 2, 1),
(5, 0.0, '2026-05-12T00:00:00.0000000', 5, 1),
(8, 90.0, '2026-05-25T00:00:00.0000000', 8, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
    SET IDENTITY_INSERT [Balances] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
    SET IDENTITY_INSERT [Enrollments] ON;
INSERT INTO [Enrollments] ([Id], [CompletionDate], [EnrollmentDate], [PaymentDueDate], [SessionId], [Status], [TraineeId], [UserId])
VALUES (3, NULL, '2026-05-20T00:00:00.0000000', NULL, 3, 0, 1, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletionDate', N'EnrollmentDate', N'PaymentDueDate', N'SessionId', N'Status', N'TraineeId', N'UserId') AND [object_id] = OBJECT_ID(N'[Enrollments]'))
    SET IDENTITY_INSERT [Enrollments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] ON;
INSERT INTO [Payments] ([Id], [Amount], [EnrollmentId], [PaymentDate], [Status])
VALUES (2, 180.0, 2, '2026-05-10T00:00:00.0000000', 1),
(5, 175.0, 5, '2026-05-12T00:00:00.0000000', 1),
(8, 90.0, 8, '2026-05-11T00:00:00.0000000', 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
    SET IDENTITY_INSERT [Assessments] ON;
INSERT INTO [Assessments] ([Id], [CompletedBy], [DueDate], [EnrollmentId], [Status])
VALUES (3, NULL, '2026-06-20T00:00:00.0000000', 3, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CompletedBy', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Assessments]'))
    SET IDENTITY_INSERT [Assessments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
    SET IDENTITY_INSERT [Balances] ON;
INSERT INTO [Balances] ([Id], [AmountDue], [DueDate], [EnrollmentId], [Status])
VALUES (3, 100.0, '2026-06-01T00:00:00.0000000', 3, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AmountDue', N'DueDate', N'EnrollmentId', N'Status') AND [object_id] = OBJECT_ID(N'[Balances]'))
    SET IDENTITY_INSERT [Balances] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] ON;
INSERT INTO [Payments] ([Id], [Amount], [EnrollmentId], [PaymentDate], [Status])
VALUES (3, 100.0, 3, '2026-05-20T00:00:00.0000000', 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'EnrollmentId', N'PaymentDate', N'Status') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] OFF;

CREATE UNIQUE INDEX [IX_Assessments_EnrollmentId] ON [Assessments] ([EnrollmentId]);

CREATE UNIQUE INDEX [IX_Balances_EnrollmentId] ON [Balances] ([EnrollmentId]);

CREATE INDEX [IX_ClassroomEquipment_EquipmentsId] ON [ClassroomEquipment] ([EquipmentsId]);

CREATE INDEX [IX_Courses_PrerequisiteId] ON [Courses] ([PrerequisiteId]);

CREATE INDEX [IX_CourseTrack_TracksId] ON [CourseTrack] ([TracksId]);

CREATE INDEX [IX_Enrollments_SessionId] ON [Enrollments] ([SessionId]);

CREATE INDEX [IX_Enrollments_TraineeId] ON [Enrollments] ([TraineeId]);

CREATE INDEX [IX_Enrollments_UserId] ON [Enrollments] ([UserId]);

CREATE INDEX [IX_Feedbacks_CourseId] ON [Feedbacks] ([CourseId]);

CREATE INDEX [IX_Feedbacks_InstructorId] ON [Feedbacks] ([InstructorId]);

CREATE INDEX [IX_Feedbacks_TraineeId] ON [Feedbacks] ([TraineeId]);

CREATE INDEX [IX_InstructorAvailabilities_InstructorId] ON [InstructorAvailabilities] ([InstructorId]);

CREATE INDEX [IX_InstructorExpertises_CourseId] ON [InstructorExpertises] ([CourseId]);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

CREATE INDEX [IX_Payments_EnrollmentId] ON [Payments] ([EnrollmentId]);

CREATE INDEX [IX_Sessions_ClassroomId_SessionDate] ON [Sessions] ([ClassroomId], [SessionDate]);

CREATE INDEX [IX_Sessions_CourseId] ON [Sessions] ([CourseId]);

CREATE INDEX [IX_Sessions_InstructorId_SessionDate] ON [Sessions] ([InstructorId], [SessionDate]);

CREATE INDEX [IX_TraineeCertifications_TrackId] ON [TraineeCertifications] ([TrackId]);

CREATE INDEX [IX_TraineeCertifications_TraineeId] ON [TraineeCertifications] ([TraineeId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260525192808_IntialCreate', N'9.0.14');

UPDATE [Assessments] SET [CompletedBy] = '2026-04-05T00:00:00.0000000', [DueDate] = '2026-04-05T00:00:00.0000000'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Assessments] SET [CompletedBy] = '2026-04-20T00:00:00.0000000', [DueDate] = '2026-04-20T00:00:00.0000000', [Status] = 1
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [Assessments] SET [CompletedBy] = '2026-05-10T00:00:00.0000000', [DueDate] = '2026-05-10T00:00:00.0000000', [Status] = 1
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Assessments] SET [CompletedBy] = '2026-04-08T00:00:00.0000000', [DueDate] = '2026-04-08T00:00:00.0000000'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Assessments] SET [CompletedBy] = '2026-04-25T00:00:00.0000000', [DueDate] = '2026-04-25T00:00:00.0000000', [Status] = 1
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [Assessments] SET [CompletedBy] = '2026-04-05T00:00:00.0000000', [DueDate] = '2026-04-05T00:00:00.0000000'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-04-15T00:00:00.0000000'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [AmountDue] = 0.0, [DueDate] = '2026-05-01T00:00:00.0000000', [Status] = 1
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-04-20T00:00:00.0000000'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-05-10T00:00:00.0000000'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [Balances] SET [DueDate] = '2026-05-01T00:00:00.0000000'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [CompletionDate] = '2026-04-05T00:00:00.0000000', [EnrollmentDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [CompletionDate] = '2026-04-20T00:00:00.0000000', [EnrollmentDate] = '2026-04-15T00:00:00.0000000', [Status] = 3
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [CompletionDate] = '2026-05-10T00:00:00.0000000', [EnrollmentDate] = '2026-05-01T00:00:00.0000000', [Status] = 3
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [CompletionDate] = '2026-04-08T00:00:00.0000000', [EnrollmentDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [CompletionDate] = '2026-04-25T00:00:00.0000000', [EnrollmentDate] = '2026-04-20T00:00:00.0000000', [Status] = 3
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [CompletionDate] = '2026-04-05T00:00:00.0000000', [EnrollmentDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [Enrollments] SET [EnrollmentDate] = '2026-04-16T00:00:00.0000000'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [Notifications] SET [CreatedDate] = '2026-05-11T00:00:00.0000000', [Message] = N'Congratulations! You have completed the Full-Stack .NET Developer track.'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [PaymentDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [PaymentDate] = '2026-04-15T00:00:00.0000000'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [Amount] = 200.0, [PaymentDate] = '2026-05-01T00:00:00.0000000', [Status] = 1
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [PaymentDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [PaymentDate] = '2026-04-20T00:00:00.0000000'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [PaymentDate] = '2026-04-01T00:00:00.0000000'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [Payments] SET [PaymentDate] = '2026-04-16T00:00:00.0000000'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [Sessions] SET [SessionDate] = '2026-04-05T00:00:00.0000000'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Sessions] SET [SessionDate] = '2026-04-20T00:00:00.0000000'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [Sessions] SET [SessionDate] = '2026-05-10T00:00:00.0000000'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Sessions] SET [SessionDate] = '2026-04-08T00:00:00.0000000'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Sessions] SET [SessionDate] = '2026-04-25T00:00:00.0000000'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260525194316_FixSeedDataConsistency', N'9.0.14');

COMMIT;
GO

