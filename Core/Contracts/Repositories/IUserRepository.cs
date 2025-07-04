using Portfolio.Core.Entities;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();

        Task<User> CreateUser(User user);

        Task<User> GetUserById(int Id);

        Task<User> UpdateUser(User user);

        Task<bool> DeleteUserById(int Id);
    }
}
