namespace DryCleaning.Domain;

public class Item
{
    public int ItemId { get; set; }
    public required string Name { get; set; }
    public required string Material { get; set; }
    public required ItemCategory Category { get; set; }
}
