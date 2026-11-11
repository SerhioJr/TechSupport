using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs;

/// <summary>Дані, які повертаються клієнту після операції із заявкою.</summary>
public sealed record TicketResponseDto(
    Guid Id,
    string Title,
    string Description,
    TicketCategory Category,
    TicketPriority Priority,
    TicketStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? AssigneeId,
    DateTime? ResolvedAt,
    DateTime DueDate,
    bool IsOverdue);
