using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Application.Mapping;

/// <summary>Перетворення коментарів між доменною моделлю та DTO.</summary>
public static class CommentMappings
{
    public static CommentResponseDto ToResponse(this TicketComment comment) => new(
        comment.Id,
        comment.AuthorName,
        comment.Message,
        comment.CreatedAt);
}
