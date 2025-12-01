
/* create database  */
IF DB_ID(N'CinemaDb') IS NULL
BEGIN
    PRINT 'Creating database CinemaDb...'
    CREATE DATABASE CinemaDb;
END
GO

USE CinemaDb;
GO


IF OBJECT_ID(N'dbo.Seats', N'U') IS NOT NULL
    DROP TABLE dbo.Seats;
GO

CREATE TABLE dbo.Seats (
    SeatId INT IDENTITY(1,1) PRIMARY KEY,
    RowNumber INT NOT NULL,
    ColNumber INT NOT NULL,
    Status TINYINT NOT NULL DEFAULT 0, -- 0=Available,1=Booked
    CONSTRAINT UQ_Seats_Row_Col UNIQUE (RowNumber, ColNumber)
);
GO

/* Table: Tickets */
IF OBJECT_ID(N'dbo.Tickets', N'U') IS NOT NULL
    DROP TABLE dbo.Tickets;
GO

CREATE TABLE dbo.Tickets (
    TicketId UNIQUEIDENTIFIER PRIMARY KEY,
    SeatId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_Tickets_Seats FOREIGN KEY (SeatId) REFERENCES dbo.Seats(SeatId)
);
GO

/* seed seats */
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
GO

PRINT 'Init script finished.';



