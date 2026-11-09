using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Domain.Exceptions;

namespace SupportTicketSystem.Application.Tests.Domain;

public sealed class TicketStatusTransitionTests
{
    [Fact]
    public void AssigningNewTicket_MovesItToInProgress()
    {
        var ticket = CreateTicket();

        ticket.AssignTo("specialist-1");

        Assert.Equal("specialist-1", ticket.AssigneeId);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Fact]
    public void AssignedTicket_CanMoveToResolvedAndClosed()
    {
        var ticket = CreateTicket();
        ticket.AssignTo("specialist-1");

        ticket.ChangeStatus(TicketStatus.Resolved);
        ticket.ChangeStatus(TicketStatus.Closed);

        Assert.Equal(TicketStatus.Closed, ticket.Status);
        Assert.NotNull(ticket.ResolvedAt);
    }

    [Fact]
    public void NewTicket_CannotBeClosedWithoutAssignee()
    {
        var ticket = CreateTicket();

        var exception = Assert.Throws<InvalidStatusTransitionException>(
            () => ticket.ChangeStatus(TicketStatus.Closed));

        Assert.Equal(TicketStatus.New, exception.CurrentStatus);
        Assert.Equal(TicketStatus.Closed, exception.NewStatus);
    }

    [Fact]
    public void NewTicket_CannotMoveDirectlyToResolved()
    {
        var ticket = CreateTicket();

        Assert.Throws<InvalidStatusTransitionException>(
            () => ticket.ChangeStatus(TicketStatus.Resolved));
    }

    private static Ticket CreateTicket() => new(
        "Cannot access VPN",
        "The VPN connection fails.",
        TicketCategory.Network,
        TicketPriority.High);
}
