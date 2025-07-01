using Bogus;
using Microsoft.EntityFrameworkCore;
using Portfolio.Core.Entities;

namespace Portfolio.Infrastructure.Persistence.Seeding
{
    public class DbInitialiser
    {
        private readonly ApplicationDbContext _context;
        private static readonly string[] genders = ["Male", "Female", "Prefer not to say"];

        public DbInitialiser(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task InitialiseAsync()
        {
            Randomizer.Seed = new Random(1234); // Seed for reproducibility

            _context.Database.Migrate();
            // Seed the database with initial data
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
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(u => u.Email, f => f.Internet.Email());

                users = faker.Generate(10); // Generate 10 random users

                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }
            else
            {
                users = await _context.Users.ToListAsync(); // Load existing users if already seeded
            }

            if (!_context.Experiences.Any())
            {
                var experienceFaker = new Faker<Experience>()
                    .RuleFor(e => e.Id, _ => Guid.NewGuid())
                    .RuleFor(e => e.JobTitle, f => f.Name.JobTitle())
                    .RuleFor(e => e.CompanyName, f => f.Company.CompanyName())
                    .RuleFor(e => e.StartDate, f => f.Date.PastDateOnly(5))
                    .RuleFor(e => e.EndDate, (f, e) => f.Date.BetweenDateOnly(e.StartDate, DateOnly.FromDateTime(DateTime.UtcNow)))
                    .RuleFor(e => e.UserId, f => f.PickRandom(users).Id); // Assign a random user

                var experiences = experienceFaker.Generate(20);

                await _context.Experiences.AddRangeAsync(experiences);
                await _context.SaveChangesAsync();
            }
        }
    }
}
