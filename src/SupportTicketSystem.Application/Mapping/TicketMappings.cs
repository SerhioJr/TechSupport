using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Mapping;

/// <summary>Містить явні перетворення між доменними сутностями та API DTO.</summary>
public static class TicketMappings
{
    public static TicketResponseDto ToResponse(this Ticket ticket, DateTime currentTimeUtc)
    {
        var dueDate = ticket.CreatedAt.Add(ticket.Priority switch
        {
            TicketPriority.High => TimeSpan.FromHours(4),
            TicketPriority.Medium => TimeSpan.FromHours(24),
            TicketPriority.Low => TimeSpan.FromHours(72),
            _ => throw new ArgumentOutOfRangeException(nameof(ticket), "Невідомий пріоритет заявки.")
        });
        var isCompleted = ticket.Status is TicketStatus.Resolved or TicketStatus.Closed;

        return new TicketResponseDto(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Category,
            ticket.Priority,
            ticket.Status,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            ticket.AssigneeId,
            ticket.ResolvedAt,
            dueDate,
            !isCompleted && currentTimeUtc > dueDate);
    }
}
