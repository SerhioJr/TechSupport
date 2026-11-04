using TicketSystem.Domain.Enums;

namespace TicketSystem.Application.Tickets;

public class CreateTicketDto
{
    /// <summary>Коротка тема заявки.</summary>
    /// <example>Не працює принтер</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Детальний опис проблеми.</summary>
    /// <example>Принтер у кабінеті 12 не друкує після оновлення драйвера</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>Категорія заявки.</summary>
    public TicketCategory Category { get; set; }

    /// <summary>Бажаний пріоритет.</summary>
    public TicketPriority Priority { get; set; }
}

public class TicketResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketCategory Category { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}