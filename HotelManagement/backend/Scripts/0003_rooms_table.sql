USE HotelDb;
GO

IF OBJECT_ID('dbo.Rooms','U') IS NULL
BEGIN
  CREATE TABLE dbo.Rooms(
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber   NVARCHAR(20) NOT NULL UNIQUE,  -- e.g., "101", "A-203"
    Capacity     INT NOT NULL CONSTRAINT DF_Rooms_Capacity DEFAULT(2),
    Status       NVARCHAR(20) NOT NULL CONSTRAINT DF_Rooms_Status DEFAULT('Available'), -- Available | Occupied | OutOfService
    CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Rooms_Created DEFAULT SYSUTCDATETIME()
  );
END
GO
