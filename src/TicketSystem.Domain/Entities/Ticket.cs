using TicketSystem.Domain.Enums;

namespace TicketSystem.Domain.Entities;

public class Ticket
{
    public const int TitleMinLength = 5;
    public const int TitleMaxLength = 100;
    public const int DescriptionMaxLength = 2000;

    private Ticket() { }   // для EF Core

    public Ticket(string title, string description, TicketCategory category, TicketPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Тема не може бути порожньою.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Опис не може бути порожнім.", nameof(description));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Category = category;
        Priority = priority;
        Status = TicketStatus.New;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TicketCategory Category { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}