using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Validators;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tests.Validators;

public sealed class CreateTicketDtoValidatorTests
{
    private readonly CreateTicketDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_WhenTitleIsEmpty_ReturnsValidationError()
    {
        var dto = ValidDto() with { Title = string.Empty };

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTicketDto.Title));
    }

    [Fact]
    public async Task ValidateAsync_WhenTitleExceedsLimit_ReturnsValidationError()
    {
        var dto = ValidDto() with { Title = new string('x', 101) };

        var result = await _validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTicketDto.Title));
    }

    [Fact]
    public async Task ValidateAsync_WhenDataIsValid_ReturnsSuccess()
    {
        var result = await _validator.ValidateAsync(ValidDto());

        Assert.True(result.IsValid);
    }

    private static CreateTicketDto ValidDto() => new()
    {
        Title = "Cannot access VPN",
        Description = "The VPN connection fails for the user.",
        Category = TicketCategory.Network,
        Priority = TicketPriority.High
    };
}
