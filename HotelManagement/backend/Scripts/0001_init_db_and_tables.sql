-- ============================================
-- HotelDb | Init DB + Core Tables (no SPs)
-- Safe to re-run (IF NOT EXISTS guards)
-- ============================================

IF DB_ID('HotelDb') IS NULL CREATE DATABASE HotelDb;
GO
USE HotelDb;
GO

-- =========================
-- Users (Auth + Admin/Staff)
-- =========================
IF OBJECT_ID('dbo.Users','U') IS NULL
BEGIN
  CREATE TABLE dbo.Users(
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(200) NOT NULL,     -- BCrypt or ASP.NET Identity hash
    Role         NVARCHAR(20)  NOT NULL,     -- 'Admin' | 'Staff'
    ActiveFlag   BIT           NOT NULL CONSTRAINT DF_Users_Active DEFAULT(1),
    CreatedAt    DATETIME2(0)  NOT NULL CONSTRAINT DF_Users_Created DEFAULT SYSUTCDATETIME()
  );
END
GO

-- Optional: simple unique index if not auto-created by UNIQUE constraint
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes 
  WHERE name = 'UX_Users_Username' AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
  CREATE UNIQUE INDEX UX_Users_Username ON dbo.Users(Username);
END
GO

-- =========================
-- (Optional) AuthLog (audit)
-- =========================
IF OBJECT_ID('dbo.AuthLog','U') IS NULL
BEGIN
  CREATE TABLE dbo.AuthLog(
    Id         BIGINT IDENTITY(1,1) PRIMARY KEY,
    Username   NVARCHAR(50) NULL,
    Success    BIT NOT NULL,
    IpAddress  NVARCHAR(45) NULL,
    CreatedAt  DATETIME2(0) NOT NULL CONSTRAINT DF_AuthLog_Created DEFAULT SYSUTCDATETIME()
  );
END
GO

