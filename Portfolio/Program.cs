using Microsoft.AspNetCore.Identity;
using Portfolio.Application;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Persistence.Seeding;
using Portfolio.WebApi.Middleware;
using Portfolio.WebApi.RateLimiting;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173", "http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                // Lets the frontend read when a rate-limited request can be retried.
                .WithExposedHeaders("Retry-After");
        }
    );
});

// Add Infrastructure (DbContext, repositories, DbInitialiser) and Application (services)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add rate limiting (policies and limits are in the "RateLimiting" section of appsettings.json)
builder.Services.AddApiRateLimiting(builder.Configuration);

// Add Identity services
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        // Password settings
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;

        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add Authentication and Authorization
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };

    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database (runs before app starts serving requests)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitialiser = services.GetRequiredService<DbInitialiser>();
    await dbInitialiser.InitialiseAsync(); // Seeding logic
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// After authentication, so the per-user "pdf" policy knows who is calling
app.UseRateLimiter();

app.MapControllers();

app.Run();

public partial class Program { }
