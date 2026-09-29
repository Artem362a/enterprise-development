namespace DryCleaning.Api.Requests;

using DryCleaning.Domain.Enum;

/// <summary>
/// Новые данные существующей категории изделий.
/// </summary>
public class UpdateCategoryRequest
{
    /// <summary>
    /// Название категории после обновления.
    /// </summary>
    public required string CategoryName { get; set; }
    /// <summary>
    /// Допустимый вид обработки, рекомендуемый после обновления.
    /// </summary>
    public required CleaningType RecommendedCleaningType { get; set; }
    /// <summary>
    /// Новая стоимость обработки
    /// </summary>
    public required decimal Price { get; set; }
}
