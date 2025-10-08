using Portfolio.Core.DTOs;
using Portfolio.Core.Entities;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(Guid id);

        Task AddUser(User user);

        Task UpdateUser(UpdateUserDTO updateUserDTO);

        Task DeleteUser(Guid id);

        Task<ResumeDto?> GetResumeDtoByUserId(Guid userId);
    }
}
