using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddUsersAsync(IEnumerable<User> users);

        Task DeleteUserAsync(int id);

        Task<User> GetUserByIdAsync(int id);

        //Task<(IEnumerable<User> Users, int TotalPages)> GetUsersAsync(int pageNumber, int pageSize, int? age, string country);
        Task<UserResponseDto> GetUsersAsync(int pageNumber, int pageSize, int? age, string country);

        Task UpdateUserAsync(User user);

    }
}