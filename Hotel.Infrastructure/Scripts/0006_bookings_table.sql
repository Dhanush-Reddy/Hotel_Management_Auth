USE HotelDb;
GO

-- ============================================================
-- Bookings table
--  - Dates: Exclusive EndDate (stay is [StartDate, EndDate))
--  - Status: Pending | Confirmed | CheckedIn | CheckedOut | Cancelled
--  - FKs: Rooms(Id), Guests(Id), Users(Id) as CreatedByUserId
--  - Indexes for fast overlap checks and filtering
-- ============================================================

IF OBJECT_ID('dbo.Bookings','U') IS NULL
BEGIN
  CREATE TABLE dbo.Bookings
  (
    Id               INT IDENTITY(1,1) PRIMARY KEY,

    RoomId           INT           NOT NULL,   -- FK -> Rooms(Id)
    GuestId          INT           NOT NULL,   -- FK -> Guests(Id)
    StartDate        DATE          NOT NULL,
    EndDate          DATE          NOT NULL,   -- EXCLUSIVE (EndDate is checkout date)
    Status           NVARCHAR(20)  NOT NULL CONSTRAINT DF_Bookings_Status DEFAULT('Pending'),
    NightlyRate      DECIMAL(10,2) NULL,      -- optional for billing later
    Notes            NVARCHAR(200) NULL,

    CreatedByUserId  INT           NOT NULL,   -- FK -> Users(Id), staff/admin who created the booking
    CreatedAt        DATETIME2(0)  NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT SYSUTCDATETIME(),

    -- Convenience computed nights (End - Start); assumes exclusive EndDate
    Nights AS DATEDIFF(DAY, StartDate, EndDate)
  );

  -- FKs (restrict deletes)
  ALTER TABLE dbo.Bookings
    ADD CONSTRAINT FK_Bookings_Rooms_RoomId
      FOREIGN KEY (RoomId) REFERENCES dbo.Rooms(Id);

  ALTER TABLE dbo.Bookings
    ADD CONSTRAINT FK_Bookings_Guests_GuestId
      FOREIGN KEY (GuestId) REFERENCES dbo.Guests(Id);

  ALTER TABLE dbo.Bookings
    ADD CONSTRAINT FK_Bookings_Users_CreatedByUserId
      FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id);

  -- Check constraints
  ALTER TABLE dbo.Bookings
    ADD CONSTRAINT CK_Bookings_Start_End
      CHECK (StartDate < EndDate);  -- no zero/negative length

  ALTER TABLE dbo.Bookings
    ADD CONSTRAINT CK_Bookings_Status
      CHECK (Status IN ('Pending','Confirmed','CheckedIn','CheckedOut','Cancelled'));
END
GO

-- ============================================================
-- Indexes
-- ============================================================

-- For overlap detection: queries filter by RoomId and date range
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_Bookings_RoomId_Start_End' AND object_id = OBJECT_ID('dbo.Bookings')
)
BEGIN
  CREATE INDEX IX_Bookings_RoomId_Start_End
    ON dbo.Bookings(RoomId, StartDate, EndDate);
END
GO

-- Filter by GuestId (history)
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_Bookings_GuestId' AND object_id = OBJECT_ID('dbo.Bookings')
)
BEGIN
  CREATE INDEX IX_Bookings_GuestId
    ON dbo.Bookings(GuestId);
END
GO

-- Filter by Status
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_Bookings_Status' AND object_id = OBJECT_ID('dbo.Bookings')
)
BEGIN
  CREATE INDEX IX_Bookings_Status
    ON dbo.Bookings(Status);
END
GO

-- Optional: created by user lookups
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_Bookings_CreatedByUserId' AND object_id = OBJECT_ID('dbo.Bookings')
)
BEGIN
  CREATE INDEX IX_Bookings_CreatedByUserId
    ON dbo.Bookings(CreatedByUserId);
END
GO

-- Notes:
-- 1) Overlap validation should be enforced in the repository/service using:
--      (@StartDate < b.EndDate AND b.StartDate < @EndDate)
--    with Status IN ('Pending','Confirmed','CheckedIn') to block conflicts.
-- 2) Keep EndDate exclusive to simplify nights and overlap logic.

