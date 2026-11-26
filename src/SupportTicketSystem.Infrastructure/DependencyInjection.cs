using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportTicketSystem.Application.Abstractions;
using SupportTicketSystem.Infrastructure.Persistence;
using SupportTicketSystem.Infrastructure.Repositories;

namespace SupportTicketSystem.Infrastructure;

/// <summary>Реєструє Infrastructure-сервіси у контейнері залежностей застосунку.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is required. " +
                "Configure it with the ConnectionStrings__DefaultConnection environment variable.");
        }

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ITicketRepository, TicketRepository>();
        return services;
    }
}
