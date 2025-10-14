using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IUserService
    {
        Task<GetUserDetailsDTO> GetUserDetails(Guid id);
        Task AddUser(AddUserDTO dto);

        Task<List<string>> GetAllSkillsByUserId(Guid userId);

        Task<List<EducationItem>> GetAllEducationItemsByUserId(Guid userId);
    }
}
