using SupportTicketSystem.Application.DTOs;

namespace SupportTicketSystem.Application.Services;

public interface ITicketService
{
    Task<TicketResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TicketResponseDto> CreateAsync(CreateTicketDto dto, CancellationToken cancellationToken = default);
    Task<TicketResponseDto?> AssignAsync(Guid id, AssignUserDto dto, CancellationToken cancellationToken = default);
    Task<TicketResponseDto?> UpdateStatusAsync(Guid id, UpdateTicketStatusDto dto, CancellationToken cancellationToken = default);
    Task<CommentResponseDto?> AddCommentAsync(Guid id, CreateCommentDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentResponseDto>?> GetCommentsAsync(Guid id, CancellationToken cancellationToken = default);
}
