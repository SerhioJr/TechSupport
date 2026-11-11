using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Domain.Exceptions;

namespace SupportTicketSystem.API.Controllers;

/// <summary>
/// HTTP-точка входу для операцій із заявками. Контролер перевіряє запити,
/// викликає Application-сервіс і перетворює результати у HTTP-відповіді.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class TicketsController(
    ITicketService ticketService,
    IValidator<CreateTicketDto> validator,
    IValidator<AssignUserDto> assignUserValidator,
    IValidator<UpdateTicketStatusDto> statusValidator,
    IValidator<CreateCommentDto> commentValidator,
    IValidator<TicketQueryFilter> queryFilterValidator) : ControllerBase
{
    /// <summary>Повертає заявку за її ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <response code="200">Заявку знайдено.</response>
    /// <response code="404">Заявки із вказаним ідентифікатором не існує.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Сервіс повертає null, якщо репозиторій не знайшов потрібну заявку.
        var response = await ticketService.GetByIdAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    /// <summary>Повертає відфільтрований список заявок з пагінацією.</summary>
    /// <param name="filter">Категорія, пріоритет, статус, фахівець, пошуковий запит і параметри сторінки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <remarks>
    /// Приклад запиту: <c>GET /api/tickets?category=Network&amp;priority=High&amp;pageNumber=1&amp;pageSize=10</c>.
    /// Приклад елемента сторінки: <c>{"items":[{"title":"VPN не працює","dueDate":"2026-11-11T16:00:00Z","isOverdue":false}],"totalCount":1,"pageNumber":1,"pageSize":10}</c>.
    /// </remarks>
    /// <response code="200">Список заявок і метадані сторінки.</response>
    /// <response code="400">Параметри фільтрації або пагінації некоректні.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TicketResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<TicketResponseDto>>> GetTickets(
        [FromQuery] TicketQueryFilter filter,
        CancellationToken cancellationToken)
    {
        var validationResult = await queryFilterValidator.ValidateAsync(filter, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        var response = await ticketService.GetTicketsAsync(filter, cancellationToken);
        return Ok(response);
    }

    /// <summary>Створює нову заявку технічної підтримки.</summary>
    /// <param name="dto">Тема, опис, категорія та пріоритет заявки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <returns>Створена заявка зі статусом New.</returns>
    /// <response code="201">Заявку успішно створено.</response>
    /// <response code="400">Запит містить помилки валідації.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketResponseDto>> CreateTicket(
        CreateTicketDto dto,
        CancellationToken cancellationToken)
    {
        // Помилки FluentValidation повертаємо у стандартному форматі ValidationProblemDetails.
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return BadRequest(new ValidationProblemDetails(errors));
        }

        var response = await ticketService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Призначає фахівця технічної підтримки для заявки.</summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="dto">Ідентифікатор фахівця технічної підтримки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <response code="200">Фахівця успішно призначено.</response>
    /// <response code="400">Дані призначення некоректні.</response>
    /// <response code="404">Заявки із вказаним ідентифікатором не існує.</response>
    [HttpPatch("{id:guid}/assign")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> AssignUser(
        Guid id,
        AssignUserDto dto,
        CancellationToken cancellationToken)
    {
        // Призначення може перевести заявку з New у InProgress у доменній сутності.
        var validationResult = await assignUserValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        var response = await ticketService.AssignAsync(id, dto, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    /// <summary>Змінює статус заявки технічної підтримки.</summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="dto">Новий статус і необов'язковий коментар.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <response code="200">Статус успішно змінено.</response>
    /// <response code="400">Перехід статусу заборонений або запит некоректний.</response>
    /// <response code="404">Заявки із вказаним ідентифікатором не існує.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponseDto>> UpdateStatus(
        Guid id,
        UpdateTicketStatusDto dto,
        CancellationToken cancellationToken)
    {
        // Доменний виняток перетворюємо на зрозумілу для клієнта відповідь 400.
        var validationResult = await statusValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        try
        {
            var response = await ticketService.UpdateStatusAsync(id, dto, cancellationToken);
            return response is null ? NotFound() : Ok(response);
        }
        catch (InvalidStatusTransitionException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid status transition",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>Додає коментар до заявки.</summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="dto">Автор і текст коментаря.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <response code="201">Коментар успішно додано.</response>
    /// <response code="400">Дані коментаря некоректні.</response>
    /// <response code="404">Заявки із вказаним ідентифікатором не існує.</response>
    [HttpPost("{id:guid}/comments")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentResponseDto>> AddComment(
        Guid id,
        CreateCommentDto dto,
        CancellationToken cancellationToken)
    {
        var validationResult = await commentValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        var response = await ticketService.AddCommentAsync(id, dto, cancellationToken);
        return response is null ? NotFound() : StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>Повертає історію коментарів заявки у хронологічному порядку.</summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="cancellationToken">Токен скасування операції.</param>
    /// <response code="200">Історію коментарів повернуто.</response>
    /// <response code="404">Заявки із вказаним ідентифікатором не існує.</response>
    [HttpGet("{id:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CommentResponseDto>>> GetComments(
        Guid id,
        CancellationToken cancellationToken)
    {
        var response = await ticketService.GetCommentsAsync(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }
}
