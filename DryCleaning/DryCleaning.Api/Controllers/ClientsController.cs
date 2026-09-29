namespace DryCleaning.Api.Controllers;

using DryCleaning.Domain.Seeds;
using Microsoft.AspNetCore.Mvc;
using DryCleaning.Api.Requests;
using DryCleaning.Domain;
using DryCleaning.Domain.Enum;

/// <summary>
/// Работает с клиентами и аналитическими выборками по их заказам.
/// </summary>
/// <param name="data">Клиенты и заказы, хранящиеся в памяти.</param>
[ApiController]
[Route("api/[controller]")]
public class ClientsController(TestData data) : ControllerBase
{
    private readonly TestData _data = data;

    /// <summary>
    /// Возвращает всех клиентов химчистки.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_data.Clients);
    }
    /// <summary>
    /// Возвращает клиента по идентификатору или 404, если он не найден.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var client = _data.Clients.FirstOrDefault(c => c.ClientId == id);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }

    /// <summary>
    /// Находит пять клиентов с наибольшим числом неотменённых заказов за период.
    /// </summary>
    /// <param name="start">Начало периода, включаемое в расчёт.</param>
    /// <param name="end">Конец периода, не включаемый в расчёт.</param>
    [HttpGet("top")]
    public IActionResult GetTopClients([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        if (start >= end)
        {
            return BadRequest("Начало периода должно быть раньше конца.");
        }

        var topClients = _data.Orders
            .Where(o => o.AcceptDate >= start && o.AcceptDate < end)
            .Where(o => o.Status != OrderStatus.Cancelled)
            .GroupBy(o => o.Client.ClientId)
            .Select(g => new
            {
                Client = g.First().Client,
                ItemCount = g.Count()
            })
            .OrderByDescending(c => c.ItemCount)
            .ThenBy(c => c.Client.FullName, StringComparer.Ordinal)
            .ThenBy(c => c.Client.ClientId)
            .Take(5)
            .Select(c => new
            {
                c.Client.ClientId,
                c.Client.FullName,
                c.Client.PhoneNumber,
                c.ItemCount
            })
            .ToList();

        return Ok(topClients);
    }

    /// <summary>
    /// Находит клиента с наибольшей суммой выданных заказов по текущим тарифам категорий.
    /// </summary>
    [HttpGet("top-spender")]
    public IActionResult GetTopSpender()
    {
        var topSpender = _data.Orders
            .Where(o => o.Status == OrderStatus.Issued)
            .GroupBy(o => o.Client.ClientId)
            .Select(g => new
            {
                g.First().Client,
                TotalSpent = g.Sum(o => o.Item.Category.Price)
            })
            .OrderByDescending(c => c.TotalSpent)
            .ThenBy(c => c.Client.FullName, StringComparer.Ordinal)
            .ThenBy(c => c.Client.ClientId)
            .FirstOrDefault();

        if (topSpender == null)
            return NotFound();

        return Ok(topSpender);
    }

    /// <summary>
    /// Возвращает клиентов с максимальной фактической длительностью завершённого заказа.
    /// Каждый клиент входит в результат один раз.
    /// </summary>
    [HttpGet("longest-processing")]
    public IActionResult GetLongestProcessingOrder()
    {
        var completedOrders = _data.Orders
            .Where(o => (o.Status == OrderStatus.Completed ||
                         o.Status == OrderStatus.Issued) &&
                        o.CompletedDate.HasValue)
            .Select(o => new
            {
                o.Client,
                Duration = o.CompletedDate!.Value - o.AcceptDate
            })
            .ToList();

        if (completedOrders.Count == 0)
            return NotFound();

        var maxDuration = completedOrders.Max(o => o.Duration);

        var clients = completedOrders
            .Where(o => o.Duration == maxDuration)
            .Select(o => o.Client)
            .DistinctBy(c => c.ClientId)
            .OrderBy(c => c.FullName, StringComparer.Ordinal)
            .ThenBy(c => c.ClientId)
            .ToList();

        return Ok(new
        {
            durationDays = maxDuration.TotalDays,
            clients
        });
    }

    /// <summary>
    /// Добавляет клиента в справочник.
    /// </summary>
    /// <param name="request">Полное имя и телефон нового клиента.</param>
    [HttpPost]
    public IActionResult Create([FromBody] CreateClientRequest request)
    { 
        var newClient = new Client
        {
            ClientId = _data.Clients.Select(c => c.ClientId).DefaultIfEmpty(0).Max() + 1,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };
        _data.Clients.Add(newClient);
        return CreatedAtAction(nameof(GetById), new { id = newClient.ClientId }, newClient);
    }
    /// <summary>
    /// Меняет полное имя и телефон существующего клиента.
    /// </summary>
    /// <param name="id">Идентификатор изменяемого клиента.</param>
    /// <param name="request">Новое полное имя и телефон.</param>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateClientRequest request)
    {
        var client = _data.Clients.FirstOrDefault(c => c.ClientId == id);
        if (client == null)
        {
            return NotFound();
        }
        client.FullName = request.FullName;
        client.PhoneNumber = request.PhoneNumber;
        return NoContent();
    }
    /// <summary>
    /// Удаляет клиента, только если с ним не связаны заказы.
    /// </summary>
    /// <param name="id">Идентификатор удаляемого клиента.</param>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var client = _data.Clients.FirstOrDefault(c => c.ClientId == id);
        if (client == null)
        {
            return NotFound();
        }
        if(_data.Orders.Any(order => order.Client.ClientId == id))
        {
            return Conflict("Нельзя удалить клиента, у которого есть заказы.");
        }
        _data.Clients.Remove(client);
        return NoContent();
    }
}
