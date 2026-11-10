using FluentValidation;
using SupportTicketSystem.Application.DTOs;

namespace SupportTicketSystem.Application.Validators;

/// <summary>Перевіряє дані нового коментаря.</summary>
public sealed class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.AuthorName)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Ім'я автора не може бути порожнім.")
            .MaximumLength(100);

        RuleFor(x => x.Message)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Текст коментаря не може бути порожнім.")
            .Length(2, 1000)
            .WithMessage("Текст коментаря має містити від 2 до 1000 символів.");
    }
}
