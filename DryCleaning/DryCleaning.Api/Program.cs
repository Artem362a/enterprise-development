using DryCleaning.Domain.Seeds;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Один набор данных используется всеми запросами до остановки приложения.
builder.Services.AddSingleton<TestData>();

var app = builder.Build();

// Описание API доступно только в режиме разработки.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
