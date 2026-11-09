using FluentValidation;
using SupportTicketSystem.Application.DTOs;

namespace SupportTicketSystem.Application.Validators;

/// <summary>Перевіряє обов'язкові поля заявки та обмеження їх довжини і значень.</summary>
public sealed class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
