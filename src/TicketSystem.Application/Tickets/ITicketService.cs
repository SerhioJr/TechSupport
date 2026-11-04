namespace TicketSystem.Application.Tickets;

public interface ITicketService
{
    Task<TicketResponseDto> CreateAsync(CreateTicketDto dto, CancellationToken cancellationToken = default);
}