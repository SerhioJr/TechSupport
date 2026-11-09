using System.ComponentModel.DataAnnotations;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs;

/// <summary>Дані, які клієнт передає під час створення заявки.</summary>
public sealed record CreateTicketDto
{
    /// <summary>Коротка назва заявки.</summary>
    [Required]
    public string Title { get; init; } = string.Empty;

    /// <summary>Детальний опис проблеми.</summary>
    [Required]
    public string Description { get; init; } = string.Empty;

    /// <summary>Категорія, до якої належить заявка.</summary>
    public TicketCategory Category { get; init; }

    /// <summary>Бажаний пріоритет заявки.</summary>
    public TicketPriority Priority { get; init; }
}
