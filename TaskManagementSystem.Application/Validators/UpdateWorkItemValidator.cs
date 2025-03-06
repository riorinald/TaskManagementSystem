using FluentValidation;
using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Validators
{
    public class UpdateWorkItemValidator : AbstractValidator<UpdateWorkItemDto>
    {
        public UpdateWorkItemValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters")
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
                .When(x => x.Description != null);

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Due date cannot be in the past")
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value")
                .When(x => x.Priority.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value")
                .When(x => x.Status.HasValue);
        }
    }
} 