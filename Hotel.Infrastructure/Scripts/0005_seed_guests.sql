USE HotelDb;
GO

-- ============================================
-- Seed data for Guests (for testing only)
-- Run AFTER 0004_guests_table.sql
-- ============================================

INSERT INTO dbo.Guests (FullName, Phone, Email, IdProof)
VALUES
  ('John Doe',    '+1-202-555-0101', 'john.doe@example.com',    'Passport: A1234567'),
  ('Jane Smith',  '+1-202-555-0123', 'jane.smith@example.com',  'DriverLicense: D998877'),
  ('Rahul Kumar', '+91-9876543210',  'rahul.kumar@example.in',  'Aadhar: 1234-5678-9101'),
  ('Maria Lopez', NULL,              'maria.lopez@example.mx',  'Passport: M7654321'),
  ('Chen Wei',    '+86-13800138000', NULL,                      'NationalID: CN445566');
GO
