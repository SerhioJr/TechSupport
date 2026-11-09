using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Mapping;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Application.Services;

/// <summary>
/// Сценарії Application-рівня для читання, створення, призначення
/// та оновлення заявок. Сервіс координує репозиторій і передає бізнес-правила
/// доменній сутності Ticket.
/// </summary>
public sealed class TicketService(ITicketRepository repository) : ITicketService
{
    /// <summary>Завантажує заявку та перетворює її у відповідь API.</summary>
    public async Task<TicketResponseDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetByIdAsync(id, cancellationToken);
        return ticket?.ToResponse();
    }

    public async Task<TicketResponseDto> CreateAsync(
        CreateTicketDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = new Ticket(dto.Title.Trim(), dto.Description.Trim(), dto.Category, dto.Priority);

        await repository.AddAsync(ticket, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ticket.ToResponse();
    }

    public async Task<TicketResponseDto?> AssignAsync(
        Guid id,
        AssignUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        ticket.AssignTo(dto.AssigneeId);
        await repository.SaveChangesAsync(cancellationToken);
        return ticket.ToResponse();
    }

    public async Task<TicketResponseDto?> UpdateStatusAsync(
        Guid id,
        UpdateTicketStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        ticket.ChangeStatus(dto.NewStatus);
        await repository.SaveChangesAsync(cancellationToken);
        return ticket.ToResponse();
    }
}
