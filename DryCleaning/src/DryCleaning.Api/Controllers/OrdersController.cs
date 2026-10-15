using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// Работает с заказами химчистки и выборкой заказов в обработке
/// </summary>
/// <param name="service">Сервис заказов</param>
/// <param name="logger">Журнал ошибок контроллера</param>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService service, ILogger<OrdersController> logger) : ControllerBase
{
    private readonly IOrderService _service = service;
    private readonly ILogger<OrdersController> _logger = logger;

    /// <summary>
    /// Возвращает все заказы
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll()
    {
        try
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetAll));
        }
    }

    /// <summary>
    /// Возвращает заказы клиента или 404, если клиент не найден
    /// </summary>
    /// <param name="clientId">Идентификатор клиента</param>
    [HttpGet("/api/clients/{clientId:int}/orders")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByClientId(int clientId)
    {
        try
        {
            var orders = await _service.GetByClientIdAsync(clientId);
            if (orders is null)
            {
                _logger.LogWarning("Клиент {ClientId} для получения заказов не найден", clientId);
                return NotFound();
            }

            return Ok(orders);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetByClientId));
        }
    }

    /// <summary>
    /// Возвращает заказ по идентификатору или 404, если он не найден
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        try
        {
            var order = await _service.GetByIdAsync(id);
            if (order is null)
            {
                _logger.LogWarning("Заказ {OrderId} не найден", id);
                return NotFound();
            }

            return Ok(order);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetById));
        }
    }

    /// <summary>
    /// Возвращает заказы в обработке по дате приема, затем по идентификатору
    /// </summary>
    [HttpGet("in-progress")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetInProgress()
    {
        try
        {
            var orders = await _service.GetInProgressAsync();
            return Ok(orders);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetInProgress));
        }
    }

    /// <summary>
    /// Создает заказ для существующих клиента и изделия, фиксируя время приема в UTC
    /// </summary>
    /// <param name="request">Клиент, изделие и плановый срок обработки</param>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto request)
    {
        try
        {
            var order = await _service.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (ValidationException exception)
        {
            return InvalidOrder(exception, nameof(Create));
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Create));
        }
    }

    /// <summary>
    /// Обновляет данные заказа после проверки статуса и даты завершения
    /// </summary>
    /// <param name="id">Идентификатор изменяемого заказа</param>
    /// <param name="request">Новые данные заказа</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] OrderEditDto request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated)
            {
                _logger.LogWarning("Заказ {OrderId} для обновления не найден", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (ValidationException exception)
        {
            return InvalidOrder(exception, nameof(Update));
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Update));
        }
    }

    /// <summary>
    /// Удаляет заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор удаляемого заказа</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Заказ {OrderId} для удаления не найден", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Delete));
        }
    }

    /// <summary>
    /// Записывает ошибку проверки данных в журнал и возвращает ответ 400
    /// </summary>
    /// <param name="exception">Ошибка проверки данных заказа</param>
    /// <param name="operation">Название операции</param>
    /// <returns>Ответ с ошибками проверки данных</returns>
    private ActionResult InvalidOrder(ValidationException exception, string operation)
    {
        _logger.LogWarning(exception, "Некорректные данные заказа при выполнении операции {Operation}", operation);
        ModelState.AddModelError("request", "Некорректные данные заказа.");
        return ValidationProblem(ModelState);
    }

    /// <summary>
    /// Записывает ошибку в журнал и возвращает ответ без внутренних деталей
    /// </summary>
    /// <param name="exception">Возникшая ошибка</param>
    /// <param name="operation">Название операции</param>
    /// <returns>Ответ с кодом 500</returns>
    private ObjectResult ServerError(Exception exception, string operation)
    {
        _logger.LogError(exception, "Ошибка при выполнении операции {Operation}", operation);
        return Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Не удалось обработать запрос.");
    }
}
