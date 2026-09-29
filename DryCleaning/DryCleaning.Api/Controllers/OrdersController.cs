namespace DryCleaning.Api.Controllers;

using DryCleaning.Domain.Seeds;
using Microsoft.AspNetCore.Mvc;
using DryCleaning.Api.Requests;
using DryCleaning.Domain;
using DryCleaning.Domain.Enum;

/// <summary>
/// Работает с заказами химчистки и выборкой заказов в обработке.
/// </summary>
/// <param name="data">Заказы, клиенты и изделия, хранящиеся в памяти.</param>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(TestData data) : ControllerBase
{
    private readonly TestData _data = data;
    /// <summary>
    /// Возвращает все заказы
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_data.Orders);
    }
    /// <summary>
    /// Возвращает заказ по идентификатору или 404, если он не найден.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var order = _data.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    /// <summary>
    /// Возвращает заказы в обработке по дате приёма, затем по идентификатору.
    /// </summary>
    [HttpGet("in-progress")]
    public IActionResult GetInProgress()
    {
        var inProgressOrders = _data.Orders.Where(o => o.Status == OrderStatus.InProgress)
        .OrderBy(o => o.AcceptDate)
        .ThenBy(o => o.Id)
        .ToList();
        return Ok(inProgressOrders);
    }

    /// <summary>
    /// Создаёт заказ для существующих клиента и изделия, фиксируя время приёма в UTC.
    /// </summary>
    /// <param name="request">Клиент, изделие и плановый срок обработки.</param>
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
       var client = _data.Clients.FirstOrDefault(c => c.ClientId == request.ClientId);
        if (client == null)
        {
            return BadRequest("Клиент не найден");
        }
        var item = _data.Items.FirstOrDefault(i => i.ItemId == request.ItemId);
        if (item == null)
        {
            return BadRequest("Вещь не найдена");
        }
        if (request.ProcessingDays <= 0)
            return BadRequest("Срок обработки заказа должен быть больше 0 дней");

        var newOrder = new Order
        {
            Id = _data.Orders.Select(o => o.Id).DefaultIfEmpty(0).Max() + 1,
            Client = client,
            Item = item,
            ProcessingDays = request.ProcessingDays,
            AcceptDate = DateTime.UtcNow,
            Status = OrderStatus.Accepted
        };
        _data.Orders.Add(newOrder);
        return CreatedAtAction(nameof(GetById), new { id = newOrder.Id }, newOrder);
    }
    /// <summary>
    /// Обновляет заказ, проверяя допустимость статуса и даты завершения.
    /// </summary>
    /// <param name="id">Идентификатор изменяемого заказа.</param>
    /// <param name="request">Новые данные заказа.</param>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateOrderRequest request)
    {
        var order = _data.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        if (request.ProcessingDays <= 0)
            return BadRequest("Срок обработки заказа должен быть больше 0 дней");

        if (!Enum.IsDefined(request.Status))
            return BadRequest("Недопустимый статус заказа.");

        var finished = request.Status is OrderStatus.Completed or OrderStatus.Issued;

        if (finished && request.CompletedDate is null)
            return BadRequest("Для завершённого или выданного заказа нужна дата завершения.");

        if (!finished && request.CompletedDate is not null)
            return BadRequest("У незавершённого заказа не должно быть даты завершения.");

        DateTime? completedUtc = null;

        if (request.CompletedDate is DateTime completed)
        {
            if (completed.Kind == DateTimeKind.Unspecified)
                return BadRequest("Укажите часовой пояс даты завершения: Z или смещение.");

            completedUtc = completed.ToUniversalTime();
        }

        var acceptedUtc = order.AcceptDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(order.AcceptDate, DateTimeKind.Utc)
            : order.AcceptDate.ToUniversalTime();

        if (completedUtc.HasValue && completedUtc.Value < acceptedUtc)
            return BadRequest("Дата завершения не может быть раньше даты приёма.");

        var client = _data.Clients.FirstOrDefault(c => c.ClientId == request.ClientId);
        if (client == null)
        {
            return BadRequest("Клиент не найден");
        }
        var item = _data.Items.FirstOrDefault(i => i.ItemId == request.ItemId);
        if (item == null)
        {
            return BadRequest("Вещь не найдена");
        }
        order.Client = client;
        order.Item = item;
        order.Status = request.Status;
        order.ProcessingDays = request.ProcessingDays;
        order.CompletedDate = completedUtc;
        return NoContent();
    }
    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор удаляемого заказа.</param>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var order = _data.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        _data.Orders.Remove(order);
        return NoContent();
    }
}
