using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBookingSystem.Authorization;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Middleware;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services;
using MovieTicketBookingSystem.Services.Interfaces;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IMovieRepository, MovieRepository>();

builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();

builder.Services.AddScoped<IScreenRepository, ScreenRepository>();

builder.Services.AddScoped<ISeatRepository, SeatRepository>();

builder.Services.AddScoped<IShowRepository, ShowRepository>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();


builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<ITheatreService, TheatreService>();

builder.Services.AddScoped<IScreenService, ScreenService>();

builder.Services.AddScoped<ISeatService, SeatService>();

builder.Services.AddScoped<IShowService, ShowService>();

builder.Services.AddScoped<IBookingService, BookingService>();

// User services and repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// JWT token creation service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.BookingOwner,
        policy =>
        {
            policy.RequireAuthenticatedUser();

            policy.AddRequirements(
                new BookingOwnerRequirement());
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await RoleSeeder.SeedAsync(app.Services, app.Configuration);
    app.MapOpenApi();
    app.MapScalarApiReference();
}


// 1. HTTPS Redirection
app.UseHttpsRedirection();

// 2. Routing
//app.UseRouting();

// 3. Endpoint Metadata Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<EndpointMetadataMiddleware>();
}

// 4. Authentication
// Validates JWT and populates HttpContext.User
app.UseAuthentication();

// 5. Authentication and Authorization Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthenticationDiagnosticsMiddleware>();

    app.UseMiddleware<AuthorizationDiagnosticsMiddleware>();
}

// 6. Authorization
// Checks endpoint access permissions
app.UseAuthorization();


// ==========================================
// 12. MAP CONTROLLERS
// ==========================================

app.MapControllers();


// ==========================================
// 13. RUN APPLICATION
// ==========================================

app.Run();

