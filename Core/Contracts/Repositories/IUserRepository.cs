using Portfolio.Core.DTOs.Resume;
using Portfolio.Core.Models;

namespace Portfolio.Core.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<GetAllResumeDetails?> GetAllResumeDetailsByUserIdAsync(Guid userId);

        Task<UserModel> GetUserDetailsByUserIdAsync(Guid id);
    }
}
