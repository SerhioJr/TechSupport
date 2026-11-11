using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Infrastructure.Persistence;
using SupportTicketSystem.Infrastructure.Repositories;

namespace SupportTicketSystem.Application.Tests.Repositories;

public sealed class TicketRepositoryTests
{
    [Fact]
    public async Task GetTicketsAsync_FiltersByCategory()
    {
        await using var database = await CreateDatabaseAsync();
        var hardware = CreateTicket("Laptop issue", TicketCategory.Hardware, TicketPriority.Low);
        var network = CreateTicket("VPN issue", TicketCategory.Network, TicketPriority.High);
        database.Context.Tickets.AddRange(hardware, network);
        await database.Context.SaveChangesAsync();

        var result = await database.Repository.GetTicketsAsync(new TicketQueryFilter
        {
            Category = TicketCategory.Hardware
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(hardware.Id, Assert.Single(result.Items).Id);
    }

    [Fact]
    public async Task GetTicketsAsync_FiltersByPriority()
    {
        await using var database = await CreateDatabaseAsync();
        var highPriority = CreateTicket("Urgent issue", TicketCategory.Network, TicketPriority.High);
        var lowPriority = CreateTicket("Minor issue", TicketCategory.Network, TicketPriority.Low);
        database.Context.Tickets.AddRange(highPriority, lowPriority);
        await database.Context.SaveChangesAsync();

        var result = await database.Repository.GetTicketsAsync(new TicketQueryFilter
        {
            Priority = TicketPriority.High
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(highPriority.Id, Assert.Single(result.Items).Id);
    }

    [Fact]
    public async Task GetTicketsAsync_FiltersByStatus()
    {
        await using var database = await CreateDatabaseAsync();
        var inProgress = CreateTicket("In progress", TicketCategory.Software, TicketPriority.Medium);
        inProgress.AssignTo("specialist-1");
        var newTicket = CreateTicket("New ticket", TicketCategory.Software, TicketPriority.Medium);
        database.Context.Tickets.AddRange(inProgress, newTicket);
        await database.Context.SaveChangesAsync();

        var result = await database.Repository.GetTicketsAsync(new TicketQueryFilter
        {
            Status = TicketStatus.InProgress
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(inProgress.Id, Assert.Single(result.Items).Id);
    }

    [Fact]
    public async Task GetTicketsAsync_CombinesFiltersAndSearchesCaseInsensitivelyWithPagination()
    {
        await using var database = await CreateDatabaseAsync();
        var matching = CreateTicket("VPN CLIENT", TicketCategory.Software, TicketPriority.High);
        matching.AssignTo("specialist-1");
        var differentAssignee = CreateTicket("VPN configuration", TicketCategory.Software, TicketPriority.High);
        differentAssignee.AssignTo("specialist-2");
        var differentCategory = CreateTicket("VPN hardware", TicketCategory.Hardware, TicketPriority.High);
        differentCategory.AssignTo("specialist-1");
        database.Context.Tickets.AddRange(matching, differentAssignee, differentCategory);
        await database.Context.SaveChangesAsync();

        var result = await database.Repository.GetTicketsAsync(new TicketQueryFilter
        {
            Category = TicketCategory.Software,
            Status = TicketStatus.InProgress,
            AssigneeId = "specialist-1",
            SearchTerm = "vpn",
            PageNumber = 1,
            PageSize = 1
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(matching.Id, Assert.Single(result.Items).Id);
    }

    [Fact]
    public async Task GetTicketsAsync_ReturnsTotalCountBeforePagination()
    {
        await using var database = await CreateDatabaseAsync();
        var tickets = Enumerable.Range(1, 3)
            .Select(index => CreateTicket($"Issue {index}", TicketCategory.Other, TicketPriority.Low))
            .ToArray();
        database.Context.Tickets.AddRange(tickets);
        await database.Context.SaveChangesAsync();

        var result = await database.Repository.GetTicketsAsync(new TicketQueryFilter
        {
            PageNumber = 2,
            PageSize = 1
        });

        Assert.Equal(3, result.TotalCount);
        Assert.Single(result.Items);
    }

    private static Ticket CreateTicket(string title, TicketCategory category, TicketPriority priority) =>
        new(title, "Test description", category, priority);

    private static async Task<DatabaseFixture> CreateDatabaseAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return new DatabaseFixture(connection, context, new TicketRepository(context));
    }

    private sealed class DatabaseFixture(
        SqliteConnection connection,
        ApplicationDbContext context,
        TicketRepository repository) : IAsyncDisposable
    {
        public ApplicationDbContext Context { get; } = context;
        public TicketRepository Repository { get; } = repository;

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await connection.DisposeAsync();
        }
    }
}
