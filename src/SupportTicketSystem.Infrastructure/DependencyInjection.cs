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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=support-tickets.db"));
        services.AddScoped<ITicketRepository, TicketRepository>();
        return services;
    }
}
