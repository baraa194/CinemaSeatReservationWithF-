USE CinemaDb;

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

-- 2) Ensure Seats has HallId column (nullable at first)
IF OBJECT_ID('dbo.Seats','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Seats','HallId') IS NULL
    BEGIN
        ALTER TABLE dbo.Seats ADD HallId INT NULL;
    END
END

-- 3) Insert default hall if none exists
IF OBJECT_ID('dbo.Halls','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Halls)
    BEGIN
        INSERT INTO dbo.Halls (Name, RowsCount, ColsCount) VALUES ('Default Hall', 5, 8);
    END

    DECLARE @defaultHallId INT = (SELECT TOP 1 Id FROM dbo.Halls ORDER BY Id);

    -- populate seats with default hall
    IF OBJECT_ID('dbo.Seats','U') IS NOT NULL
    BEGIN
        UPDATE dbo.Seats
        SET HallId = @defaultHallId
        WHERE HallId IS NULL;
    END
END

-- 4) Make Seats.HallId NOT NULL and add FK
IF OBJECT_ID('dbo.Seats','U') IS NOT NULL AND COL_LENGTH('dbo.Seats','HallId') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Seats WHERE HallId IS NULL)
    BEGIN
        ALTER TABLE dbo.Seats ALTER COLUMN HallId INT NOT NULL;

        IF NOT EXISTS (
            SELECT 1 FROM sys.foreign_keys
            WHERE parent_object_id = OBJECT_ID('dbo.Seats') AND referenced_object_id = OBJECT_ID('dbo.Halls')
        )
        BEGIN
            ALTER TABLE dbo.Seats
            ADD CONSTRAINT FK_Seats_Halls FOREIGN KEY (HallId) REFERENCES dbo.Halls(Id);
        END
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

-- 7) Modify Tickets: add ScreeningId column (nullable) and FK; add unique constraint
IF OBJECT_ID('dbo.Tickets','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Tickets','ScreeningId') IS NULL
    BEGIN
        ALTER TABLE dbo.Tickets ADD ScreeningId INT NULL;
    END

    IF OBJECT_ID('dbo.Screenings','U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM sys.foreign_keys
            WHERE parent_object_id = OBJECT_ID('dbo.Tickets') AND referenced_object_id = OBJECT_ID('dbo.Screenings')
        )
        BEGIN
            ALTER TABLE dbo.Tickets
            ADD CONSTRAINT FK_Tickets_Screenings FOREIGN KEY (ScreeningId) REFERENCES dbo.Screenings(Id);
        END
    END

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes WHERE name = 'UQ_Ticket_Screening_Seat' AND object_id = OBJECT_ID('dbo.Tickets')
    )
    BEGIN
        ALTER TABLE dbo.Tickets ADD CONSTRAINT UQ_Ticket_Screening_Seat UNIQUE (ScreeningId, SeatId);
    END
END

