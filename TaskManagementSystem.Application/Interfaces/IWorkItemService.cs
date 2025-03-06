using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces
{
    public interface IWorkItemService
    {
        Task<IEnumerable<WorkItemDto>> GetAllWorkItemsAsync();
        Task<WorkItemDto?> GetWorkItemByIdAsync(Guid id);
        Task<IEnumerable<WorkItemDto>> GetWorkItemsByUserIdAsync(Guid userId);
        Task<WorkItemDto> CreateWorkItemAsync(CreateWorkItemDto createWorkItemDto);
        Task<WorkItemDto?> UpdateWorkItemAsync(Guid id, UpdateWorkItemDto updateWorkItemDto);
        Task<bool> DeleteWorkItemAsync(Guid id);
    }
} 