/* create database */
IF DB_ID(N'CinemaDb') IS NULL
BEGIN
    CREATE DATABASE CinemaDb;
END

USE CinemaDb;

/* Table: Seats */
IF OBJECT_ID('dbo.Seats','U') IS NULL
BEGIN
    CREATE TABLE dbo.Seats (
        SeatId INT IDENTITY(1,1) PRIMARY KEY,
        RowNumber INT NOT NULL,
        ColNumber INT NOT NULL,
        Status TINYINT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_Seats_Row_Col UNIQUE (RowNumber, ColNumber)
    );
END

/* Table: Tickets */
IF OBJECT_ID('dbo.Tickets','U') IS NULL
BEGIN
    CREATE TABLE dbo.Tickets (
        TicketId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        SeatId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Tickets_Seats FOREIGN KEY (SeatId) REFERENCES dbo.Seats(SeatId)
    );
END

/* Table: Roles */
IF OBJECT_ID('dbo.Roles','U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles(
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL UNIQUE
    );
END

/* Table: Users */
IF OBJECT_ID('dbo.Users','U') IS NULL
BEGIN
    CREATE TABLE dbo.Users(
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(255) NULL,
        PasswordHash VARBINARY(MAX) NOT NULL,
        PasswordSalt VARBINARY(MAX) NOT NULL,
        RoleId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
    );
END
ELSE
BEGIN
    IF COL_LENGTH('dbo.Users','Email') IS NULL
    BEGIN
        ALTER TABLE dbo.Users ADD Email NVARCHAR(255) NULL;
    END
END

/* Seed roles */
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'Admin')
    INSERT INTO dbo.Roles (Name) VALUES ('Admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'User')
    INSERT INTO dbo.Roles (Name) VALUES ('User');

/* Seed seats – only if table exists and is empty */
IF OBJECT_ID('dbo.Seats','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Seats)
    BEGIN
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
END

-- Seed admin (safe insert)
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, Email, PasswordHash, PasswordSalt, RoleId)
    VALUES (
        'admin',
        'admin1@example.com',
        0xCF5FA97AA00D65AE7EE26EDA3DF21D30113E07CE5B969E9C0D8B12F0ED2231EC,
        0x18FE9E3DC7F8F159F2F1FADEA53A4453,
        (SELECT Id FROM dbo.Roles WHERE Name = 'Admin')
    );
END

/* new updates**/

-- 1) Create Halls table if missing
IF OBJECT_ID('dbo.Halls','U') IS NULL
BEGIN
    CREATE TABLE dbo.Halls (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL UNIQUE,
        RowsCount INT NOT NULL DEFAULT 0,
        ColsCount INT NOT NULL DEFAULT 0
    );
END

-- 2) Insert default hall if none exists
IF OBJECT_ID('dbo.Halls','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Halls)
    BEGIN
        INSERT INTO dbo.Halls (Name, RowsCount, ColsCount) VALUES ('Default Hall', 5, 8);
    END
END

-- 3) Add HallId column to Seats table if it doesn't exist
IF OBJECT_ID('dbo.Seats','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Seats','HallId') IS NULL
    BEGIN
        ALTER TABLE dbo.Seats ADD HallId INT NULL;
        
        -- Set default value for existing seats
        DECLARE @defaultHallId INT;
        SELECT TOP 1 @defaultHallId = Id FROM dbo.Halls ORDER BY Id;
        
        IF @defaultHallId IS NOT NULL
        BEGIN
            UPDATE dbo.Seats SET HallId = @defaultHallId WHERE HallId IS NULL;
        END
    END
END

-- 4) Make HallId NOT NULL after setting default values
IF OBJECT_ID('dbo.Seats','U') IS NOT NULL 
    AND COL_LENGTH('dbo.Seats','HallId') IS NOT NULL
BEGIN
    -- Check if there are any NULL values left
    IF NOT EXISTS (SELECT 1 FROM dbo.Seats WHERE HallId IS NULL)
    BEGIN
        -- Drop existing constraint if exists
        IF EXISTS (
            SELECT 1 FROM sys.foreign_keys 
            WHERE parent_object_id = OBJECT_ID('dbo.Seats') 
            AND referenced_object_id = OBJECT_ID('dbo.Halls')
        )
        BEGIN
            DECLARE @fkName NVARCHAR(128);
            SELECT @fkName = name FROM sys.foreign_keys 
            WHERE parent_object_id = OBJECT_ID('dbo.Seats') 
            AND referenced_object_id = OBJECT_ID('dbo.Halls');
            
            EXEC('ALTER TABLE dbo.Seats DROP CONSTRAINT ' + @fkName);
        END
        
        -- Alter column to NOT NULL
        ALTER TABLE dbo.Seats ALTER COLUMN HallId INT NOT NULL;
        
        -- Add foreign key constraint
        ALTER TABLE dbo.Seats
        ADD CONSTRAINT FK_Seats_Halls FOREIGN KEY (HallId) REFERENCES dbo.Halls(Id);
    END
END

-- 5) Create Movies table if missing
IF OBJECT_ID('dbo.Movies','U') IS NULL
BEGIN
    CREATE TABLE dbo.Movies (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        DurationMinutes INT NOT NULL,
        Description NVARCHAR(MAX) NULL
    );
END

-- 6) Create Screenings table if missing
IF OBJECT_ID('dbo.Screenings','U') IS NULL
BEGIN
    CREATE TABLE dbo.Screenings (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MovieId INT NOT NULL,
        HallId INT NOT NULL,
        StartAt DATETIME2 NOT NULL,
        CONSTRAINT FK_Screenings_Movies FOREIGN KEY (MovieId) REFERENCES dbo.Movies(Id),
        CONSTRAINT FK_Screenings_Halls FOREIGN KEY (HallId) REFERENCES dbo.Halls(Id)
    );
END

-- 7) Modify Tickets table
IF OBJECT_ID('dbo.Tickets','U') IS NOT NULL
BEGIN
    -- Add ScreeningId column if it doesn't exist
    IF COL_LENGTH('dbo.Tickets','ScreeningId') IS NULL
    BEGIN
        ALTER TABLE dbo.Tickets ADD ScreeningId INT NULL;
    END
    
    -- Add foreign key constraint for ScreeningId
    IF OBJECT_ID('dbo.Screenings','U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM sys.foreign_keys
            WHERE parent_object_id = OBJECT_ID('dbo.Tickets') 
            AND referenced_object_id = OBJECT_ID('dbo.Screenings')
        )
        BEGIN
            ALTER TABLE dbo.Tickets
            ADD CONSTRAINT FK_Tickets_Screenings FOREIGN KEY (ScreeningId) 
            REFERENCES dbo.Screenings(Id);
        END
    END
    
    -- Add unique constraint for ScreeningId and SeatId
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes 
        WHERE name = 'UQ_Ticket_Screening_Seat' 
        AND object_id = OBJECT_ID('dbo.Tickets')
    )
    BEGIN
        ALTER TABLE dbo.Tickets 
        ADD CONSTRAINT UQ_Ticket_Screening_Seat UNIQUE (ScreeningId, SeatId);
    END
END