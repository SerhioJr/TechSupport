using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Exceptions;

namespace SupportTicketSystem.Domain.Entities;

/// <summary>
/// Основна доменна сутність, що представляє заявку технічної підтримки.
/// Сутність самостійно керує змінами свого стану, тому некоректні переходи
/// статусів неможливо виконати з рівня Application або API.
/// </summary>
public class Ticket
{
    // EF Core використовує приватний конструктор під час завантаження запису з бази.
    private Ticket()
    {
    }

    public Ticket(string title, string description, TicketCategory category, TicketPriority priority)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Category = category;
        Priority = priority;
        Status = TicketStatus.New;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TicketCategory Category { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? AssigneeId { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    /// <summary>
    /// Призначає заявку фахівцю. Нова заявка автоматично переходить
    /// у статус InProgress після призначення першого фахівця.
    /// </summary>
    public void AssignTo(string assigneeId)
    {
        if (string.IsNullOrWhiteSpace(assigneeId))
        {
            throw new ArgumentException("Assignee identifier cannot be empty.", nameof(assigneeId));
        }

        AssigneeId = assigneeId.Trim();
        if (Status == TicketStatus.New)
        {
            ChangeStatus(TicketStatus.InProgress);
        }
    }

    /// <summary>
    /// Застосовує доменні правила переходів і фіксує час вирішення заявки.
    /// </summary>
    public void ChangeStatus(TicketStatus newStatus)
    {
        if (Status == newStatus)
        {
            return;
        }

        var isAllowed = (Status, newStatus) switch
        {
            (TicketStatus.New, TicketStatus.InProgress) => true,
            (TicketStatus.InProgress, TicketStatus.Resolved) => true,
            (TicketStatus.Resolved, TicketStatus.Closed) => true,
            (TicketStatus.Resolved, TicketStatus.InProgress) => true,
            _ => false
        };

        if (!isAllowed)
        {
            throw new InvalidStatusTransitionException(
                Status,
                newStatus,
                "The requested status transition is not allowed.");
        }

        if (newStatus is TicketStatus.Resolved or TicketStatus.Closed && string.IsNullOrWhiteSpace(AssigneeId))
        {
            throw new InvalidStatusTransitionException(
                Status,
                newStatus,
                "A ticket must have an assignee before it can be resolved or closed.");
        }

        Status = newStatus;
        if (newStatus == TicketStatus.Resolved)
        {
            ResolvedAt = DateTime.UtcNow;
        }
    }

    /// <summary>Оновлює час останньої зміни перед збереженням у базі.</summary>
    public void MarkUpdated(DateTime updatedAt)
    {
        UpdatedAt = updatedAt;
    }
}
