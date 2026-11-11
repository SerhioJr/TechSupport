using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Application.DTOs;
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

    public async Task<(IReadOnlyList<Ticket> Items, int TotalCount)> GetTicketsAsync(
        TicketQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Ticket> query = dbContext.Tickets.AsNoTracking();

        if (filter.Category.HasValue)
        {
            query = query.Where(ticket => ticket.Category == filter.Category.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(ticket => ticket.Priority == filter.Priority.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(ticket => ticket.Status == filter.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.AssigneeId))
        {
            var assigneeId = filter.AssigneeId.Trim();
            query = query.Where(ticket => ticket.AssigneeId == assigneeId);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(ticket =>
                ticket.Title.ToLower().Contains(searchTerm) ||
                ticket.Description.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var offset = (int)Math.Min(((long)filter.PageNumber - 1) * filter.PageSize, int.MaxValue);
        var items = await query
            .OrderByDescending(ticket => ticket.CreatedAt)
            .ThenBy(ticket => ticket.Id)
            .Skip(offset)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<TicketComment>> GetCommentsAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TicketComment>()
            .Where(comment => comment.TicketId == ticketId)
            .OrderBy(comment => comment.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets.AddAsync(ticket, cancellationToken).AsTask();
    }

    public Task AddCommentAsync(TicketComment comment, CancellationToken cancellationToken = default)
    {
        return dbContext.Set<TicketComment>().AddAsync(comment, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
