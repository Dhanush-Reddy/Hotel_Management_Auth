USE HotelDb;
GO

IF OBJECT_ID('dbo.Invoices','U') IS NULL
BEGIN
  CREATE TABLE dbo.Invoices
  (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    BookingId       INT          NOT NULL,  -- FK -> Bookings(Id)
    InvoiceNumber   NVARCHAR(30) NOT NULL,  -- e.g., "INV-2025-00001"
    Nights          INT          NOT NULL,
    NightlyRate     DECIMAL(10,2) NULL,     -- copy from Booking at invoice time
    Subtotal        DECIMAL(10,2) NOT NULL, -- Nights * NightlyRate (NULL rate -> 0)
    Status          NVARCHAR(10) NOT NULL CONSTRAINT DF_Invoices_Status DEFAULT('Unpaid'), -- Unpaid|Paid
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Invoices_CreatedAt DEFAULT SYSUTCDATETIME(),
    PaidAt          DATETIME2(0) NULL
  );

  ALTER TABLE dbo.Invoices
    ADD CONSTRAINT FK_Invoices_Bookings_BookingId
    FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(Id);

  ALTER TABLE dbo.Invoices
    ADD CONSTRAINT CK_Invoices_Status
    CHECK (Status IN ('Unpaid','Paid'));
END
GO

-- Simple unique per number
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'UX_Invoices_InvoiceNumber' AND object_id = OBJECT_ID('dbo.Invoices')
)
BEGIN
  CREATE UNIQUE INDEX UX_Invoices_InvoiceNumber ON dbo.Invoices(InvoiceNumber);
END
GO

-- Helpful lookups
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_Invoices_BookingId' AND object_id = OBJECT_ID('dbo.Invoices')
)
BEGIN
  CREATE INDEX IX_Invoices_BookingId ON dbo.Invoices(BookingId);
END
GO

