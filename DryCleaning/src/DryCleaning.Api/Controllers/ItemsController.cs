using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// Работает с изделиями, принимаемыми в химчистку
/// </summary>
/// <param name="service">Сервис изделий</param>
/// <param name="logger">Журнал ошибок контроллера</param>
[ApiController]
[Route("api/[controller]")]
public class ItemsController(IItemService service, ILogger<ItemsController> logger) : ControllerBase
{
    private readonly IItemService _service = service;
    private readonly ILogger<ItemsController> _logger = logger;

    /// <summary>
    /// Возвращает изделия вместе с их категориями
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ItemDto>>> GetAll()
    {
        try
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetAll));
        }
    }

    /// <summary>
    /// Возвращает изделия категории или 404, если категория не найдена
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    [HttpGet("/api/categories/{categoryId:int}/items")]
    [ProducesResponseType(typeof(IReadOnlyList<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ItemDto>>> GetByCategoryId(int categoryId)
    {
        try
        {
            var items = await _service.GetByCategoryIdAsync(categoryId);
            if (items is null)
            {
                _logger.LogWarning("Категория {CategoryId} для получения изделий не найдена", categoryId);
                return NotFound();
            }

            return Ok(items);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetByCategoryId));
        }
    }

    /// <summary>
    /// Возвращает изделие по идентификатору или 404, если оно не найдено
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemDto>> GetById(int id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            if (item is null)
            {
                _logger.LogWarning("Изделие {ItemId} не найдено", id);
                return NotFound();
            }

            return Ok(item);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetById));
        }
    }

    /// <summary>
    /// Создает изделие с существующей категорией
    /// </summary>
    /// <param name="request">Название, материал и идентификатор категории изделия</param>
    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemDto>> Create([FromBody] ItemEditDto request)
    {
        try
        {
            var item = await _service.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = item.ItemId }, item);
        }
        catch (ValidationException exception)
        {
            return InvalidItem(exception, nameof(Create));
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Create));
        }
    }

    /// <summary>
    /// Обновляет данные изделия и его категорию
    /// </summary>
    /// <param name="id">Идентификатор изменяемого изделия</param>
    /// <param name="request">Новые название, материал и идентификатор категории</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] ItemEditDto request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated)
            {
                _logger.LogWarning("Изделие {ItemId} для обновления не найдено", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (ValidationException exception)
        {
            return InvalidItem(exception, nameof(Update));
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Update));
        }
    }

    /// <summary>
    /// Удаляет изделие, если с ним не связаны заказы
    /// </summary>
    /// <param name="id">Идентификатор удаляемого изделия</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Изделие {ItemId} для удаления не найдено", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (ItemHasOrdersException exception)
        {
            _logger.LogWarning(exception, "Нельзя удалить изделие {ItemId}, с которым связаны заказы", id);
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Удаление изделия невозможно.",
                Detail = "С изделием связаны заказы."
            });
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Delete));
        }
    }

    /// <summary>
    /// Записывает ошибку проверки данных в журнал и возвращает ответ 400
    /// </summary>
    /// <param name="exception">Ошибка проверки данных изделия</param>
    /// <param name="operation">Название операции</param>
    /// <returns>Ответ с ошибками проверки данных</returns>
    private ActionResult InvalidItem(ValidationException exception, string operation)
    {
        _logger.LogWarning(exception, "Некорректные данные изделия при выполнении операции {Operation}", operation);
        ModelState.AddModelError("request", "Некорректные данные изделия или категория не найдена.");
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
