namespace SupportTicketSystem.Domain.Entities;

/// <summary>Коментар, залишений користувачем або фахівцем у заявці.</summary>
public sealed class TicketComment
{
    private TicketComment()
    {
    }

    public TicketComment(Guid ticketId, string authorName, string message)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        AuthorName = authorName;
        Message = message;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string AuthorName { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public Ticket Ticket { get; private set; } = null!;
}
