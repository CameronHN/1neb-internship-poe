using Bogus;
using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Entities;

namespace Portfolio.Infrastructure.Persistence.Seeding
{
    public class DbInitialiser(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;
        private static readonly string[] genders = ["Male", "Female", "Prefer not to say"];
        private static readonly string[] proficiencyLevels = ["Beginner", "Intermediate", "Advanced", "Expert"];
        private static readonly string[] institutionTypes = ["High School", "University", "University of Technology"];

        public async Task InitialiseAsync()
        {
            Randomizer.Seed = new Random(1234); // Deterministic

            _context.Database.Migrate();

            await SeedUserData();
        }
        private async Task SeedUserData()
        {
            List<User> users = [];

            if (!_context.Users.Any())
            {
                var faker = new Faker<User>()
                    .RuleFor(u => u.Id, _ => Guid.NewGuid())
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.Gender, f => f.PickRandom(genders))
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber(format: "0#########"))
                    .RuleFor(u => u.Email, f => f.Internet.Email());

                users = faker.Generate(10);

                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }
            else
            {
                users = await _context.Users.ToListAsync();
            }

            if (!_context.Experiences.Any())
            {
                var experienceFaker = new Faker<Experience>()
                    .RuleFor(e => e.Id, _ => Guid.NewGuid())
                    .RuleFor(e => e.JobTitle, f => f.Name.JobTitle())
                    .RuleFor(e => e.CompanyName, f => f.PickRandom(f.Company.CompanyName(), null))
                    .RuleFor(e => e.StartDate, f => f.Date.PastDateOnly(5))
                    .RuleFor(e => e.EndDate, (f, e) => f.Date.BetweenDateOnly(e.StartDate, DateOnly.FromDateTime(DateTime.UtcNow)))
                    .RuleFor(e => e.UserId, f => f.PickRandom(users).Id);

                var experiences = experienceFaker.Generate(20);

                await _context.Experiences.AddRangeAsync(experiences);
                await _context.SaveChangesAsync();
            }

            if (!_context.ProfessionalSummaries.Any())
            {
                var summaryFaker = new Faker<ProfessionalSummary>()
                    .RuleFor(s => s.Id, _ => Guid.NewGuid())
                    .RuleFor(s => s.Summary, f => f.Lorem.Sentence(10))
                    .RuleFor(s => s.UserId, f => f.PickRandom(users).Id);

                var summaries = summaryFaker.Generate(10);

                await _context.ProfessionalSummaries.AddRangeAsync(summaries);
                await _context.SaveChangesAsync();
            }

            if (!_context.Skills.Any())
            {
                var skillFaker = new Faker<Skill>()
                    .RuleFor(s => s.Id, _ => Guid.NewGuid())
                    .RuleFor(s => s.SkillName, f => f.Name.JobArea())
                    .RuleFor(s => s.ProficiencyLevel, f => f.PickRandom(proficiencyLevels))
                    .RuleFor(s => s.UserId, f => f.PickRandom(users).Id);

                var skills = skillFaker.Generate(40);

                await _context.Skills.AddRangeAsync(skills);
                await _context.SaveChangesAsync();
            }

            if (!_context.Educations.Any())
            {
                var educationFaker = new Faker<Education>()
                    .RuleFor(e => e.Id, _ => Guid.NewGuid())
                    .RuleFor(e => e.InstitutionName, f => f.Lorem.Words() + f.PickRandom(institutionTypes))
                    .RuleFor(e => e.Qualification, f => f.Lorem.Word())
                    .RuleFor(e => e.Major, f => f.Lorem.Word())
                    .RuleFor(e => e.Achievement, f => f.Lorem.Word())
                    .RuleFor(e => e.StartDate, f => f.Date.PastDateOnly(7))
                    .RuleFor(e => e.EndDate, (f, e) => f.Date.BetweenDateOnly(e.StartDate, DateOnly.FromDateTime(DateTime.UtcNow)))
                    .RuleFor(e => e.UserId, f => f.PickRandom(users).Id);

                var educations = educationFaker.Generate(40);

                await _context.Educations.AddRangeAsync(educations);
                await _context.SaveChangesAsync();
            }
        }

    }
}
