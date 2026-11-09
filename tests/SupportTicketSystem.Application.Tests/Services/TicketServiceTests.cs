using Moq;
using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tests.Services;

public sealed class TicketServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsTicketAndAssignsNewStatus()
    {
        var repository = new Mock<ITicketRepository>();
        var service = new TicketService(repository.Object);
        var dto = new CreateTicketDto
        {
            Title = "Cannot access VPN",
            Description = "The VPN connection fails for the user.",
            Category = TicketCategory.Network,
            Priority = TicketPriority.High
        };

        var result = await service.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(TicketStatus.New, result.Status);
        Assert.Equal(dto.Title, result.Title);
        repository.Verify(x => x.AddAsync(
            It.Is<SupportTicketSystem.Domain.Entities.Ticket>(ticket => ticket.Status == TicketStatus.New),
            It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
