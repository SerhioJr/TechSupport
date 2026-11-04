using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
}