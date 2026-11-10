using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Infrastructure.Persistence.Configurations;

/// <summary>Налаштовує властивості Ticket та обмеження схеми бази даних.</summary>
public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(ticket => ticket.Id);
        builder.Property(ticket => ticket.Title).IsRequired().HasMaxLength(100);
        builder.Property(ticket => ticket.Description).IsRequired().HasMaxLength(2000);
        builder.Property(ticket => ticket.Category).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(ticket => ticket.Priority).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(ticket => ticket.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(ticket => ticket.CreatedAt).IsRequired();
        builder.Property(ticket => ticket.AssigneeId).HasMaxLength(100);
        builder.HasMany(ticket => ticket.Comments)
            .WithOne(comment => comment.Ticket)
            .HasForeignKey(comment => comment.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
