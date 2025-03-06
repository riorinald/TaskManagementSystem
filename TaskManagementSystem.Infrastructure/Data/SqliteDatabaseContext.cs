using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Infrastructure.Data
{
    public class SqliteDatabaseContext : IDatabaseContext
    {
        private readonly string _connectionString;
        private readonly ILogger<SqliteDatabaseContext> _logger;

        public SqliteDatabaseContext(IConfiguration configuration, ILogger<SqliteDatabaseContext> logger)
        {
            _connectionString = configuration.GetConnectionString("SqliteConnection") 
                ?? "Data Source=taskmanagement.db";
            _logger = logger;
        }

        public async Task InitializeDatabaseAsync()
        {
            _logger.LogInformation("Initializing SQLite database");
            
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Create Users table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id TEXT PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Email TEXT NOT NULL
                    )";
                await command.ExecuteNonQueryAsync();
            }

            // Create WorkItems table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS WorkItems (
                        Id TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        DueDate TEXT NOT NULL,
                        Priority INTEGER NOT NULL,
                        Status INTEGER NOT NULL,
                        AssignedToUserId TEXT,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT,
                        FOREIGN KEY (AssignedToUserId) REFERENCES Users (Id)
                    )";
                await command.ExecuteNonQueryAsync();
            }
            
            // Create indexes for better performance
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE INDEX IF NOT EXISTS idx_workitems_assignedtouser ON WorkItems(AssignedToUserId);
                    CREATE INDEX IF NOT EXISTS idx_workitems_status ON WorkItems(Status);
                    CREATE INDEX IF NOT EXISTS idx_workitems_priority ON WorkItems(Priority);
                    CREATE INDEX IF NOT EXISTS idx_workitems_duedate ON WorkItems(DueDate);
                ";
                await command.ExecuteNonQueryAsync();
            }
            
            // Add mock users if they don't exist
            await AddMockUsersIfNotExistAsync(connection);
            
            // Add mock work items if they don't exist
            await AddMockWorkItemsIfNotExistAsync(connection);
            
            _logger.LogInformation("SQLite database initialized successfully");
        }

        private async Task AddMockUsersIfNotExistAsync(SqliteConnection connection)
        {
            // Check if users already exist
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Users";
                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                
                if (count > 0)
                {
                    _logger.LogInformation("Mock users already exist, skipping creation");
                    return;
                }
            }
            
            // Create mock users
            var mockUsers = new List<(Guid Id, string Name, string Email)>
            {
                (Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"), "User One", "user1@example.com"),
                (Guid.Parse("12345678-89ab-cdef-0123-456789abcdef"), "User Two", "user2@example.com"),
                (Guid.Parse("23456789-89ab-cdef-0123-456789abcdef"), "User Three", "user3@example.com"),
                (Guid.Parse("34567890-89ab-cdef-0123-456789abcdef"), "User Four", "user4@example.com"),
                (Guid.Parse("45678901-89ab-cdef-0123-456789abcdef"), "User Five", "user5@example.com")
            };
            
            foreach (var user in mockUsers)
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Users (Id, Name, Email)
                    VALUES (@Id, @Name, @Email)";
                
                command.Parameters.AddWithValue("@Id", user.Id.ToString());
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                
                await command.ExecuteNonQueryAsync();
            }
            
            _logger.LogInformation("Added 5 mock users to the database");
        }

        private async Task AddMockWorkItemsIfNotExistAsync(SqliteConnection connection)
        {
            // Check if work items already exist
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM WorkItems";
                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                
                if (count > 0)
                {
                    _logger.LogInformation("Mock work items already exist, skipping creation");
                    return;
                }
            }
            
            // Get user IDs
            var userIds = new List<string>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id FROM Users";
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userIds.Add(reader["Id"].ToString());
                }
            }
            
            if (userIds.Count == 0)
            {
                _logger.LogWarning("No users found, cannot create mock work items");
                return;
            }
            
            // Create mock work items
            var mockWorkItems = new List<(string Title, string Description, DateTime DueDate, Priority Priority, Status Status, string AssignedToUserId)>
            {
                // User 1 work items
                ("Implement login page", "Create a login page with email and password fields", DateTime.Now.AddDays(5), Priority.High, Status.InProgress, userIds[0]),
                ("Design database schema", "Create the database schema for the application", DateTime.Now.AddDays(3), Priority.Medium, Status.Done, userIds[0]),
                
                // User 2 work items
                ("Fix navigation bug", "Fix the navigation bug in the mobile app", DateTime.Now.AddDays(2), Priority.Critical, Status.ToDo, userIds[1]),
                ("Update documentation", "Update the API documentation", DateTime.Now.AddDays(7), Priority.Low, Status.InProgress, userIds[1]),
                
                // User 3 work items
                ("Implement payment gateway", "Integrate with Stripe for payments", DateTime.Now.AddDays(10), Priority.High, Status.ToDo, userIds[2]),
                ("Create user profile page", "Design and implement the user profile page", DateTime.Now.AddDays(6), Priority.Medium, Status.InProgress, userIds[2]),
                
                // User 4 work items
                ("Optimize database queries", "Improve performance of database queries", DateTime.Now.AddDays(4), Priority.Medium, Status.ToDo, userIds[3]),
                ("Add unit tests", "Write unit tests for the core functionality", DateTime.Now.AddDays(8), Priority.Low, Status.Done, userIds[3]),
                
                // User 5 work items
                ("Deploy to production", "Deploy the application to production servers", DateTime.Now.AddDays(1), Priority.Critical, Status.InProgress, userIds[4]),
                ("Conduct security audit", "Perform a security audit of the application", DateTime.Now.AddDays(9), Priority.High, Status.ToDo, userIds[4])
            };
            
            foreach (var workItem in mockWorkItems)
            {
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO WorkItems (Id, Title, Description, DueDate, Priority, Status, AssignedToUserId, CreatedAt)
                    VALUES (@Id, @Title, @Description, @DueDate, @Priority, @Status, @AssignedToUserId, @CreatedAt)";
                
                var id = Guid.NewGuid();
                var createdAt = DateTime.UtcNow;
                
                command.Parameters.AddWithValue("@Id", id.ToString());
                command.Parameters.AddWithValue("@Title", workItem.Title);
                command.Parameters.AddWithValue("@Description", workItem.Description);
                command.Parameters.AddWithValue("@DueDate", workItem.DueDate.ToString("o"));
                command.Parameters.AddWithValue("@Priority", (int)workItem.Priority);
                command.Parameters.AddWithValue("@Status", (int)workItem.Status);
                command.Parameters.AddWithValue("@AssignedToUserId", workItem.AssignedToUserId);
                command.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("o"));
                
                await command.ExecuteNonQueryAsync();
            }
            
            _logger.LogInformation("Added 10 mock work items to the database");
        }

        public async Task<IEnumerable<WorkItem>> GetAllWorkItemsAsync()
        {
            var workItems = new List<WorkItem>();
            
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM WorkItems";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                workItems.Add(ReadWorkItem(reader));
            }
            
            return workItems;
        }

        public async Task<WorkItem?> GetWorkItemByIdAsync(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM WorkItems WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id.ToString());
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadWorkItem(reader);
            }
            
            return null;
        }

        public async Task<IEnumerable<WorkItem>> GetWorkItemsByUserIdAsync(Guid userId)
        {
            var workItems = new List<WorkItem>();
            
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM WorkItems WHERE AssignedToUserId = @UserId";
            command.Parameters.AddWithValue("@UserId", userId.ToString());
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                workItems.Add(ReadWorkItem(reader));
            }
            
            return workItems;
        }

        public async Task<WorkItem> AddWorkItemAsync(WorkItem workItem)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO WorkItems (Id, Title, Description, DueDate, Priority, Status, AssignedToUserId, CreatedAt, UpdatedAt)
                VALUES (@Id, @Title, @Description, @DueDate, @Priority, @Status, @AssignedToUserId, @CreatedAt, @UpdatedAt)";
            
            command.Parameters.AddWithValue("@Id", workItem.Id.ToString());
            command.Parameters.AddWithValue("@Title", workItem.Title);
            command.Parameters.AddWithValue("@Description", workItem.Description);
            command.Parameters.AddWithValue("@DueDate", workItem.DueDate.ToString("o"));
            command.Parameters.AddWithValue("@Priority", (int)workItem.Priority);
            command.Parameters.AddWithValue("@Status", (int)workItem.Status);
            command.Parameters.AddWithValue("@AssignedToUserId", workItem.AssignedToUserId?.ToString() ?? DBNull.Value as object);
            command.Parameters.AddWithValue("@CreatedAt", workItem.CreatedAt.ToString("o"));
            command.Parameters.AddWithValue("@UpdatedAt", workItem.UpdatedAt?.ToString("o") ?? DBNull.Value as object);
            
            await command.ExecuteNonQueryAsync();
            
            return workItem;
        }

        public async Task UpdateWorkItemAsync(WorkItem workItem)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE WorkItems 
                SET Title = @Title, 
                    Description = @Description, 
                    DueDate = @DueDate, 
                    Priority = @Priority, 
                    Status = @Status, 
                    AssignedToUserId = @AssignedToUserId, 
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            command.Parameters.AddWithValue("@Id", workItem.Id.ToString());
            command.Parameters.AddWithValue("@Title", workItem.Title);
            command.Parameters.AddWithValue("@Description", workItem.Description);
            command.Parameters.AddWithValue("@DueDate", workItem.DueDate.ToString("o"));
            command.Parameters.AddWithValue("@Priority", (int)workItem.Priority);
            command.Parameters.AddWithValue("@Status", (int)workItem.Status);
            command.Parameters.AddWithValue("@AssignedToUserId", workItem.AssignedToUserId?.ToString() ?? DBNull.Value as object);
            command.Parameters.AddWithValue("@UpdatedAt", workItem.UpdatedAt?.ToString("o") ?? DBNull.Value as object);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteWorkItemAsync(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM WorkItems WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id.ToString());
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(ReadUser(reader));
            }
            
            return users;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id.ToString());
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadUser(reader);
            }
            
            return null;
        }

        public async Task<User> AddUserAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Id, Name, Email)
                VALUES (@Id, @Name, @Email)";
            
            command.Parameters.AddWithValue("@Id", user.Id.ToString());
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            
            await command.ExecuteNonQueryAsync();
            
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users 
                SET Name = @Name, 
                    Email = @Email
                WHERE Id = @Id";
            
            command.Parameters.AddWithValue("@Id", user.Id.ToString());
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id.ToString());
            
            await command.ExecuteNonQueryAsync();
        }

        private static WorkItem ReadWorkItem(SqliteDataReader reader)
        {
            var workItem = new WorkItem
            {
                Id = Guid.Parse(reader["Id"].ToString()!),
                Title = reader["Title"].ToString()!,
                Description = reader["Description"].ToString() ?? string.Empty,
                DueDate = DateTime.Parse(reader["DueDate"].ToString()!),
                Priority = (Priority)Convert.ToInt32(reader["Priority"]),
                Status = (Status)Convert.ToInt32(reader["Status"]),
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()!)
            };

            if (reader["AssignedToUserId"] != DBNull.Value)
            {
                workItem.AssignedToUserId = Guid.Parse(reader["AssignedToUserId"].ToString()!);
            }

            if (reader["UpdatedAt"] != DBNull.Value)
            {
                workItem.UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()!);
            }

            return workItem;
        }

        private static User ReadUser(SqliteDataReader reader)
        {
            return new User
            {
                Id = Guid.Parse(reader["Id"].ToString()!),
                Name = reader["Name"].ToString()!,
                Email = reader["Email"].ToString()!
            };
        }
    }
} 