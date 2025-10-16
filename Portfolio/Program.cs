using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Services;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Persistence.Seeding;
using Portfolio.Infrastructure.Repositories;
using Portfolio.WebApi.Middleware;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IExperienceRepository, ExperienceRepository>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();

builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<ICertificationService, CertificationService>();

builder.Services.AddScoped<IResumeService, ResumeService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Portfolio.Infrastructure")
    )
);

// Register DbInitialiser
builder.Services.AddScoped<DbInitialiser>();

var app = builder.Build();

// Seed the database (runs before app starts serving requests)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitialiser = services.GetRequiredService<DbInitialiser>();
    await dbInitialiser.InitialiseAsync(); // Seeding logic
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
