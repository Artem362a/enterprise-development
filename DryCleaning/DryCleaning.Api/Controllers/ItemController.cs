namespace DryCleaning.Api.Controllers;

using DryCleaning.Domain.Seeds;
using Microsoft.AspNetCore.Mvc;
using DryCleaning.Domain;
using DryCleaning.Api.Requests;

/// <summary>
/// Работает с изделиями, принимаемыми в химчистку.
/// </summary>
/// <param name="data">Изделия, категории и заказы, хранящиеся в памяти.</param>
[ApiController]
[Route("api/[controller]")]

public class ItemsController(TestData data) : ControllerBase
{
    private readonly TestData _data = data;

    /// <summary>
    /// Возвращает изделия вместе с их категориями.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        
        return Ok(_data.Items);
    }
    /// <summary>
    /// Возвращает изделие по идентификатору или 404, если оно не найдено.
    /// </summary>
    /// <param name="id">Идентификатор изделия.</param>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var item = _data.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }
    /// <summary>
    /// Добавляет изделие, относя его к существующей категории.
    /// </summary>
    /// <param name="request">Название, материал и идентификатор категории изделия.</param>
    [HttpPost]
    public IActionResult Create([FromBody] CreateItemRequest request)
    {
        var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == request.CategoryId);
        if (category == null)
        {
            return BadRequest("Категория не найдена");
        }
        var newItem = new Item
        {
            ItemId = _data.Items.Select(i => i.ItemId).DefaultIfEmpty(0).Max() + 1,
            Name = request.Name,
            Material = request.Material,
            Category = category
        };
        _data.Items.Add(newItem);
        return CreatedAtAction(nameof(GetById), new { id = newItem.ItemId }, newItem);
    }
    /// <summary>
    /// Меняет описание изделия и его категорию, если новая категория существует.
    /// </summary>
    /// <param name="id">Идентификатор изменяемого изделия.</param>
    /// <param name="request">Новые название, материал и идентификатор категории.</param>
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateItemRequest request)
    {
        var item = _data.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null)
        {
            return NotFound();
        }
        var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == request.CategoryId);
        if (category == null)
        {
            return BadRequest("Категория не найдена");
        }
        item.Name = request.Name;
        item.Material = request.Material;
        item.Category = category;
        return NoContent();
    }
    /// <summary>
    /// Удаляет изделие, если оно не используется ни в одном заказе.
    /// </summary>
    /// <param name="id">Идентификатор удаляемого изделия.</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = _data.Items.FirstOrDefault(i => i.ItemId == id);
        if (item == null)
        {
            return NotFound();
        }
        if(_data.Orders.Any(o => o.Item.ItemId == id))
        {
            return Conflict("Нельзя удалить предмет, который используется в заказе.");
        }
        _data.Items.Remove(item);
        return NoContent();
    }
}
