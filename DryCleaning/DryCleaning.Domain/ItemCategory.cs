using DryCleaning.Domain.Enum;

namespace DryCleaning.Domain;

/// <summary>
/// Категория изделий, принимаемых в химчистку.
/// Содержит уникальный идентификатор, название категории,
/// рекомендуемый вид обработки и стоимость обработки.
/// </summary>
public class ItemCategory
{
    /// <summary>
    /// Уникальный идентификатор категории
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Название категории
    /// </summary>
    public required string CategoryName { get; set; }
    /// <summary>
    /// Рекомендуемый вид обработки
    /// </summary>
    public required CleaningType RecommendedCleaningType { get; set; }
    /// <summary>
    /// Стоимость обработки
    /// </summary>
    public required decimal Price { get; set; }
}
