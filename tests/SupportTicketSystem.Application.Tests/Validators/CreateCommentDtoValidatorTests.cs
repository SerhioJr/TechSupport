using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Validators;

namespace SupportTicketSystem.Application.Tests.Validators;

public sealed class CreateCommentDtoValidatorTests
{
    private readonly CreateCommentDtoValidator validator = new();

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    public async Task Message_WhenEmptyOrTooShort_IsInvalid(string message)
    {
        var result = await validator.ValidateAsync(new CreateCommentDto
        {
            AuthorName = "Olena",
            Message = message
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateCommentDto.Message));
    }

    [Fact]
    public async Task Message_WhenLongerThan1000Characters_IsInvalid()
    {
        var result = await validator.ValidateAsync(new CreateCommentDto
        {
            AuthorName = "Olena",
            Message = new string('x', 1001)
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidComment_IsAccepted()
    {
        var result = await validator.ValidateAsync(new CreateCommentDto
        {
            AuthorName = "Olena",
            Message = "Проблему уточнено"
        });

        Assert.True(result.IsValid);
    }
}
