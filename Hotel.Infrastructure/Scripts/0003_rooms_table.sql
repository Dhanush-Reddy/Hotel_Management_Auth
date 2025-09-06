-- ============================================
-- Rooms table (basic)
-- Safe to re-run
-- ============================================

USE HotelDb;
GO

IF OBJECT_ID('dbo.Rooms','U') IS NULL
BEGIN
  CREATE TABLE dbo.Rooms(
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Number       NVARCHAR(20)  NOT NULL UNIQUE,
    Type         NVARCHAR(50)  NOT NULL,  -- e.g., Standard, Deluxe
    Status       NVARCHAR(20)  NOT NULL,  -- e.g., Available, Occupied, Cleaning
    ActiveFlag   BIT           NOT NULL CONSTRAINT DF_Rooms_Active DEFAULT(1),
    CreatedAt    DATETIME2(0)  NOT NULL CONSTRAINT DF_Rooms_Created DEFAULT SYSUTCDATETIME()
  );
END
GO

