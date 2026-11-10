using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Infrastructure.Persistence.Configurations;

/// <summary>Налаштовує таблицю коментарів і зв'язок із заявкою.</summary>
public sealed class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.HasKey(comment => comment.Id);
        builder.Property(comment => comment.AuthorName).IsRequired().HasMaxLength(100);
        builder.Property(comment => comment.Message).IsRequired().HasMaxLength(1000);
        builder.Property(comment => comment.CreatedAt).IsRequired();
        builder.HasIndex(comment => new { comment.TicketId, comment.CreatedAt });
    }
}
