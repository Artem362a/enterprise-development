namespace DryCleaning.Domain;

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
