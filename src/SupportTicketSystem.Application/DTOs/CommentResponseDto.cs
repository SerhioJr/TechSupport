namespace SupportTicketSystem.Application.DTOs;

/// <summary>Дані коментаря, які повертаються клієнту.</summary>
public sealed record CommentResponseDto(
    Guid Id,
    string AuthorName,
    string Message,
    DateTime CreatedAt);
