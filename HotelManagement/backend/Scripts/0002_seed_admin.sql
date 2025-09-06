-- ============================================
-- Seed an initial Admin user
-- Replace PASSWORD_HASH below with a real hash.
-- For BCrypt: starts with $2a$ / $2b$ / $2y$
-- ============================================

USE HotelDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin')
BEGIN
  INSERT INTO dbo.Users(Username, PasswordHash, Role, ActiveFlag)
  VALUES
  (
    N'admin',
    N'$2b$12$REPLACE_WITH_YOUR_BCRYPT_HASH',  -- <-- paste full hash here
    N'Admin',
    1
  );
END
GO

