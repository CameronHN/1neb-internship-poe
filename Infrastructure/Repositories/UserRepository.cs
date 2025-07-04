using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Entities;
using Portfolio.Core.Exceptions;

namespace Portfolio.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAllUsers()
        {
            throw new NotFoundException("No users found");
        }

        public Task<User> GetUserById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
