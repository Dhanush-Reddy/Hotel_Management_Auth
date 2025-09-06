USE HotelDb;
GO

-- ============================================
-- Guests table (customers distinct from Users)
-- Safe to re-run with IF NOT EXISTS guards
-- ============================================

IF OBJECT_ID('dbo.Guests','U') IS NULL
BEGIN
  CREATE TABLE dbo.Guests
  (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    FullName   NVARCHAR(100) NOT NULL,
    Phone      NVARCHAR(30)  NULL,
    Email      NVARCHAR(100) NULL,
    IdProof    NVARCHAR(50)  NULL,   -- e.g., "Passport: AB12345"
    CreatedAt  DATETIME2(0)   NOT NULL CONSTRAINT DF_Guests_CreatedAt DEFAULT SYSUTCDATETIME()
  );
END
GO

-- Optional but recommended: filtered unique indexes so multiple NULLs are allowed
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'UX_Guests_Email' AND object_id = OBJECT_ID('dbo.Guests')
)
BEGIN
  CREATE UNIQUE INDEX UX_Guests_Email
  ON dbo.Guests(Email)
  WHERE Email IS NOT NULL;
END
GO

IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'UX_Guests_Phone' AND object_id = OBJECT_ID('dbo.Guests')
)
BEGIN
  CREATE UNIQUE INDEX UX_Guests_Phone
  ON dbo.Guests(Phone)
  WHERE Phone IS NOT NULL;
END
GO

-- Convenience search index on FullName
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_Guests_FullName' AND object_id = OBJECT_ID('dbo.Guests')
)
BEGIN
  CREATE INDEX IX_Guests_FullName ON dbo.Guests(FullName);
END
GO

