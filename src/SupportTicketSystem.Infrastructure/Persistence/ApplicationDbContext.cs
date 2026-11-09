using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Infrastructure.Persistence;

/// <summary>
/// Контекст бази даних EF Core. Також оновлює UpdatedAt для змінених заявок.
/// </summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфігурації сутностей винесені в окремі класи, щоб не перевантажувати контекст.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Цей код виконується під час кожного оновлення, тому UpdatedAt не можна випадково забути.
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Ticket>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkUpdated(now);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
