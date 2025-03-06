using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Data
{
    public class JsonDatabaseContext : IDatabaseContext
    {
        private readonly string _workItemsFilePath;
        private readonly string _usersFilePath;
        private readonly ILogger<JsonDatabaseContext> _logger;
        private readonly object _fileLock = new();

        public JsonDatabaseContext(IConfiguration configuration, ILogger<JsonDatabaseContext> logger)
        {
            var dataDirectory = configuration["JsonDatabase:Directory"] ?? "Data";
            
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            
            _workItemsFilePath = Path.Combine(dataDirectory, "workitems.json");
            _usersFilePath = Path.Combine(dataDirectory, "users.json");
            _logger = logger;
        }

        public Task InitializeDatabaseAsync()
        {
            _logger.LogInformation("Initializing JSON database");
            
            if (!File.Exists(_workItemsFilePath))
            {
                File.WriteAllText(_workItemsFilePath, JsonConvert.SerializeObject(new List<WorkItem>()));
            }
            
            if (!File.Exists(_usersFilePath))
            {
                File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(new List<User>()));
            }
            
            _logger.LogInformation("JSON database initialized successfully");
            
            return Task.CompletedTask;
        }

        public Task<IEnumerable<WorkItem>> GetAllWorkItemsAsync()
        {
            var workItems = ReadWorkItems();
            return Task.FromResult<IEnumerable<WorkItem>>(workItems);
        }

        public Task<WorkItem?> GetWorkItemByIdAsync(Guid id)
        {
            var workItems = ReadWorkItems();
            var workItem = workItems.FirstOrDefault(w => w.Id == id);
            return Task.FromResult(workItem);
        }

        public Task<IEnumerable<WorkItem>> GetWorkItemsByUserIdAsync(Guid userId)
        {
            var workItems = ReadWorkItems();
            var filteredWorkItems = workItems.Where(w => w.AssignedToUserId == userId);
            return Task.FromResult(filteredWorkItems);
        }

        public Task<WorkItem> AddWorkItemAsync(WorkItem workItem)
        {
            var workItems = ReadWorkItems().ToList();
            workItems.Add(workItem);
            SaveWorkItems(workItems);
            return Task.FromResult(workItem);
        }

        public Task UpdateWorkItemAsync(WorkItem workItem)
        {
            var workItems = ReadWorkItems().ToList();
            var index = workItems.FindIndex(w => w.Id == workItem.Id);
            
            if (index != -1)
            {
                workItems[index] = workItem;
                SaveWorkItems(workItems);
            }
            
            return Task.CompletedTask;
        }

        public Task DeleteWorkItemAsync(Guid id)
        {
            var workItems = ReadWorkItems().ToList();
            var workItem = workItems.FirstOrDefault(w => w.Id == id);
            
            if (workItem != null)
            {
                workItems.Remove(workItem);
                SaveWorkItems(workItems);
            }
            
            return Task.CompletedTask;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = ReadUsers();
            return Task.FromResult<IEnumerable<User>>(users);
        }

        public Task<User?> GetUserByIdAsync(Guid id)
        {
            var users = ReadUsers();
            var user = users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User> AddUserAsync(User user)
        {
            var users = ReadUsers().ToList();
            users.Add(user);
            SaveUsers(users);
            return Task.FromResult(user);
        }

        public Task UpdateUserAsync(User user)
        {
            var users = ReadUsers().ToList();
            var index = users.FindIndex(u => u.Id == user.Id);
            
            if (index != -1)
            {
                users[index] = user;
                SaveUsers(users);
            }
            
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(Guid id)
        {
            var users = ReadUsers().ToList();
            var user = users.FirstOrDefault(u => u.Id == id);
            
            if (user != null)
            {
                users.Remove(user);
                SaveUsers(users);
            }
            
            return Task.CompletedTask;
        }

        private IEnumerable<WorkItem> ReadWorkItems()
        {
            lock (_fileLock)
            {
                if (!File.Exists(_workItemsFilePath))
                {
                    return new List<WorkItem>();
                }
                
                var json = File.ReadAllText(_workItemsFilePath);
                return JsonConvert.DeserializeObject<List<WorkItem>>(json) ?? new List<WorkItem>();
            }
        }

        private void SaveWorkItems(IEnumerable<WorkItem> workItems)
        {
            lock (_fileLock)
            {
                var json = JsonConvert.SerializeObject(workItems, Formatting.Indented);
                File.WriteAllText(_workItemsFilePath, json);
            }
        }

        private IEnumerable<User> ReadUsers()
        {
            lock (_fileLock)
            {
                if (!File.Exists(_usersFilePath))
                {
                    return new List<User>();
                }
                
                var json = File.ReadAllText(_usersFilePath);
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
        }

        private void SaveUsers(IEnumerable<User> users)
        {
            lock (_fileLock)
            {
                var json = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(_usersFilePath, json);
            }
        }
    }
} 