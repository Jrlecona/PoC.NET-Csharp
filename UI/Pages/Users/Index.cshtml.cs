using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace UI.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }


        public IEnumerable<User> Users { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int? Age { get; set; }
        public string Country { get; set; }

        public async Task OnGetAsync(int pageNumber = 1, int? age = null, string? country = null)
        {
            Age = age;
            Country = country;
            PageNumber = pageNumber;
            try
            {
                var response = await _httpClient.GetAsync($"/api/users?pageNumber={pageNumber}&age={age}&country={country}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<UserApiResponse>(jsonResponse);

                    Users = result.Users;
                    TotalPages = result.TotalPages;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public class UserApiResponse
        {
            public IEnumerable<User> Users { get; set; }
            public int TotalPages { get; set; }
        }
    }
}