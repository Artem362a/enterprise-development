namespace DryCleaning.Api.Controllers;

using DryCleaning.Api.Requests;
using DryCleaning.Domain;
using DryCleaning.Domain.Enum;
using DryCleaning.Domain.Seeds;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Работает со справочником категорий изделий и их популярностью по заказам.
/// </summary>
/// <param name="data">Категории, изделия и заказы, хранящиеся в памяти.</param>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(TestData data) : ControllerBase
{
    private readonly TestData _data = data;

    /// <summary>
    /// Возвращает категории с тарифами и рекомендуемыми видами обработки.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_data.ItemCategories);
    }
    /// <summary>
    /// Возвращает категорию по идентификатору или 404, если она не найдена.
    /// </summary>
    /// <param name="id">Идентификатор категории.</param>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    /// <summary>
    /// Возвращает пять наиболее и пять наименее популярных категорий за предшествующий год.
    /// Отменённые заказы не учитываются; категории без заказов входят в выборку.
    /// </summary>
    /// <param name="end">Конец периода, не включаемый в расчёт.</param>
    [HttpGet("popularity")]
    public IActionResult GetPopularity([FromQuery] DateTime end)
    {
        if (end.Year <= 1)
            return BadRequest("Укажите дату окончания периода в параметре end.");

        var start = end.AddYears(-1);

        var orders = _data.Orders
            .Where(o => o.AcceptDate >= start && o.AcceptDate < end)
            .Where(o => o.Status != OrderStatus.Cancelled)
            .ToList();

        var categories = _data.ItemCategories
            .Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                ItemCount = orders.Count(o => o.Item.Category.CategoryId == c.CategoryId)
            })
            .ToList();

        var mostPopular = categories
            .OrderByDescending(c => c.ItemCount)
            .ThenBy(c => c.CategoryName, StringComparer.Ordinal)
            .ThenBy(c => c.CategoryId)
            .Take(5)
            .ToList();

        var leastPopular = categories
            .OrderBy(c => c.ItemCount)
            .ThenBy(c => c.CategoryName, StringComparer.Ordinal)
            .ThenBy(c => c.CategoryId)
            .Take(5)
            .ToList();

        return Ok(new { mostPopular, leastPopular });
    }

    /// <summary>
    /// Добавляет категорию, если цена положительна, а вид обработки допустим.
    /// </summary>
    /// <param name="request">Название, вид обработки и цена новой категории.</param>
    [HttpPost]
    public IActionResult Create([FromBody] CreateCategoryRequest request)
    {
        if(request.Price <= 0)
        {
            return BadRequest("Цена должна быть положительным числом");
        }
        if(!Enum.IsDefined(request.RecommendedCleaningType))
        {
            return BadRequest("Некорректный тип рекомендуемой очистки");
        }
        var newCategory = new ItemCategory
        {
            CategoryId = _data.ItemCategories.Select(c => c.CategoryId).DefaultIfEmpty(0).Max() + 1,
            CategoryName = request.CategoryName,
            RecommendedCleaningType = request.RecommendedCleaningType,
            Price = request.Price
        };
        _data.ItemCategories.Add(newCategory);
        return CreatedAtAction(nameof(GetById), new { id = newCategory.CategoryId }, newCategory);
    }
    /// <summary>
    /// Обновляет категорию после проверки цены и вида обработки.
    /// </summary>
    /// <param name="id">Идентификатор изменяемой категории.</param>
    /// <param name="request">Новые название, вид обработки и цена.</param>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        if(request.Price <= 0)
        {
            return BadRequest("Цена должна быть положительным числом");
        }
        if(!Enum.IsDefined(request.RecommendedCleaningType))
        {
            return BadRequest("Некорректный тип рекомендуемой очистки");
        }

        var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return NotFound();
        }
        category.CategoryName = request.CategoryName;
        category.RecommendedCleaningType = request.RecommendedCleaningType;
        category.Price = request.Price;
        return NoContent();
    }
    /// <summary>
    /// Удаляет категорию, если ни одно изделие на неё не ссылается.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой категории.</param>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == id);
        if (category == null)
        {
            return NotFound();
        }
        var isUsed = _data.Items.Any(item => item.Category.CategoryId == id);
        if (isUsed)
        {
            return Conflict("Нельзя удалить категорию, которая используется изделиями");
        }
        _data.ItemCategories.Remove(category);
        return NoContent();
    }
}
