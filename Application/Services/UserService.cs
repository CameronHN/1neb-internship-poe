using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;

namespace Portfolio.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserDetailsDTO> GetUserDetails(Guid id)
        {
            try
            {
                var userDetails = await _userRepository.GetUserById(id);

                var user = new GetUserDetailsDTO
                {
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email,
                    PhoneNumber = userDetails.PhoneNumber,
                    Gender = userDetails.Gender
                };

                return user;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
