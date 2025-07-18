using Portfolio.Core.Entities;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IUserRepository
    {

        Task<User> GetUserById(Guid id);
    }
}
