using DryCleaning.Domain.Seeds;
using DryCleaning.Application.Services;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Infrastructure.InMemory.Repositories;
using DryCleaning.Infrastructure.InMemory;

var builder = WebApplication.CreateBuilder(args);

// Записываем в лог ошибки проверки запроса до вызова контроллера
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    var defaultResponseFactory = options.InvalidModelStateResponseFactory;

    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        var invalidFields = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .Select(entry => entry.Key);

        logger.LogWarning("Некорректные данные запроса {Method} {Path}. Поля: {Fields}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            string.Join(", ", invalidFields));

        return defaultResponseFactory(context);
    };
});
builder.Services.AddOpenApi();
// Один набор данных используется всеми запросами до остановки приложения.
builder.Services.AddSingleton<TestData>();
builder.Services.AddSingleton<IRelationshipLock, RelationshipLock>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IItemCategoryRepository, ItemCategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemCategoryService, ItemCategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
var app = builder.Build();

// Документация и интерфейс проверки API доступны в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "DryCleaning API v1");
    });
}

app.UseHttpsRedirection();

// Записываем в лог запросы с неподдерживаемым типом содержимого
app.Use(async (context, next) =>
{
    await next(context);

    if (context.Response.StatusCode == StatusCodes.Status415UnsupportedMediaType)
    {
        app.Logger.LogWarning("Ответ 415 на запрос {Method} {Path}. Content-Type: {ContentType}",
            context.Request.Method,
            context.Request.Path,
            context.Request.ContentType);
    }
});

app.MapControllers();

app.Run();
