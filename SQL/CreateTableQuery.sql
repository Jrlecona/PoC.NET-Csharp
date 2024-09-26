CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Age INT,
    DateOfBirth DATETIME,
    Country NVARCHAR(50),
    Province NVARCHAR(50),
    City NVARCHAR(50),
    PasswordHash VARBINARY(64),  -- If you're storing passwords
    PasswordSalt VARBINARY(128)   -- Salt for password security
);

-- Add indexes to optimize queries, especially on columns that will be frequently filtered.
CREATE INDEX idx_Users_Age ON Users (Age);
CREATE INDEX idx_Users_Country ON Users (Country);
