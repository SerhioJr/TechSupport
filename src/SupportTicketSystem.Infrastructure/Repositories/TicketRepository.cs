using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Infrastructure.Persistence;

namespace SupportTicketSystem.Infrastructure.Repositories;

/// <summary>Реалізація контракту збереження заявок за допомогою EF Core.</summary>
public sealed class TicketRepository(ApplicationDbContext dbContext) : ITicketRepository
{
    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets.SingleOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets.AddAsync(ticket, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
