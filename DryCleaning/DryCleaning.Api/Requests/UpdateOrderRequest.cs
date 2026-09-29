using DryCleaning.Domain.Enum;

namespace DryCleaning.Api.Requests;

/// <summary>
/// Новые данные существующего заказа
/// </summary>
public class UpdateOrderRequest
{
    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public required int ClientId { get; set; }
    /// <summary>
    /// Идентификатор изделия
    /// </summary>
    public required int ItemId { get; set; }
    /// <summary>
    /// Статус заказа
    /// </summary>
    public required OrderStatus Status { get; set; }
    /// <summary>
    /// Плановый срок обработки в днях
    /// </summary>
    public required int ProcessingDays { get; set; }
    /// <summary>
    /// Фактическая дата завершения
    /// Обязательна для завершённого и выданного заказа, иначе должна быть пустой.
    /// </summary>
    public DateTime? CompletedDate { get; set; }
}
