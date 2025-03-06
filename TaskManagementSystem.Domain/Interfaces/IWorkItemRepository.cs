using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Interfaces
{
    public interface IWorkItemRepository
    {
        Task<IEnumerable<WorkItem>> GetAllAsync();
        Task<WorkItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<WorkItem>> GetByUserIdAsync(Guid userId);
        Task<WorkItem> AddAsync(WorkItem workItem);
        Task UpdateAsync(WorkItem workItem);
        Task DeleteAsync(Guid id);
    }
} 