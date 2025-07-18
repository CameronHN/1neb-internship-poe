using Microsoft.Extensions.Logging;
using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<User> _logger;

        public UserRepository(ApplicationDbContext dbContext, ILogger<User> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _dbContext.User.FindAsync(id);
        }
    }
}
