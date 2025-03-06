using System;
using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new work item
    /// </summary>
    public class CreateWorkItemDto
    {
        /// <summary>
        /// The title of the work item
        /// </summary>
        /// <example>Implement login functionality</example>
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The description of the work item
        /// </summary>
        /// <example>Create a login form with email and password fields</example>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The due date of the work item
        /// </summary>
        /// <example>2023-12-31</example>
        [Required]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The priority of the work item
        /// </summary>
        /// <example>Medium</example>
        [Required]
        public Priority Priority { get; set; }

        /// <summary>
        /// The ID of the user assigned to the work item
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid? AssignedToUserId { get; set; }
    }
} 