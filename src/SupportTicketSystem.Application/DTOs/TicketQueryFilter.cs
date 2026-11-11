using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs;

/// <summary>Параметри фільтрації та пагінації списку заявок.</summary>
public sealed record TicketQueryFilter
{
    /// <summary>Категорія заявок для відбору.</summary>
    public TicketCategory? Category { get; init; }

    /// <summary>Пріоритет заявок для відбору.</summary>
    public TicketPriority? Priority { get; init; }

    /// <summary>Статус заявок для відбору.</summary>
    public TicketStatus? Status { get; init; }

    /// <summary>Ідентифікатор призначеного фахівця.</summary>
    public string? AssigneeId { get; init; }

    /// <summary>Пошукове слово в назві або описі заявки.</summary>
    public string? SearchTerm { get; init; }

    /// <summary>Номер сторінки, починаючи з 1.</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>Кількість заявок на сторінці (від 1 до 50).</summary>
    public int PageSize { get; init; } = 10;
}
