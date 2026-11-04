using FluentValidation;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets;

public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketDtoValidator()
    {
        RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Тема є обов'язковою.")
            .Length(Ticket.TitleMinLength, Ticket.TitleMaxLength)
            .WithMessage("Тема має містити від 5 до 100 символів.");

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Опис є обов'язковим.")
            .MaximumLength(Ticket.DescriptionMaxLength)
            .WithMessage("Опис не може перевищувати 2000 символів.");

        RuleFor(x => x.Category).IsInEnum().WithMessage("Невідома категорія.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("Невідомий пріоритет.");
    }
}