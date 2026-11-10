using Moq;
using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tests.Services;

public sealed class CommentServiceTests
{
    [Fact]
    public async Task AddCommentAsync_WhenTicketExists_SavesCommentAndUpdatesActivity()
    {
        var ticket = new Ticket("VPN", "Connection fails", TicketCategory.Network, TicketPriority.High);
        var repository = new Mock<ITicketRepository>();
        repository.Setup(x => x.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        var service = new TicketService(repository.Object);
        var dto = new CreateCommentDto { AuthorName = " Olena ", Message = " Restarted VPN " };

        var result = await service.AddCommentAsync(ticket.Id, dto);

        Assert.NotNull(result);
        Assert.Equal("Olena", result.AuthorName);
        Assert.Equal("Restarted VPN", result.Message);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(ticket.UpdatedAt);
        repository.Verify(x => x.AddCommentAsync(
            It.Is<TicketComment>(comment =>
                comment.TicketId == ticket.Id &&
                comment.AuthorName == "Olena" &&
                comment.Message == "Restarted VPN"),
            It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_WhenTicketDoesNotExist_ReturnsNullAndDoesNotSave()
    {
        var repository = new Mock<ITicketRepository>();
        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);
        var service = new TicketService(repository.Object);

        var result = await service.AddCommentAsync(Guid.NewGuid(),
            new CreateCommentDto { AuthorName = "Olena", Message = "Details" });

        Assert.Null(result);
        repository.Verify(x => x.AddCommentAsync(It.IsAny<TicketComment>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetCommentsAsync_ReturnsCommentsInRepositoryOrder()
    {
        var ticket = new Ticket("VPN", "Connection fails", TicketCategory.Network, TicketPriority.High);
        var comments = new[]
        {
            new TicketComment(ticket.Id, "First", "First message"),
            new TicketComment(ticket.Id, "Second", "Second message")
        };
        var repository = new Mock<ITicketRepository>();
        repository.Setup(x => x.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);
        repository.Setup(x => x.GetCommentsAsync(ticket.Id, It.IsAny<CancellationToken>())).ReturnsAsync(comments);
        var service = new TicketService(repository.Object);

        var result = await service.GetCommentsAsync(ticket.Id);

        Assert.NotNull(result);
        Assert.Collection(result,
            comment => Assert.Equal("First message", comment.Message),
            comment => Assert.Equal("Second message", comment.Message));
    }
}
