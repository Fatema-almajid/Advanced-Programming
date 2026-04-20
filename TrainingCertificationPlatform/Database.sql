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

-- InstructorExpertise 
CREATE TABLE InstructorExpertise (
    InstructorId INT,
    CourseId INT,
    PRIMARY KEY (InstructorId, CourseId),
    FOREIGN KEY (InstructorId) REFERENCES [User](Id),
    FOREIGN KEY (CourseId) REFERENCES Course(Id)
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

-- M:N TABLES

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

-- INSERT DATA (FIXED)

INSERT INTO [User] (FirstName, LastName, Password, Role, Email, Phone, RegistrationDate) VALUES
('Ali','Ahmad','123',0,'ali@mail.com','99999999',GETDATE()),
('Sara','Mohamed','123',1,'sara@mail.com','88888888',GETDATE());

INSERT INTO Course (Category, Title, Description, PrerequisiteId, Duration, Capacity, Fee) VALUES
(0,'C# Basics','Intro',NULL,10,30,100),
(0,'Advanced C#','Advanced',1,15,25,150);

INSERT INTO InstructorExpertise VALUES (2,1),(2,2);

INSERT INTO Track (Name, Description) VALUES
('Backend','Programming');

INSERT INTO CourseTrack VALUES (1,1),(2,1);

INSERT INTO Classroom (Name, Seats) VALUES ('Room A',30);

INSERT INTO Equipment (Name) VALUES ('Projector');

INSERT INTO ClassroomEquipment VALUES (1,1);

INSERT INTO Session (CourseId, InstructorId, ClassroomId, SessionDate, StartTime, EndTime)
VALUES (1,2,1,GETDATE(),'10:00','12:00');

INSERT INTO Enrollment (TraineeId, SessionId, Status, EnrollmentDate)
VALUES (1,1,0,GETDATE());

INSERT INTO Payment (EnrollmentId, Amount, PaymentDate, Status)
VALUES (1,100,GETDATE(),1);

INSERT INTO Balance (EnrollmentId, AmountDue, DueDate)
VALUES (1,50,GETDATE());

INSERT INTO Assessment (EnrollmentId, Status, DueDate)
VALUES (1,0,GETDATE());

INSERT INTO Notification (UserId, Message, CreatedDate, Status)
VALUES (1,'Welcome',GETDATE(),0);

INSERT INTO InstructorAvailability (InstructorId, DayStart, DayEnd, StartTime, EndTime)
VALUES (2,0,4,'09:00','17:00');

INSERT INTO TraineeCertification (TraineeId, TrackId, Status)
VALUES (1,1,1);