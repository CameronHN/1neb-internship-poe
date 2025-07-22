using Portfolio.Core.DTOs;

namespace Portfolio.Core.Contracts.Services
{
    public interface IUserService
    {
        Task<GetUserDetailsDTO> GetUserDetails(Guid id);
    }
}
