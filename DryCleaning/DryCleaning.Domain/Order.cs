namespace DryCleaning.Domain;

/// <summary>
/// Заказ на обработку изделия в химчистке.
/// Содержит уникальный идентификатор, клиента, изделие, статус,
/// срок обработки, дату приёма и фактическую дату завершения.
/// Дата завершения может быть null, если заказ ещё не завершён.
/// </summary>
public class Order
{
    public int Id { get; set; }
    public required Client Client { get; set; }
    public required Item Item { get; set; }
    public OrderStatus Status { get; set; }
    public int ProcessingDays { get; set; }
    public required DateTime AcceptDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}
