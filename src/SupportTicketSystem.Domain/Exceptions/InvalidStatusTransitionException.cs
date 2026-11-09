using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Domain.Exceptions;

/// <summary>
/// Виникає, коли запитаний перехід статусу порушує доменні правила роботи із заявкою.
/// </summary>
public sealed class InvalidStatusTransitionException(TicketStatus currentStatus, TicketStatus newStatus, string reason)
    : InvalidOperationException($"Cannot change ticket status from {currentStatus} to {newStatus}. {reason}")
{
    public TicketStatus CurrentStatus { get; } = currentStatus;
    public TicketStatus NewStatus { get; } = newStatus;
}
