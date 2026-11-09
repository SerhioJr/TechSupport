using System.Text.Json;
using System.Text.Json.Serialization;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tests.Serialization;

public sealed class TicketDtoSerializationTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public void CreateTicketDto_CanBeSerializedAndDeserialized()
    {
        var dto = new CreateTicketDto
        {
            Title = "Cannot access VPN",
            Description = "The VPN connection fails.",
            Category = TicketCategory.Network,
            Priority = TicketPriority.High
        };

        var json = JsonSerializer.Serialize(dto, Options);
        var result = JsonSerializer.Deserialize<CreateTicketDto>(json, Options);

        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Category, result.Category);
        Assert.Equal(dto.Priority, result.Priority);
    }

    [Fact]
    public void TicketResponseDto_UsesExpectedJsonContract()
    {
        var dto = new TicketResponseDto(
            Guid.NewGuid(),
            "Cannot access VPN",
            "The VPN connection fails.",
            TicketCategory.Network,
            TicketPriority.High,
            TicketStatus.New,
            DateTime.UtcNow,
            null);

        var json = JsonSerializer.Serialize(dto, Options);

        Assert.Contains("\"status\":\"New\"", json);
        Assert.Contains("\"category\":\"Network\"", json);
        Assert.Contains("\"priority\":\"High\"", json);
    }
}
