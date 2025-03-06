using System;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs
{
    public class WorkItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 