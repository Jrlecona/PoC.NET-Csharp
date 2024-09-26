using Application.Interfaces;
using Application.Services;
using Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DataSeeder>();

// Add SQL connection string
builder.Services.AddSingleton(sp => builder.Configuration.GetConnectionString("DefaultConnection"));

// Configure Swagger (if needed)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();