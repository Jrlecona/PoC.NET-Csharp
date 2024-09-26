using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //public async Task<(IEnumerable<User> Users, int TotalPages)> GetUsers(int pageNumber, int pageSize, int? age, string country)
        public async Task<UserResponseDto> GetUsers(int pageNumber, int pageSize, int? age, string country)
        {
            return await _userRepository.GetUsersAsync(pageNumber, pageSize, age, country);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task CreateUsers(IEnumerable<UserDto> users)
        {
            var userEntities = new List<User>();

            foreach (var dto in users)
            {
                byte[] passwordHash, passwordSalt;
                PasswordHasher.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);

                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    DateOfBirth = dto.DateOfBirth,
                    Country = dto.Country,
                    Province = dto.Province,
                    City = dto.City,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };

                userEntities.Add(user);
            }

            await _userRepository.AddUsersAsync(userEntities);
        }

        public async Task UpdateUserAsync(int id, UserEditDto userDto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Age = userDto.Age;
                user.DateOfBirth = userDto.DateOfBirth;
                user.Country = userDto.Country;
                user.Province = userDto.Province;
                user.City = userDto.City;

                await _userRepository.UpdateUserAsync(user);
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task ChangePasswordAsync(int id, string newPassword)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user != null)
            {
                byte[] passwordHash, passwordSalt;
                PasswordHasher.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepository.UpdateUserAsync(user);
            }
        }
    }
}