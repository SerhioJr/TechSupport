using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Application.Mapping;

/// <summary>Містить явні перетворення між доменними сутностями та API DTO.</summary>
public static class TicketMappings
{
    public static TicketResponseDto ToResponse(this Ticket ticket) => new(
        ticket.Id,
        ticket.Title,
        ticket.Description,
        ticket.Category,
        ticket.Priority,
        ticket.Status,
        ticket.CreatedAt,
        ticket.UpdatedAt,
        ticket.AssigneeId,
        ticket.ResolvedAt);
}
