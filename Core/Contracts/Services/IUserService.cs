using Portfolio.Core.DTOs.User;

namespace Portfolio.Core.Contracts.Services
{
    public interface IUserService
    {
        Task<GetUserDetailsDTO> GetUserDetailsAsync(Guid id);
    }
}
