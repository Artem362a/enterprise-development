namespace DryCleaning.Api.Requests;

/// <summary>
/// Данные для оформления нового заказа на обработку изделия.
/// </summary>
public class CreateOrderRequest
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
    /// Плановый срок обработки в днях
    /// </summary>
    public required int ProcessingDays { get; set; }


}
