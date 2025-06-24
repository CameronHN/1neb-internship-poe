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

            // User to Education (One-to-Many)
            modelBuilder.Entity<Education>()
                .HasOne<User>()
                .WithMany(u => u.Educations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User to Experience (One-to-Many)
            modelBuilder.Entity<Experience>()
                .HasOne<User>()
                .WithMany(u => u.Experiences)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User to ProfessionalSummary (One-to-Many)
            modelBuilder.Entity<ProfessionalSummary>()
                .HasOne<User>()
                .WithMany(u => u.ProfessionalSummaries)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User to Skill (One-to-Many)
            modelBuilder.Entity<Skill>()
                .HasOne<User>()
                .WithMany(u => u.Skills)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User to Contact (One-to-Many)
            modelBuilder.Entity<Contact>()
                .HasOne<User>()
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User to Resume (One-to-Many)
            modelBuilder.Entity<Resume>()
                .HasOne<User>()
                .WithMany(u => u.Resumes)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User to Certification (One-to-Many)
            modelBuilder.Entity<Certification>()
                .HasOne<User>()
                .WithMany(u => u.Certifications)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Resume to ResumeTemplate (One-to-One)
            modelBuilder.Entity<Resume>()
                .HasOne<ResumeTemplate>()
                .WithMany()
                .HasForeignKey(r => r.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
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
