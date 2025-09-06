# Hotel Solution

This repository contains a .NET solution for a hotel management system, structured as follows:

- **Hotel.Api**: ASP.NET Core Web API project
- **Hotel.Application**: Application layer class library
- **Hotel.Infrastructure**: Infrastructure layer class library
- **Hotel.Domain**: Domain layer class library

## Project References
- Hotel.Api references Hotel.Application and Hotel.Infrastructure
- Hotel.Application and Hotel.Infrastructure reference Hotel.Domain

## NuGet Packages
- Hotel.Api: Microsoft.AspNetCore.Authentication.JwtBearer, Swashbuckle.AspNetCore
- Hotel.Infrastructure: Microsoft.Data.SqlClient, Dapper, BCrypt.Net-Next

## Build
To build the solution:

```sh
dotnet build Hotel.sln
```

## Run
To run the Web API:

```sh
dotnet run --project Hotel.Api/Hotel.Api.csproj
```

---

This solution is ready for further development.
