using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Application.Abstractions;

/// <summary>
/// Контракт збереження даних, який не змушує Application-рівень залежати від EF Core.
/// </summary>
public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Ticket> Items, int TotalCount)> GetTicketsAsync(
        TicketQueryFilter filter,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketComment>> GetCommentsAsync(Guid ticketId, CancellationToken cancellationToken = default);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task AddCommentAsync(TicketComment comment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
