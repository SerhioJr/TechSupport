using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Application.Abstractions;

/// <summary>
/// Контракт збереження даних, який не змушує Application-рівень залежати від EF Core.
/// </summary>
public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
