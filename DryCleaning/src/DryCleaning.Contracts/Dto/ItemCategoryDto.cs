using DryCleaning.Domain.Enum;

namespace DryCleaning.Contracts.Dto;
/// <summary>
/// Данные категории изделий в ответе API
/// </summary>
public class ItemCategoryDto
{
    /// <summary>
    /// Уникальный идентификатор категории
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Имя (называние) категории
    /// </summary>
    public required string CategoryName { get; set; } 
    /// <summary>
    /// Рекомендованный тип очистки для категории изделий
    /// </summary>
    public required CleaningType RecommendedCleaningType { get; set; }
    /// <summary>
    ///Стоимость очистки
    /// </summary>
    public required decimal Price { get; set; }
}
