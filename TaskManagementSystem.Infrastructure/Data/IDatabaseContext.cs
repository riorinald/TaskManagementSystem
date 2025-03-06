using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Data
{
    public interface IDatabaseContext
    {
        Task<IEnumerable<WorkItem>> GetAllWorkItemsAsync();
        Task<WorkItem?> GetWorkItemByIdAsync(Guid id);
        Task<IEnumerable<WorkItem>> GetWorkItemsByUserIdAsync(Guid userId);
        Task<WorkItem> AddWorkItemAsync(WorkItem workItem);
        Task UpdateWorkItemAsync(WorkItem workItem);
        Task DeleteWorkItemAsync(Guid id);
        
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        
        Task InitializeDatabaseAsync();
    }
} 