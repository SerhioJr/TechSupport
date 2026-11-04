using TicketSystem.Domain.Entities;

namespace TicketSystem.Application.Tickets;

public class TicketService(ITicketRepository repository) : ITicketService
{
    public async Task<TicketResponseDto> CreateAsync(CreateTicketDto dto, CancellationToken cancellationToken = default)
    {
        var ticket = dto.ToEntity();
        await repository.AddAsync(ticket, cancellationToken);
        return ticket.ToResponseDto();
    }
}