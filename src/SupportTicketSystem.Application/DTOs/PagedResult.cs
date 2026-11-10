namespace SupportTicketSystem.Application.DTOs;

/// <summary>Сторінка результатів разом із метаданими пагінації.</summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
