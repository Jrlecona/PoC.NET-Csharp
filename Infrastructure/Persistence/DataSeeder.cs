using Application.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Entities;
using System.Globalization;

namespace Infrastructure.Persistence
{
    public class DataSeeder
    {
        private readonly IUserRepository _userRepository;

        public DataSeeder(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Method to process the CSV file from the root of the solution and seed the database
        public async Task SeedFromCsvFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var users = new List<User>();

            using (var stream = new StreamReader(filePath))
            {
                var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreReferences = true,
                    TrimOptions = TrimOptions.Trim,
                    BadDataFound = null
                };

                using (var csvReader = new CsvReader(stream, csvConfiguration))
                {
                    // Read the CSV file, skip the first column, and map to User objects
                    var records = csvReader.GetRecords<dynamic>().ToList();

                    foreach (var record in records)
                    {
                        var values = ((IDictionary<string, object>)record).Values.ToList();

                        // Assuming first column is to be ignored, hence starting from index 1
                        var user = new User
                        {
                            FirstName = values[1]?.ToString(),
                            LastName = values[2]?.ToString(),
                            Age = int.Parse(values[4]?.ToString()),
                            DateOfBirth = DateTime.Parse(values[3]?.ToString()),
                            Country = values[5]?.ToString(),
                            Province = values[6]?.ToString(),
                            City = values[7]?.ToString()
                        };

                        users.Add(user);
                    }
                }
            }

            // Insert the users into the database in batches of 1000
            var userBatches = users.Take(1000).ToList();
            if (userBatches.Count > 0)
            {
                await _userRepository.AddUsersAsync(userBatches);
            }
        }
    }
}