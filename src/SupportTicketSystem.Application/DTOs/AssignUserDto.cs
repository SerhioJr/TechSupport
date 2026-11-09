using System.ComponentModel.DataAnnotations;

namespace SupportTicketSystem.Application.DTOs;

/// <summary>Тіло запиту для призначення фахівця технічної підтримки.</summary>
public sealed record AssignUserDto
{
    /// <summary>Ідентифікатор фахівця технічної підтримки.</summary>
    [Required]
    public string AssigneeId { get; init; } = string.Empty;
}
