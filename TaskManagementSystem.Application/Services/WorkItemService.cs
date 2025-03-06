using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Validators;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Application.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<WorkItemService> _logger;
        private readonly CreateWorkItemValidator _createValidator;
        private readonly UpdateWorkItemValidator _updateValidator;

        public WorkItemService(
            IWorkItemRepository workItemRepository,
            IUserRepository userRepository,
            ILogger<WorkItemService> logger,
            CreateWorkItemValidator createValidator,
            UpdateWorkItemValidator updateValidator)
        {
            _workItemRepository = workItemRepository;
            _userRepository = userRepository;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<WorkItemDto>> GetAllWorkItemsAsync()
        {
            var workItems = await _workItemRepository.GetAllAsync();
            var workItemDtos = new List<WorkItemDto>();

            foreach (var workItem in workItems)
            {
                var dto = MapToDto(workItem);
                
                if (workItem.AssignedToUserId.HasValue)
                {
                    var user = await _userRepository.GetByIdAsync(workItem.AssignedToUserId.Value);
                    if (user != null)
                    {
                        dto.AssignedToUserName = user.Name;
                    }
                }
                
                workItemDtos.Add(dto);
            }

            return workItemDtos;
        }

        public async Task<WorkItemDto?> GetWorkItemByIdAsync(Guid id)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            if (workItem == null)
            {
                return null;
            }

            var dto = MapToDto(workItem);
            
            if (workItem.AssignedToUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(workItem.AssignedToUserId.Value);
                if (user != null)
                {
                    dto.AssignedToUserName = user.Name;
                }
            }
            
            return dto;
        }

        public async Task<IEnumerable<WorkItemDto>> GetWorkItemsByUserIdAsync(Guid userId)
        {
            var workItems = await _workItemRepository.GetByUserIdAsync(userId);
            var user = await _userRepository.GetByIdAsync(userId);
            
            return workItems.Select(workItem => 
            {
                var dto = MapToDto(workItem);
                if (user != null)
                {
                    dto.AssignedToUserName = user.Name;
                }
                return dto;
            });
        }

        public async Task<WorkItemDto> CreateWorkItemAsync(CreateWorkItemDto createWorkItemDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createWorkItemDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Invalid work item data: {errors}");
            }

            if (createWorkItemDto.AssignedToUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(createWorkItemDto.AssignedToUserId.Value);
                if (user == null)
                {
                    throw new ArgumentException($"User with ID {createWorkItemDto.AssignedToUserId} not found");
                }
            }

            var workItem = new WorkItem
            {
                Id = Guid.NewGuid(),
                Title = createWorkItemDto.Title,
                Description = createWorkItemDto.Description,
                DueDate = createWorkItemDto.DueDate,
                Priority = createWorkItemDto.Priority,
                Status = Status.ToDo,
                AssignedToUserId = createWorkItemDto.AssignedToUserId,
                CreatedAt = DateTime.UtcNow
            };

            var createdWorkItem = await _workItemRepository.AddAsync(workItem);
            _logger.LogInformation("Work item created: {Id}", createdWorkItem.Id);

            return MapToDto(createdWorkItem);
        }

        public async Task<WorkItemDto?> UpdateWorkItemAsync(Guid id, UpdateWorkItemDto updateWorkItemDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateWorkItemDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Invalid work item data: {errors}");
            }

            var existingWorkItem = await _workItemRepository.GetByIdAsync(id);
            if (existingWorkItem == null)
            {
                return null;
            }

            if (updateWorkItemDto.AssignedToUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(updateWorkItemDto.AssignedToUserId.Value);
                if (user == null)
                {
                    throw new ArgumentException($"User with ID {updateWorkItemDto.AssignedToUserId} not found");
                }
            }

            if (updateWorkItemDto.Title != null)
                existingWorkItem.Title = updateWorkItemDto.Title;
            
            if (updateWorkItemDto.Description != null)
                existingWorkItem.Description = updateWorkItemDto.Description;
            
            if (updateWorkItemDto.DueDate.HasValue)
                existingWorkItem.DueDate = updateWorkItemDto.DueDate.Value;
            
            if (updateWorkItemDto.Priority.HasValue)
                existingWorkItem.Priority = updateWorkItemDto.Priority.Value;
            
            if (updateWorkItemDto.Status.HasValue)
                existingWorkItem.Status = updateWorkItemDto.Status.Value;
            
            existingWorkItem.AssignedToUserId = updateWorkItemDto.AssignedToUserId ?? existingWorkItem.AssignedToUserId;
            existingWorkItem.UpdatedAt = DateTime.UtcNow;

            await _workItemRepository.UpdateAsync(existingWorkItem);
            _logger.LogInformation("Work item updated: {Id}", existingWorkItem.Id);

            var dto = MapToDto(existingWorkItem);
            
            if (existingWorkItem.AssignedToUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(existingWorkItem.AssignedToUserId.Value);
                if (user != null)
                {
                    dto.AssignedToUserName = user.Name;
                }
            }
            
            return dto;
        }

        public async Task<bool> DeleteWorkItemAsync(Guid id)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            if (workItem == null)
            {
                return false;
            }

            await _workItemRepository.DeleteAsync(id);
            _logger.LogInformation("Work item deleted: {Id}", id);
            
            return true;
        }

        private static WorkItemDto MapToDto(WorkItem workItem)
        {
            return new WorkItemDto
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Description = workItem.Description,
                DueDate = workItem.DueDate,
                Priority = workItem.Priority,
                Status = workItem.Status,
                AssignedToUserId = workItem.AssignedToUserId,
                CreatedAt = workItem.CreatedAt,
                UpdatedAt = workItem.UpdatedAt
            };
        }
    }
} 