using Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace UI.Pages.Users
{
    public class ChangePasswordModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ChangePasswordModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        public ChangePasswordDto PasswordModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            PasswordModel = new ChangePasswordDto { Id = id };

            // Call API to verify user exists
            var response = await _httpClient.GetAsync($"api/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Prepare the ChangePasswordDto as JSON
            var jsonContent = JsonConvert.SerializeObject(PasswordModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send POST request to the API
            var response = await _httpClient.PostAsync("api/users/changepassword", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error changing password");
                return Page();
            }

            // If successful, redirect to the Index page
            return RedirectToPage("./Index");
        }
    }
}