namespace DryCleaning.Domain;

/// <summary>
/// Категория изделий, принимаемых в химчистку.
/// Содержит уникальный идентификатор, название категории,
/// рекомендуемый вид обработки и стоимость обработки.
/// </summary>
public class ItemCategory
{
    public int CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public required CleaningType RecommendedCleaningType { get; set; }
    public required decimal Price { get; set; }
}
