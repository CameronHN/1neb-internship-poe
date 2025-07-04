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

        public static string CapitalizeEfficiently(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            char[] chars = input.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }

        public async Task InitialiseAsync()
        {
            Randomizer.Seed = new Random(1234); // Deterministic

            _context.Database.Migrate();

            await SeedUserData();
        }
        private async Task SeedUserData()
        {
            List<User> users = [];
            List<Experience> experiences = [];

            // ======== Users ========
            if (!_context.User.Any())
            {
                var faker = new Faker<User>()
                    .RuleFor(u => u.Id, _ => Guid.NewGuid())
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.Gender, f => f.PickRandom(genders))
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber(format: "0#########"))
                    .RuleFor(u => u.Email, f => f.Internet.Email());

                users = faker.Generate(10);

                await _context.User.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }
            else
            {
                users = await _context.User.ToListAsync();
            }

            // ======== Experiences ========
            if (!_context.Experience.Any())
            {
                var experienceFaker = new Faker<Experience>()
                    .RuleFor(e => e.Id, _ => Guid.NewGuid())
                    .RuleFor(e => e.JobTitle, f => f.Name.JobTitle())
                    .RuleFor(e => e.CompanyName, f => f.PickRandom(f.Company.CompanyName(), null))
                    .RuleFor(e => e.StartDate, f => f.Date.PastDateOnly(5))
                    .RuleFor(e => e.EndDate, (f, e) => f.Date.BetweenDateOnly(e.StartDate, DateOnly.FromDateTime(DateTime.UtcNow)))
                    .RuleFor(e => e.UserId, f => f.PickRandom(users).Id);

                experiences = experienceFaker.Generate(20);

                await _context.Experience.AddRangeAsync(experiences);
                await _context.SaveChangesAsync();
            }
            else
            {
                experiences = await _context.Experience.ToListAsync();
            }

            // ======== Professional Summaries ========
            if (!_context.ProfessionalSummary.Any())
            {
                var summaryFaker = new Faker<ProfessionalSummary>()
                    .RuleFor(s => s.Id, _ => Guid.NewGuid())
                    .RuleFor(s => s.Summary, f => f.Lorem.Sentence(10))
                    .RuleFor(s => s.UserId, f => f.PickRandom(users).Id);

                var summaries = summaryFaker.Generate(10);

                await _context.ProfessionalSummary.AddRangeAsync(summaries);
                await _context.SaveChangesAsync();
            }

            // ======== Skills ========
            if (!_context.Skill.Any())
            {
                var skillFaker = new Faker<Skill>()
                    .RuleFor(s => s.Id, _ => Guid.NewGuid())
                    .RuleFor(s => s.SkillName, f => f.Name.JobArea())
                    .RuleFor(s => s.ProficiencyLevel, f => f.PickRandom(proficiencyLevels))
                    .RuleFor(s => s.UserId, f => f.PickRandom(users).Id);

                var skills = skillFaker.Generate(40);

                await _context.Skill.AddRangeAsync(skills);
                await _context.SaveChangesAsync();
            }

            // ======== Educations ========
            if (!_context.Education.Any())
            {
                var educationFaker = new Faker<Education>()
                    .RuleFor(e => e.Id, _ => Guid.NewGuid())
                    .RuleFor(e => e.InstitutionName, f => $"{CapitalizeEfficiently(f.Random.Word())} {f.PickRandom(institutionTypes)}")
                    .RuleFor(e => e.Qualification, f => CapitalizeEfficiently(f.Random.Word()))
                    .RuleFor(e => e.Major, f => CapitalizeEfficiently(f.Random.Word()))
                    .RuleFor(e => e.Achievement, f => CapitalizeEfficiently(f.Random.Word()))
                    .RuleFor(e => e.StartDate, f => f.Date.PastDateOnly(7))
                    .RuleFor(e => e.EndDate, (f, e) => f.Date.BetweenDateOnly(e.StartDate, DateOnly.FromDateTime(DateTime.UtcNow)))
                    .RuleFor(e => e.UserId, f => f.PickRandom(users).Id);

                var educations = educationFaker.Generate(40);

                await _context.Education.AddRangeAsync(educations);
                await _context.SaveChangesAsync();
            }

            // ======== Contacts ========
            if (!_context.Contact.Any())
            {
                var contactFaker = new Faker<Contact>()
                    .RuleFor(c => c.Id, _ => Guid.NewGuid())
                    .RuleFor(c => c.LinkedIn, f => f.Internet.Url())
                    .RuleFor(c => c.GitHub, f => f.Internet.Url())
                    .RuleFor(e => e.UserId, f => f.PickRandom(users).Id);

                var contacts = contactFaker.Generate(20);

                await _context.Contact.AddRangeAsync(contacts);
                await _context.SaveChangesAsync();
            }

            // ======== Certifications ========
            if (!_context.Certification.Any())
            {
                var certificationFaker = new Faker<Certification>()
                    .RuleFor(c => c.Id, _ => Guid.NewGuid())
                    .RuleFor(c => c.CertificationName, f => f.Name.JobArea())
                    .RuleFor(c => c.IssuingOrganisation, f => f.Company.CompanyName())
                    .RuleFor(c => c.IssuedDate, f => f.Date.PastDateOnly())
                    .RuleFor(c => c.ExpiryDate, (f, c) =>
                        f.Random.Bool()
                            ? (c.IssuedDate.HasValue
                                ? f.Date.BetweenDateOnly(c.IssuedDate.Value, DateOnly.FromDateTime(DateTime.UtcNow))
                                : DateOnly.FromDateTime(DateTime.UtcNow))
                            : null)
                    .RuleFor(c => c.CredentialUrl, f => f.PickRandom(f.Internet.Url(), null))
                    .RuleFor(c => c.UserId, f => f.PickRandom(users).Id);

                var certifications = certificationFaker.Generate(20);

                await _context.Certification.AddRangeAsync(certifications);
                await _context.SaveChangesAsync();
            }

            // ======== Experience Responsibilities ========
            if (!_context.ExperienceResponsibility.Any())
            {
                var responsibilityFaker = new Faker<ExperienceResponsibility>()
                    .RuleFor(r => r.Id, _ => Guid.NewGuid())
                    .RuleFor(r => r.Responsibility, f => f.Lorem.Sentence())
                    .RuleFor(r => r.ExperienceId, f => f.PickRandom(experiences).Id);

                var responsibilities = responsibilityFaker.Generate(40);

                await _context.ExperienceResponsibility.AddRangeAsync(responsibilities);
                await _context.SaveChangesAsync();
            }
        }
    }
}
