using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs.User;

namespace Portfolio.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserDetailsDTO> GetUserDetailsAsync(Guid id)
        {
            var userDetails = await _userRepository.GetUserDetailsByUserIdAsync(id);

            var user = new GetUserDetailsDTO
            {
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Email = userDetails.Email,
                PhoneNumber = userDetails.PhoneNumber,
            };

            return user;
        }
    }
}
