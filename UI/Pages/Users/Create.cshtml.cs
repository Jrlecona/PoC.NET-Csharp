using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace UI.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        public List<UserDto> Users { get; set; } = new List<UserDto> { new UserDto() };

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Convert Users list to JSON
            var jsonContent = JsonConvert.SerializeObject(Users);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send POST request to the API
            var response = await _httpClient.PostAsync("api/users", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error creating user.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}