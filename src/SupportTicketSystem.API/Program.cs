using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.DTOs;
using SupportTicketSystem.Application.Services;
using SupportTicketSystem.Application.Validators;
using SupportTicketSystem.Infrastructure;
using SupportTicketSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Реєструємо MVC, перетворення enum у JSON, валідацію, Application-сервіси та збереження даних.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped<IValidator<TicketQueryFilter>, TicketQueryFilterValidator>();
builder.Services.AddScoped<IValidator<CreateTicketDto>, CreateTicketDtoValidator>();
builder.Services.AddScoped<IValidator<AssignUserDto>, AssignUserDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateTicketStatusDto>, UpdateTicketStatusDtoValidator>();
builder.Services.AddScoped<IValidator<CreateCommentDto>, CreateCommentDtoValidator>();
builder.Services.AddInfrastructure(builder.Configuration);
// Swagger використовує XML-коментарі контролерів і DTO для зрозумілої документації API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var apiXml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    var applicationXml = Path.Combine(AppContext.BaseDirectory, $"{typeof(TicketQueryFilter).Assembly.GetName().Name}.xml");

    if (File.Exists(apiXml))
    {
        options.IncludeXmlComments(apiXml, includeControllerXmlComments: true);
    }

    if (File.Exists(applicationXml))
    {
        options.IncludeXmlComments(applicationXml);
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Під час запуску лабораторного проєкту автоматично застосовуємо всі очікувані міграції.
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Налаштовуємо middleware та відкриваємо маршрути контролерів.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
