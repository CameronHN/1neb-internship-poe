using Portfolio.Core.Contracts.Repositories;
using Portfolio.Core.Contracts.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.Entities;

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

        public async Task AddUser(AddUserDTO userDto)
        {
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Gender = userDto.Gender,
                PhoneNumber = userDto.PhoneNumber
            };

            await _userRepository.AddUser(user);
        }

        public async Task UpdateUser(UpdateUserDTO updateUserDTO)
        {
            await _userRepository.UpdateUser(updateUserDTO);
        }

        public async Task DeleteUser(Guid id)
        {
            await _userRepository.DeleteUser(id);
        }
    }
}
