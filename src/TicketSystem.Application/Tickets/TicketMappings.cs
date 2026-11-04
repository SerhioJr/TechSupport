using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets;

public static class TicketMappings
{
    public static Ticket ToEntity(this CreateTicketDto dto) =>
        new(dto.Title, dto.Description, dto.Category, dto.Priority);

    public static TicketResponseDto ToResponseDto(this Ticket t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Category = t.Category,
        Priority = t.Priority,
        Status = t.Status,
        CreatedAt = t.CreatedAt
    };
}