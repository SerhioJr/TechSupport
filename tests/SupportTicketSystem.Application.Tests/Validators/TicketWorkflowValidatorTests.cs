using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Validators;

namespace SupportTicketSystem.Application.Tests.Validators;

public sealed class TicketWorkflowValidatorTests
{
    [Fact]
    public async Task AssignUserValidator_RejectsEmptyAssignee()
    {
        var result = await new AssignUserDtoValidator().ValidateAsync(new AssignUserDto());

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task StatusValidator_RejectsInvalidStatusValue()
    {
        var result = await new UpdateTicketStatusDtoValidator()
            .ValidateAsync(new UpdateTicketStatusDto { NewStatus = (SupportTicketSystem.Domain.Enums.TicketStatus)99 });

        Assert.False(result.IsValid);
    }
}
