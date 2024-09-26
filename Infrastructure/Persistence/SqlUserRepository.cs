using Application.DTOs;
using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Persistence
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public SqlUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserResponseDto> GetUsersAsync(int pageNumber, int pageSize, int? age, string country)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var parameters = new { Age = age, Country = country, Offset = (pageNumber - 1) * pageSize, PageSize = pageSize };

                var query = @"SELECT * FROM Users
                              WHERE (@Age IS NULL OR Age = @Age)
                              AND (@Country IS NULL OR Country = @Country)
                              ORDER BY Id
                              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                              SELECT COUNT(*) FROM Users WHERE (@Age IS NULL OR Age = @Age) AND (@Country IS NULL OR Country = @Country);";

                using var multi = await connection.QueryMultipleAsync(query, parameters);
                var users = (await multi.ReadAsync<User>()).ToList();
                var totalCount = await multi.ReadSingleAsync<int>();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return new UserResponseDto(users, totalPages);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as necessary
                Console.WriteLine($"An error occurred while fetching users: {ex.Message}");
                throw;  // Re-throw the exception to be handled by the service layer or higher
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var query = "SELECT * FROM Users WHERE Id = @Id";
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"An error occurred while fetching the user with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task AddUsersAsync(IEnumerable<User> users)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                var query = @"INSERT INTO Users (FirstName, LastName, Age, DateOfBirth, Country, Province, City, PasswordHash, PasswordSalt)
                              VALUES (@FirstName, @LastName, @Age, @DateOfBirth, @Country, @Province, @City, @PasswordHash, @PasswordSalt)";

                await connection.ExecuteAsync(query, users, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Rollback transaction if there was an error
                Console.WriteLine($"An error occurred while adding users: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var query = @"UPDATE Users
                              SET FirstName = @FirstName, LastName = @LastName, Age = @Age, DateOfBirth = @DateOfBirth,
                                  Country = @Country, Province = @Province, City = @City
                              WHERE Id = @Id";
                await connection.ExecuteAsync(query, user);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"An error occurred while updating the user with ID {user.Id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var query = "DELETE FROM Users WHERE Id = @Id";
                await connection.ExecuteAsync(query, new { Id = id });
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"An error occurred while deleting the user with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}
