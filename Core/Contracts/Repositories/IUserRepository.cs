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

        Task<List<string>> GetAllSkillsByUserId(Guid userId);

        Task<List<EducationItem>> GetAllEducationItemsByUserId(Guid userId);

        Task<UserEntityDetailsDto> GetUserEntityDetailsByUserId(Guid id);
    }
}
