using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Entities;

namespace Portfolio.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Certification> Certification { get; set; }

        public DbSet<Contact> Contact { get; set; }

        public DbSet<Education> Education { get; set; }

        public DbSet<Experience> Experience { get; set; }

        public DbSet<ExperienceResponsibility> ExperienceResponsibility { get; set; }

        public DbSet<ProfessionalSummary> ProfessionalSummary { get; set; }

        public DbSet<Resume> Resume { get; set; }

        public DbSet<ResumeTemplate> ResumeTemplate { get; set; }

        public DbSet<Skill> Skill { get; set; }

        public DbSet<Title> Title { get; set; }

        public DbSet<ApplicationUser> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Override to configure the database provider
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Data source=(localdb)\\MSSQLLocalDb;Initial catalog=ProjectDb;Trusted_Connection=True;TrustServerCertificate=True;"
                );
            }
        }
    }
}
