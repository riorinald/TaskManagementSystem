using FluentValidation;
using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Validators
{
    public class CreateWorkItemValidator : AbstractValidator<CreateWorkItemDto>
    {
        public CreateWorkItemValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Due date cannot be in the past");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Invalid priority value");
        }
    }
} 