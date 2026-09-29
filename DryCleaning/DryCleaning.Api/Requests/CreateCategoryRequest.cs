using DryCleaning.Domain.Enum;

namespace DryCleaning.Api.Requests;

/// <summary>
/// Данные для добавления категории изделий
/// </summary>
public class CreateCategoryRequest
{
    /// <summary>
    /// Название, под которым категория отображается
    /// </summary>
    public required string CategoryName { get; set; }
    /// <summary>
    /// Допустимый вид обработки, рекомендуемый для изделий этой категории.
    /// </summary>
    public required CleaningType RecommendedCleaningType { get; set; }
    /// <summary>
    /// Стоимость обработки изделия этой категории
    /// </summary>
    public required decimal Price { get; set; }
}
