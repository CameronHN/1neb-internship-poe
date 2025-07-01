using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Entities;

namespace Portfolio.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Education> Educations { get; set; }

        public DbSet<Experience> Experiences { get; set; }

        public DbSet<ProfessionalSummary> ProfessionalSummaries { get; set; }

        public DbSet<Resume> Resumes { get; set; }

        public DbSet<ResumeTemplate> ResumeTemplates { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Override to configure the database provider
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data source=(localdb)\\MSSQLLocalDb;Initial catalog=ProjectDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}
