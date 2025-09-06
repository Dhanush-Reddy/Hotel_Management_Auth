using System.Text;
using Hotel.Application.Features.Users.Interfaces;
using Hotel.Application.Features.Users.Services;
using Hotel.Application.Common.Interfaces;
using Hotel.Infrastructure.Common.Data;
using Hotel.Infrastructure.Features.Users.Repositories;
using Hotel.Infrastructure.Features.Auth.Security;
using Hotel.Application.Features.Rooms.Interfaces;
using Hotel.Application.Features.Rooms.Services;
using Hotel.Infrastructure.Features.Rooms.Repositories;
using Hotel.Application.Features.Guests.Interfaces;
using Hotel.Application.Features.Guests.Services;
using Hotel.Infrastructure.Features.Guests.Repositories;
using Hotel.Application.Features.Bookings.Interfaces;
using Hotel.Application.Features.Bookings.Services;
using Hotel.Infrastructure.Features.Bookings.Repositories;
using Hotel.Application.Features.Billing.Interfaces;
using Hotel.Application.Features.Billing.Services;
using Hotel.Infrastructure.Features.Billing.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Repository registration (SQL Server)
var connStr = builder.Configuration.GetConnectionString("Default");
builder.Services.AddSingleton(new SqlConnectionFactory(connStr!));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Other services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// JWT setup
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
// Keep JWT claim types as sent (don't remap 'sub' -> NameIdentifier)
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel.Api",
        Version = "v1"
    });

    // JWT Bearer Auth setup
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {your token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
