namespace SupportTicketSystem.Application.DTOs;

/// <summary>Дані для створення коментаря до заявки.</summary>
public sealed record CreateCommentDto
{
    /// <summary>Ім'я автора коментаря.</summary>
    public string AuthorName { get; init; } = string.Empty;

    /// <summary>Текст коментаря.</summary>
    public string Message { get; init; } = string.Empty;
}
