using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetUserById(Guid id)
        {
            User? user = await _dbContext.User.FindAsync(id); ;

            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            return user;
        }
    }
}
