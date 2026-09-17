namespace DryCleaning.Domain;

/// <summary>
/// Класс, описывающий изделие, принимаемое в химчистку.
/// Содержит уникальный идентификатор, название вещи, материал и категорию
/// </summary>
public class Item
{
    public int ItemId { get; set; }
    public required string Name { get; set; }
    public required string Material { get; set; }
    public required ItemCategory Category { get; set; }
}
