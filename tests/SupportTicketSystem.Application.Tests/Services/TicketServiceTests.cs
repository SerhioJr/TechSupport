using Moq;
using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Entities;
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

    [Fact]
    public async Task GetTicketsAsync_CalculatesDueDateAndMarksOverdueTicket()
    {
        var ticket = new Ticket("VPN issue", "Connection fails", TicketCategory.Network, TicketPriority.High);
        var repository = new Mock<ITicketRepository>();
        repository.Setup(x => x.GetTicketsAsync(It.IsAny<TicketQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Ticket>)new[] { ticket }, 1));
        var currentTime = new DateTimeOffset(ticket.CreatedAt.AddHours(5), TimeSpan.Zero);
        var service = new TicketService(repository.Object, new FixedTimeProvider(currentTime));

        var result = await service.GetTicketsAsync(new TicketQueryFilter());

        var response = Assert.Single(result.Items);
        Assert.Equal(ticket.CreatedAt.AddHours(4), response.DueDate);
        Assert.True(response.IsOverdue);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Theory]
    [InlineData(TicketPriority.High, 4)]
    [InlineData(TicketPriority.Medium, 24)]
    [InlineData(TicketPriority.Low, 72)]
    public async Task GetTicketsAsync_UsesSlaDurationForEachPriority(TicketPriority priority, int hours)
    {
        var ticket = new Ticket("Issue", "Description", TicketCategory.Other, priority);
        var repository = new Mock<ITicketRepository>();
        repository.Setup(x => x.GetTicketsAsync(It.IsAny<TicketQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Ticket>)new[] { ticket }, 1));
        var service = new TicketService(
            repository.Object,
            new FixedTimeProvider(new DateTimeOffset(ticket.CreatedAt, TimeSpan.Zero)));

        var result = await service.GetTicketsAsync(new TicketQueryFilter());

        Assert.Equal(ticket.CreatedAt.AddHours(hours), Assert.Single(result.Items).DueDate);
    }

    [Fact]
    public async Task GetTicketsAsync_DoesNotMarkResolvedTicketOverdue()
    {
        var ticket = new Ticket("VPN issue", "Connection fails", TicketCategory.Network, TicketPriority.High);
        ticket.AssignTo("specialist-1");
        ticket.ChangeStatus(TicketStatus.Resolved);
        var repository = new Mock<ITicketRepository>();
        repository.Setup(x => x.GetTicketsAsync(It.IsAny<TicketQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(((IReadOnlyList<Ticket>)new[] { ticket }, 1));
        var currentTime = new DateTimeOffset(ticket.CreatedAt.AddHours(5), TimeSpan.Zero);
        var service = new TicketService(repository.Object, new FixedTimeProvider(currentTime));

        var result = await service.GetTicketsAsync(new TicketQueryFilter());

        Assert.False(Assert.Single(result.Items).IsOverdue);
    }

    private sealed class FixedTimeProvider(DateTimeOffset currentTime) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => currentTime;
    }
}
