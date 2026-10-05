using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Services;
using Portfolio.Core.Contracts.Services;

namespace Portfolio.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IExperienceService, ExperienceService>();
            services.AddScoped<ICertificationService, CertificationService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IProfessionalLinkService, ProfessionalLinkService>();
            services.AddScoped<IProfessionalSummaryService, ProfessionalSummaryService>();
            services.AddScoped<ITitleService, TitleService>();
            services.AddScoped<ISavedResumeService, SavedResumeService>();
            services.AddScoped<IResumeDataService, ResumeDataService>();

            // Stateless and has no constructor dependencies, so a singleton is safe.
            // If a scoped dependency is ever added to its constructor, change this to AddScoped.
            services.AddSingleton<IResumeGenerationService, ResumeGenerationService>();

            return services;
        }
    }
}
