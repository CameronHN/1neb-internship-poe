using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.DTOs;
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
            User? user = await _dbContext.User.FindAsync(id);

            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            return user;
        }

        public async Task AddUser(User user)
        {
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var existingUser = await _dbContext.User.FindAsync(updateUserDTO.UserId);
            if (existingUser == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            existingUser.Gender = updateUserDTO.Gender;
            existingUser.PhoneNumber = updateUserDTO.PhoneNumber;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _dbContext.User.FindAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User does not exist.");
            }

            _dbContext.User.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
