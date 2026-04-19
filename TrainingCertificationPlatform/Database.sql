CREATE DATABASE TrainingCertificationDB;
GO

USE TrainingCertificationDB;
GO

-- USER
CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role INT NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    RegistrationDate DATETIME NOT NULL
);

-- COURSE
CREATE TABLE Course (
    Id INT PRIMARY KEY IDENTITY,
    Category INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    PrerequisiteId INT NULL,
    Duration INT NOT NULL,
    Capacity INT NOT NULL,
    Fee FLOAT NOT NULL,
    FOREIGN KEY (PrerequisiteId) REFERENCES Course(Id)
);

-- TRACK
CREATE TABLE Track (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);

-- CLASSROOM
CREATE TABLE Classroom (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Seats INT NOT NULL
);

-- EQUIPMENT
CREATE TABLE Equipment (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

-- SESSION
CREATE TABLE Session (
    Id INT PRIMARY KEY IDENTITY,
    CourseId INT NOT NULL,
    InstructorId INT NOT NULL,
    ClassroomId INT NOT NULL,
    SessionDate DATETIME NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    FOREIGN KEY (CourseId) REFERENCES Course(Id),
    FOREIGN KEY (InstructorId) REFERENCES [User](Id),
    FOREIGN KEY (ClassroomId) REFERENCES Classroom(Id)
);

-- ENROLLMENT
CREATE TABLE Enrollment (
    Id INT PRIMARY KEY IDENTITY,
    TraineeId INT NOT NULL,
    SessionId INT NOT NULL,
    Status INT NOT NULL,
    EnrollmentDate DATETIME NOT NULL,
    CompletionDate DATETIME NULL,
    PaymentDueDate DATETIME NULL,
    FOREIGN KEY (TraineeId) REFERENCES [User](Id),
    FOREIGN KEY (SessionId) REFERENCES Session(Id)
);

-- ASSESSMENT
CREATE TABLE Assessment (
    Id INT PRIMARY KEY IDENTITY,
    EnrollmentId INT NOT NULL,
    Status INT NOT NULL,
    DueDate DATETIME NOT NULL,
    CompletedBy DATETIME NULL,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollment(Id)
);

-- PAYMENT
CREATE TABLE Payment (
    Id INT PRIMARY KEY IDENTITY,
    EnrollmentId INT NOT NULL,
    Amount FLOAT NOT NULL,
    PaymentDate DATETIME NOT NULL,
    Status INT NOT NULL,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollment(Id)
);

-- BALANCE
CREATE TABLE Balance (
    Id INT PRIMARY KEY IDENTITY,
    EnrollmentId INT NOT NULL,
    AmountDue INT NOT NULL,
    DueDate DATETIME NOT NULL,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollment(Id)
);

-- NOTIFICATION
CREATE TABLE Notification (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    Status INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

-- INSTRUCTOR AVAILABILITY
CREATE TABLE InstructorAvailability (
    Id INT PRIMARY KEY IDENTITY,
    InstructorId INT NOT NULL,
    DayStart INT NOT NULL,
    DayEnd INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES [User](Id)
);

-- TRAINEE CERTIFICATION
CREATE TABLE TraineeCertification (
    Id INT PRIMARY KEY IDENTITY,
    TraineeId INT NOT NULL,
    TrackId INT NOT NULL,
    Status INT NOT NULL,
    FOREIGN KEY (TraineeId) REFERENCES [User](Id),
    FOREIGN KEY (TrackId) REFERENCES Track(Id)
);

-- M:N TABLES LAST

CREATE TABLE CourseTrack (
    CoursesId INT,
    TracksId INT,
    PRIMARY KEY (CoursesId, TracksId),
    FOREIGN KEY (CoursesId) REFERENCES Course(Id),
    FOREIGN KEY (TracksId) REFERENCES Track(Id)
);

CREATE TABLE ClassroomEquipment (
    ClassroomsId INT,
    EquipmentsId INT,
    PRIMARY KEY (ClassroomsId, EquipmentsId),
    FOREIGN KEY (ClassroomsId) REFERENCES Classroom(Id),
    FOREIGN KEY (EquipmentsId) REFERENCES Equipment(Id)
);

-- USERS
INSERT INTO [User] VALUES
('Ali','Ahmad','123',0,'ali@mail.com','99999999',GETDATE()),
('Sara','Mohamed','123',1,'sara@mail.com','88888888',GETDATE());

-- COURSE
INSERT INTO Course VALUES
(1,'C# Basics','Intro',NULL,10,30,100),
(1,'Advanced C#','Advanced',1,15,25,150);

-- TRACK
INSERT INTO Track VALUES
('Backend','Programming');

-- COURSE TRACK
INSERT INTO CourseTrack VALUES (1,1),(2,1);

-- CLASSROOM
INSERT INTO Classroom VALUES ('Room A',30);

-- EQUIPMENT
INSERT INTO Equipment VALUES ('Projector');

-- CLASSROOM EQUIPMENT
INSERT INTO ClassroomEquipment VALUES (1,1);

-- SESSION
INSERT INTO Session VALUES (1,2,1,GETDATE(),'10:00','12:00');

-- ENROLLMENT
INSERT INTO Enrollment VALUES (1,1,0,GETDATE(),NULL,NULL);

-- PAYMENT
INSERT INTO Payment VALUES (1,100,GETDATE(),1);

-- BALANCE
INSERT INTO Balance VALUES (1,50,GETDATE());

-- ASSESSMENT
INSERT INTO Assessment VALUES (1,0,GETDATE(),NULL);

-- NOTIFICATION
INSERT INTO Notification VALUES (1,'Welcome',GETDATE(),0);

-- AVAILABILITY
INSERT INTO InstructorAvailability VALUES (2,0,4,'09:00','17:00');

-- CERTIFICATION
INSERT INTO TraineeCertification VALUES (1,1,1);