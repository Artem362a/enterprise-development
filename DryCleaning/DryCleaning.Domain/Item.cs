namespace DryCleaning.Domain;

/// <summary>
/// Класс, описывающий изделие, принимаемое в химчистку.
/// Содержит уникальный идентификатор, название вещи, материал и категорию
/// </summary>
public class Item
{
    /// <summary>
    /// Уникальный идентификатор изделия
    /// </summary>
    public int ItemId { get; set; }
    /// <summary>
    /// Название изделия
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Материал изделия
    /// </summary>
    public required string Material { get; set; }
    /// <summary>
    /// Категория изделия
    /// </summary>  
    public required ItemCategory Category { get; set; }
}
