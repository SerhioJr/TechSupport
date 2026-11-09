using FluentValidation;
using SupportTicketSystem.Application.DTOs;

namespace SupportTicketSystem.Application.Validators;

/// <summary>Перевіряє, що призначення містить коректний ідентифікатор фахівця.</summary>
public sealed class AssignUserDtoValidator : AbstractValidator<AssignUserDto>
{
    public AssignUserDtoValidator()
    {
        RuleFor(x => x.AssigneeId)
            .NotEmpty()
            .MaximumLength(100);
    }
}
