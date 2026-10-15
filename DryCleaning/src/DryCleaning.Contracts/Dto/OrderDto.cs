using DryCleaning.Domain.Enum;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// Данные заказа в ответе API
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Уникальный идентификатор заказа
    /// </summary>
    public required int Id { get; set; }
    /// <summary>
    /// Заказчик (клиент)
    /// </summary>
    public required ClientDto Client { get; set; } 
    /// <summary>
    /// Изделие для химчистки
    /// </summary>
    public required ItemDto Item { get; set; }
    /// <summary>
    /// Статус заказа
    /// </summary>
    public required OrderStatus Status { get; set; }
    /// <summary>
    /// Количество дней, выделенных на заказ
    /// </summary>
    public int ProcessingDays { get; set; }
    /// <summary>
    /// Дата подачи изделия в химчистку
    /// </summary>
    public required DateTime AcceptDate { get; set; }
    /// <summary>
    /// Дата выполнения заказа 
    /// </summary>
    public required DateTime? CompletedDate { get; set; }

}
