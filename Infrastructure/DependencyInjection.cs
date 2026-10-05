using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Persistence.Seeding;
using Portfolio.Infrastructure.Repositories;

namespace Portfolio.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Portfolio.Infrastructure")
                )
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IExperienceRepository, ExperienceRepository>();
            services.AddScoped<ICertificationRepository, CertificationRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IEducationRepository, EducationRepository>();
            services.AddScoped<IProfessionalLinkRepository, ProfessionalLinkRepository>();
            services.AddScoped<IProfessionalSummaryRepository, ProfessionalSummaryRepository>();
            services.AddScoped<ITitleRepository, TitleRepository>();
            services.AddScoped<ISavedResumeRepository, SavedResumeRepository>();

            services.AddScoped<DbInitialiser>();

            return services;
        }
    }
}
