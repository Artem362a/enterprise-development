namespace DryCleaning.Domain;

public class ItemCategory
{
    public int CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public required CleaningType RecommendedCleaningType { get; set; }
    public required decimal Price { get; set; }
}
