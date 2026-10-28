using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ilin_LR5.Models;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// ЛР5, п.3: ім'я бази даних береться з конфігурації та перевіряється при запуску
builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<TodoContext>((services, opt) =>
{
    var dbOptions = services.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    opt.UseInMemoryDatabase(dbOptions.Name!);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (OptionsValidationException ex)
{
    // Виведення помилок валідації конфігурації
    Console.WriteLine("Помилка конфігурації бази даних:");
    foreach (var failure in ex.Failures)
    {
        Console.WriteLine($" - {failure}");
    }
    Environment.ExitCode = 1;

    if (!Console.IsInputRedirected)
    {
        Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
}
