using Application.DTOs;
using Application.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DataSeeder _dataSeeder;

        public UsersController(UserService userService, DataSeeder dataSeeder)
        {
            _userService = userService;
            _dataSeeder = dataSeeder;
        }

        // GET: api/users?pageNumber=1&pageSize=10&age=30&country=USA
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? age = null, [FromQuery] string? country = null)
        {
            var result = await _userService.GetUsers(pageNumber, pageSize, age, country);
            return Ok(result);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> CreateUsers([FromBody] List<UserDto> users)
        {
            if (users.Count > 1000)
            {
                return BadRequest("Cannot create more than 1000 users at a time.");
            }

            await _userService.CreateUsers(users);
            return Ok();
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditDto userDto)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.UpdateUserAsync(id, userDto);
            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        // POST endpoint to create users from a CSV file located in the root directory
        [HttpPost("import-from-root")]
        public async Task<IActionResult> ImportUsersFromRoot()
        {
            // Define the file path relative to the root of the solution
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "users.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest("CSV file not found at the root of the solution.");
            }

            // Process CSV file and seed the database
            await _dataSeeder.SeedFromCsvFileAsync(filePath);

            return Ok("Users successfully imported from CSV located at the root.");
        }

        // POST: api/users/changepassword
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserByIdAsync(passwordModel.Id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.ChangePasswordAsync(passwordModel.Id, passwordModel.NewPassword);
            return Ok(new { Message = "Password updated successfully" });
        }
    }
}