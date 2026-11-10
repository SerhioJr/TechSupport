namespace SupportTicketSystem.Application.DTOs;

/// <summary>Параметри пагінації та пошуку заявок.</summary>
public sealed record TicketQueryFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
