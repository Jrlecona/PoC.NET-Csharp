var builder = WebApplication.CreateBuilder(args);

// Add HttpClient for API calls
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7049/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Bypass SSL verification for local development
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});

// Razor Pages setup
builder.Services.AddRazorPages();

var app = builder.Build();

// Redirect root URL to /Users
app.MapGet("/", () => Results.Redirect("/Users"));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();