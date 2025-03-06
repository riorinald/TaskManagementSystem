using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public WorkItemRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<WorkItem>> GetAllAsync()
        {
            return await _databaseContext.GetAllWorkItemsAsync();
        }

        public async Task<WorkItem?> GetByIdAsync(Guid id)
        {
            return await _databaseContext.GetWorkItemByIdAsync(id);
        }

        public async Task<IEnumerable<WorkItem>> GetByUserIdAsync(Guid userId)
        {
            return await _databaseContext.GetWorkItemsByUserIdAsync(userId);
        }

        public async Task<WorkItem> AddAsync(WorkItem workItem)
        {
            return await _databaseContext.AddWorkItemAsync(workItem);
        }

        public async Task UpdateAsync(WorkItem workItem)
        {
            await _databaseContext.UpdateWorkItemAsync(workItem);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _databaseContext.DeleteWorkItemAsync(id);
        }
    }
} 