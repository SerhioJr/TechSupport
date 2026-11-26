using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketSystem.Infrastructure;
using SupportTicketSystem.Infrastructure.Persistence;

namespace SupportTicketSystem.Application.Tests.Configuration;

public sealed class PostgresConfigurationTests
{
    [Fact]
    public void AddInfrastructure_UsesPostgresConnectionFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=postgres;Port=5432;Database=support_tickets;Username=app;Password=test"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", dbContext.Database.ProviderName);
        Assert.Equal("support_tickets", dbContext.Database.GetDbConnection().Database);
    }

    [Fact]
    public void AddInfrastructure_ThrowsWhenConnectionStringIsMissing()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddInfrastructure(configuration));

        Assert.Contains("ConnectionStrings__DefaultConnection", exception.Message);
    }
}
