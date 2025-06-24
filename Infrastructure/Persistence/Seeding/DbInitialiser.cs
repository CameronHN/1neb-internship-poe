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
            //Randomizer.Seed = new Random(1234); // Seed for reproducibility

            _context.Database.Migrate();
            // Seed the database with initial data
            await SeedUserData();

        }
        private async Task SeedUserData()
        {
            if (!_context.Users.Any())
            {
                var faker = new Faker<User>()
                    .RuleFor(u => u.Id, _ => Guid.NewGuid())
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.Gender, f => f.PickRandom(genders))
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(u => u.Email, f => f.Internet.Email());

                var users = faker.Generate(10); // Generate 10 random users

                // Add the generated users to the context
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }
        }
    }
}
