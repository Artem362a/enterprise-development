using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DryCleaning.Api.Controllers;

/// <summary>Работает с клиентами и аналитикой по их заказам.</summary>
/// <param name="service">Сервис клиентов.</param>
/// <param name="logger">Журнал ошибок контроллера.</param>
[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientService service, ILogger<ClientsController> logger) : ControllerBase
{
    private readonly IClientService _service = service;
    private readonly ILogger<ClientsController> _logger = logger;

    /// <summary>Возвращает всех клиентов химчистки.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<ClientDto>>> GetAll()
    {
        try
        {
            var clients = await _service.GetAllAsync();
            return Ok(clients);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetAll));
        }
    }

    /// <summary>Возвращает клиента по идентификатору.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        try
        {
            var client = await _service.GetByIdAsync(id);
            if (client is null)
            {
                _logger.LogWarning("Клиент {ClientId} не найден", id);
                return NotFound();
            }

            return Ok(client);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetById));
        }
    }

    /// <summary>Возвращает пять клиентов с наибольшим числом неотменённых заказов за период.</summary>
    /// <param name="start">Начало периода, включаемое в расчёт.</param>
    /// <param name="end">Конец периода, не включаемый в расчёт.</param>
    [HttpGet("top")]
    [ProducesResponseType(typeof(IReadOnlyList<TopClientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<TopClientDto>>> GetTopClients([FromQuery, BindRequired] DateTime start, [FromQuery, BindRequired] DateTime end)
    {
        try
        {
            var clients = await _service.GetTopClientsAsync(start, end);
            return Ok(clients);
        }
        catch (ArgumentException exception) when (start >= end)
        {
            _logger.LogWarning(exception, "Некорректный период: {Start} — {End}", start, end);
            ModelState.AddModelError(nameof(start), "Начало периода должно быть раньше конца.");
            return ValidationProblem(ModelState);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetTopClients));
        }
    }

    /// <summary>Возвращает клиента с наибольшей суммой выданных заказов по текущим тарифам.</summary>
    [HttpGet("top-spender")]
    [ProducesResponseType(typeof(TopSpenderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TopSpenderDto>> GetTopSpender()
    {
        try
        {
            var client = await _service.GetTopSpenderAsync();
            if (client is null)
            {
                _logger.LogWarning("Не найдены клиенты с выданными заказами");
                return NotFound();
            }

            return Ok(client);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetTopSpender));
        }
    }

    /// <summary>Возвращает клиентов с максимальной фактической длительностью завершённого заказа.</summary>
    [HttpGet("longest-processing")]
    [ProducesResponseType(typeof(LongestProcessingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LongestProcessingDto>> GetLongestProcessingOrder()
    {
        try
        {
            var result = await _service.GetLongestProcessingOrderAsync();
            if (result is null)
            {
                _logger.LogWarning("Не найдены завершённые заказы с датой завершения");
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(GetLongestProcessingOrder));
        }
    }

    /// <summary>Создаёт клиента химчистки.</summary>
    /// <param name="request">Полное имя и телефон нового клиента.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClientDto>> Create([FromBody] ClientEditDto request)
    {
        try
        {
            var client = await _service.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = client.ClientId }, client);
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Create));
        }
    }

    /// <summary>Меняет полное имя и телефон клиента.</summary>
    /// <param name="id">Идентификатор изменяемого клиента.</param>
    /// <param name="request">Новые данные клиента.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] ClientEditDto request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated)
            {
                _logger.LogWarning("Клиент {ClientId} для обновления не найден", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Update));
        }
    }

    /// <summary>Удаляет клиента, если с ним не связаны заказы.</summary>
    /// <param name="id">Идентификатор удаляемого клиента.</param>
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
                _logger.LogWarning("Клиент {ClientId} для удаления не найден", id);
                return NotFound();
            }

            return NoContent();
        }
        catch (ClientHasOrdersException exception)
        {
            _logger.LogWarning(exception, "Нельзя удалить клиента {ClientId} с заказами", id);
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Удаление клиента невозможно.",
                Detail = "У клиента есть заказы."
            });
        }
        catch (Exception exception)
        {
            return ServerError(exception, nameof(Delete));
        }
    }

    /// <summary>Записывает ошибку в журнал и возвращает ответ без внутренних деталей.</summary>
    /// <param name="exception">Возникшая ошибка.</param>
    /// <param name="operation">Название операции.</param>
    /// <returns>Ответ с кодом 500.</returns>
    private ObjectResult ServerError(Exception exception, string operation)
    {
        _logger.LogError(exception, "Ошибка при выполнении операции {Operation}", operation);
        return Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Не удалось обработать запрос.");
    }
}
