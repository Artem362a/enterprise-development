using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// Работает со справочником категорий изделий и их популярностью по заказам
/// </summary>
/// <param name="service">Сервис категорий</param>
/// <param name="logger">Журнал ошибок контроллера</param>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    IItemCategoryService service,
    ILogger<CategoriesController> logger) : ControllerBase
{
    private readonly IItemCategoryService _service = service;
    private readonly ILogger<CategoriesController> _logger = logger;

    /// <summary>
    /// Возвращает категории с тарифами и рекомендуемыми видами обработки
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ItemCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ItemCategoryDto>>> GetAll()
    {
        try
        {
            var categories = await _service.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetAll));
        }
    }

    /// <summary>
    /// Возвращает категорию по идентификатору или 404, если она не найдена
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemCategoryDto>> GetById(int id)
    {
        try
        {
            var category = await _service.GetByIdAsync(id);
            if (category is null)
            {
                _logger.LogWarning("Категория {CategoryId} не найдена", id);
                return NotFound();
            }

            return Ok(category);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetById));
        }
    }

    /// <summary>
    /// Возвращает пять наиболее и пять наименее популярных категорий за предшествующий год
    /// Отменённые заказы не учитываются; категории без заказов входят в выборку
    /// </summary>
    /// <param name="end">Конец периода, не включаемый в расчёт</param>
    [HttpGet("popularity")]
    [ProducesResponseType(typeof(CategoryPopularityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryPopularityDto>> GetPopularity([FromQuery] DateTime end)
    {
        try
        {
            var result = await _service.GetPopularityAsync(end);
            return Ok(result);
        }
        catch (ArgumentOutOfRangeException exception) when (end.Year <= 1)
        {
            _logger.LogWarning(exception, "Некорректный конец периода: {End}", end);
            ModelState.AddModelError(nameof(end), "Укажите дату окончания периода, позволяющую вычислить предыдущий год.");
            return ValidationProblem(ModelState);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetPopularity));
        }
    }

    /// <summary>
    /// Добавляет категорию, если цена положительная и корректный вид обработки
    /// </summary>
    /// <param name="request">Название, вид обработки и цена новой категории</param>
    [HttpPost]
    [ProducesResponseType(typeof(ItemCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemCategoryDto>> Create([FromBody] ItemCategoryEditDto request)
    {
        try
        {
            var category = await _service.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, category);
        }
        catch (ValidationException exception)
        {
            return InvalidCategory(exception, nameof(Create));
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Create));
        }
    }

    /// <summary>
    /// Обновляет категорию после проверки цены и вида обработки
    /// </summary>
    /// <param name="id">Идентификатор изменяемой категории</param>
    /// <param name="request">Новые название, вид обработки и цена</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] ItemCategoryEditDto request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated)
            {
                _logger.LogWarning("Категория {CategoryId} для обновления не найдена", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (ValidationException exception)
        {
            return InvalidCategory(exception, nameof(Update));
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Update));
        }
    }

    /// <summary>
    /// Удаляет категорию, если ни одно изделие на неё не ссылается
    /// </summary>
    /// <param name="id">Идентификатор удаляемой категории</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
                _logger.LogWarning("Категория {CategoryId} для удаления не найдена", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (CategoryHasItemsException exception)
        {
            _logger.LogWarning(exception, "Нельзя удалить категорию {CategoryId}, которая используется изделиями", id);
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Удаление категории невозможно.",
                Detail = "Категория используется изделиями."
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
    /// <param name="exception">Ошибка проверки данных категории</param>
    /// <param name="operation">Название операции</param>
    /// <returns>Ответ с ошибками проверки данных</returns>
    private ActionResult InvalidCategory(ValidationException exception, string operation)
    {
        _logger.LogWarning(exception, "Некорректные данные категории при выполнении операции {Operation}", operation);
        ModelState.AddModelError("request", "Некорректные данные категории.");
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
