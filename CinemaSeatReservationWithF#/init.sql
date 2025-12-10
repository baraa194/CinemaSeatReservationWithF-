/* create database */
IF DB_ID(N'CinemaDb') IS NULL
BEGIN
    PRINT 'Creating database CinemaDb...'
    CREATE DATABASE CinemaDb;
END

USE CinemaDb;

/* Table: Seats */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='Seats')
BEGIN
CREATE TABLE dbo.Seats (
    SeatId INT IDENTITY(1,1) PRIMARY KEY,
    RowNumber INT NOT NULL,
    ColNumber INT NOT NULL,
    Status TINYINT NOT NULL DEFAULT 0,
    CONSTRAINT UQ_Seats_Row_Col UNIQUE (RowNumber, ColNumber)
)
END

/* Table: Tickets */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='Tickets')
BEGIN
CREATE TABLE dbo.Tickets (
    TicketId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SeatId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Tickets_Seats FOREIGN KEY (SeatId) REFERENCES dbo.Seats(SeatId)
)
END

/* Table: Roles */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='Roles')
BEGIN
CREATE TABLE dbo.Roles(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
)
END

/* Table: Users */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='Users')
BEGIN
CREATE TABLE dbo.Users(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARBINARY(MAX) NOT NULL,
    PasswordSalt VARBINARY(MAX) NOT NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
)
END

/* Seed roles */
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
    INSERT INTO Roles (Name) VALUES ('Admin');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
    INSERT INTO Roles (Name) VALUES ('User');

/* Seed seats – only if table is empty */
IF NOT EXISTS (SELECT 1 FROM Seats)
BEGIN
    PRINT 'Seeding seats...';
    DECLARE @r INT = 1;
    WHILE @r <= 5
    BEGIN
        DECLARE @c INT = 1;
        WHILE @c <= 8
        BEGIN
            INSERT INTO dbo.Seats (RowNumber, ColNumber, Status)
            VALUES (@r, @c, 0);
            SET @c = @c + 1;
        END
        SET @r = @r + 1;
    END
END

PRINT 'Init script finished.';

-- Seed  admin 
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, RoleId)
    VALUES ('admin', 'admin1@example.com',
    0xcf5fa97aa00d65ae7ee26eda3df21d30113e07ce5b969e9c0d8b12f0ed2231ec
, 0x18fe9e3dc7f8f159f2f1fadea53a4453, (SELECT Id FROM Roles WHERE Name = 'Admin'));
END


