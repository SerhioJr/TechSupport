using FluentValidation;
using SupportTicketSystem.Application.DTOs;

namespace SupportTicketSystem.Application.Validators;

/// <summary>Перевіряє вхідний запит на зміну статусу.</summary>
public sealed class UpdateTicketStatusDtoValidator : AbstractValidator<UpdateTicketStatusDto>
{
    public UpdateTicketStatusDtoValidator()
    {
        RuleFor(x => x.NewStatus).IsInEnum();
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}
