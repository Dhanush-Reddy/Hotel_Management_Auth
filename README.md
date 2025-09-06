# Hotel Management (Backend + Frontend)

Monorepo containing a .NET 8 Web API backend and an Angular 17 frontend.

## Repository Structure

```
HotelManagement/
  backend/
    Hotel.Api/
    Hotel.Application/
    Hotel.Infrastructure/
    Hotel.Domain/
    Scripts/
    Hotel.sln
  frontend/
    hotel-web/            # Angular 17 app (SCSS)
```

## Backend

- Framework: ASP.NET Core (.NET 8)
- Data: SQL Server, Dapper, `Microsoft.Data.SqlClient`
- Auth: JWT Bearer (DefaultMapInboundClaims = false; uses `sub`); roles: `Admin`, `Staff`
- PDF: QuestPDF (Community license configured in code)

### Database Setup
1) Ensure a SQL Server instance is available and a database (e.g., `HotelDb`) is created.
2) Execute the migration scripts in order from:
   - `HotelManagement/backend/Scripts/0001_init_db_and_tables.sql` … `0007_invoices_table.sql`
3) Update the connection string `Default` in:
   - `HotelManagement/backend/Hotel.Api/appsettings.json`

### Build

```bash
dotnet build HotelManagement/backend/Hotel.sln
```

### Run API

```bash
# From repo root
dotnet run --project HotelManagement/backend/Hotel.Api

# With hot reload
dotnet watch --project HotelManagement/backend/Hotel.Api run
```

### Notable API Features

- Users/Auth: JWT token issuance (username/password), role-based authorization.
- Rooms: CRUD + status (`Available|Occupied|OutOfService`).
- Guests: Create/update/search with de-dup by email/phone.
- Bookings: Create/list with date-overlap protection, status flow
  (`Pending → Confirmed → CheckedIn → CheckedOut/Cancelled`). EndDate is exclusive.
- Billing/Invoices: Create invoice for `CheckedOut` bookings, generate invoice number
  (`INV-YYYY-#####`), mark paid, and download PDF (`GET /api/invoices/{id}/pdf`).

### JWT Notes

- Token contains `sub` (user id), name, and role claims.
- API is configured with `JwtSecurityTokenHandler.DefaultMapInboundClaims = false`.
- Controllers typically read user id via `User.FindFirst("sub")?.Value`.

## Frontend (Angular)

- Generated with Angular CLI v17, SCSS styles.

```bash
cd HotelManagement/frontend/hotel-web
npm install   # already done by scaffolding, re-run if needed
npm start     # http://localhost:4200
```

Add a proxy config if you want to avoid CORS during local dev (optional):

`HotelManagement/frontend/hotel-web/proxy.conf.json`
```json
{
  "/api": {
    "target": "http://localhost:5206",
    "secure": false,
    "changeOrigin": true
  }
}
```

Then update `angular.json` serve options to include `--proxy-config proxy.conf.json`.

## PDF License (QuestPDF)

`Hotel.Infrastructure/Features/Billing/Services/InvoicePdfService.cs` sets:

```csharp
QuestPDF.Settings.License = LicenseType.Community;
```

If your organization’s annual revenue exceeds $1M USD, switch to:

```csharp
QuestPDF.Settings.License = LicenseType.Commercial;
```

## Useful Commands

- Build backend: `dotnet build HotelManagement/backend/Hotel.sln`
- Run backend: `dotnet run --project HotelManagement/backend/Hotel.Api`
- Run backend (watch): `dotnet watch --project HotelManagement/backend/Hotel.Api run`
- Run frontend: `cd HotelManagement/frontend/hotel-web && npm start`

## Notes

- All server-side SQL scripts are under `HotelManagement/backend/Scripts`.
- Connection strings and JWT settings live in `Hotel.Api/appsettings*.json`.
