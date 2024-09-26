using Domain.Entities;

namespace Application.DTOs
{
    public class UserResponseDto
    {
        public UserResponseDto(List<User> users, int totalPages)
        {
            Users = users;
            TotalPages = totalPages;
        }

        public IEnumerable<User> Users { get; set; }
        public int TotalPages { get; set; }
    }
}