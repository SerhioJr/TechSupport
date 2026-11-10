using FluentValidation;
using SupportTicketSystem.Application.DTOs;

namespace SupportTicketSystem.Application.Validators;

/// <summary>Перевіряє межі пагінації та значення параметрів пошуку.</summary>
public sealed class TicketQueryFilterValidator : AbstractValidator<TicketQueryFilter>
{
    public TicketQueryFilterValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
