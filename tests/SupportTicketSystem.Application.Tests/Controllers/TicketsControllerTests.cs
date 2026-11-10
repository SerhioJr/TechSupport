using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using SupportTicketSystem.API.Controllers;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Application.Validators;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Tests.Controllers;

public sealed class TicketsControllerTests
{
    [Fact]
    public async Task CreateTicket_WhenRequestIsValid_Returns201WithLocation()
    {
        var expected = new TicketResponseDto(
            Guid.NewGuid(),
            "Cannot access VPN",
            "The VPN connection fails.",
            TicketCategory.Network,
            TicketPriority.High,
            TicketStatus.New,
            DateTime.UtcNow,
            null);
        var service = new Mock<ITicketService>();
        service.Setup(x => x.CreateAsync(It.IsAny<CreateTicketDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);
        var controller = CreateController(service);

        var result = await controller.CreateTicket(ValidDto(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal(expected, created.Value);
        Assert.Equal(nameof(TicketsController.GetById), created.ActionName);
    }

    [Fact]
    public async Task CreateTicket_WhenRequestIsInvalid_Returns400WithValidationDetails()
    {
        var service = new Mock<ITicketService>();
        var controller = CreateController(service);

        var result = await controller.CreateTicket(
            ValidDto() with { Title = "bad" },
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        service.Verify(x => x.CreateAsync(It.IsAny<CreateTicketDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CreateTicketDto ValidDto() => new()
    {
        Title = "Cannot access VPN",
        Description = "The VPN connection fails.",
        Category = TicketCategory.Network,
        Priority = TicketPriority.High
    };

    private static TicketsController CreateController(Mock<ITicketService> service) => new(
        service.Object,
        new CreateTicketDtoValidator(),
        new SupportTicketSystem.Application.Validators.AssignUserDtoValidator(),
        new SupportTicketSystem.Application.Validators.UpdateTicketStatusDtoValidator(),
        new SupportTicketSystem.Application.Validators.CreateCommentDtoValidator());
}
