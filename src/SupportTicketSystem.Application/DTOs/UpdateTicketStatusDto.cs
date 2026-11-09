using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs;

/// <summary>Тіло запиту для зміни статусу заявки.</summary>
public sealed record UpdateTicketStatusDto
{
    /// <summary>Новий статус, який потрібно встановити.</summary>
    public TicketStatus NewStatus { get; init; }

    /// <summary>Необов'язковий коментар із поясненням зміни статусу.</summary>
    public string? Comment { get; init; }
}
