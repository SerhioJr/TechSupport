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
public sealed class TicketService(ITicketRepository repository, TimeProvider? timeProvider = null) : ITicketService
{
    private readonly TimeProvider clock = timeProvider ?? TimeProvider.System;

    /// <summary>Завантажує заявку та перетворює її у відповідь API.</summary>
    public async Task<TicketResponseDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetByIdAsync(id, cancellationToken);
        return ticket?.ToResponse(clock.GetUtcNow().UtcDateTime);
    }

    /// <summary>Повертає сторінку заявок, відфільтрованих репозиторієм.</summary>
    public async Task<PagedResult<TicketResponseDto>> GetTicketsAsync(
        TicketQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetTicketsAsync(filter, cancellationToken);
        var currentTimeUtc = clock.GetUtcNow().UtcDateTime;
        var items = result.Items.Select(ticket => ticket.ToResponse(currentTimeUtc)).ToArray();

        return new PagedResult<TicketResponseDto>(items, result.TotalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<TicketResponseDto> CreateAsync(
        CreateTicketDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = new Ticket(dto.Title.Trim(), dto.Description.Trim(), dto.Category, dto.Priority);

        await repository.AddAsync(ticket, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ticket.ToResponse(clock.GetUtcNow().UtcDateTime);
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
        return ticket.ToResponse(clock.GetUtcNow().UtcDateTime);
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
        return ticket.ToResponse(clock.GetUtcNow().UtcDateTime);
    }

    public async Task<CommentResponseDto?> AddCommentAsync(
        Guid id,
        CreateCommentDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        var comment = new TicketComment(id, dto.AuthorName.Trim(), dto.Message.Trim());
        ticket.MarkUpdated(DateTime.UtcNow);
        await repository.AddCommentAsync(comment, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return comment.ToResponse();
    }

    public async Task<IReadOnlyList<CommentResponseDto>?> GetCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (await repository.GetByIdAsync(id, cancellationToken) is null)
        {
            return null;
        }

        var comments = await repository.GetCommentsAsync(id, cancellationToken);
        return comments.Select(comment => comment.ToResponse()).ToArray();
    }
}
