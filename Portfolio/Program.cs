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

// Check the security stamp on every request, so logout and password changes end other
// sessions straight away instead of after the default 30 minutes.
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.Zero
);

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

    // Lax unless configured. appsettings.Development.json sets None, because the dev frontend
    // (http://localhost:5173) and the API (https://localhost:7165) count as different sites.
    options.Cookie.SameSite = builder.Configuration.GetValue(
        "Auth:CookieSameSite",
        SameSiteMode.Lax
    );
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations, and seed fake data in Development (runs before app starts serving requests)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitialiser = services.GetRequiredService<DbInitialiser>();
    var seed =
        app.Environment.IsDevelopment()
        || app.Configuration.GetValue<bool>("Database:SeedOnStartup");
    await dbInitialiser.InitialiseAsync(seed);
}

// Security headers on every response, including errors and 429s.
app.Use(
    async (context, next) =>
    {
        var headers = context.Response.Headers;
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "no-referrer";

        // Swagger UI is an HTML page with scripts, so it cannot use the API's strict policy.
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";
        }

        await next();
    }
);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
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
