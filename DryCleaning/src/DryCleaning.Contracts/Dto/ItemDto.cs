namespace DryCleaning.Contracts.Dto;
/// <summary>
/// Данные изделия в ответе API
/// </summary>
public class ItemDto
{
    /// <summary>
    /// Уникальный идентификатор изделия
    /// </summary>
    public int ItemId { get; set; }
    /// <summary>
    /// Имя (название) изделия
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Материал изделия
    /// </summary>
    public required string Material { get; set; }
    /// <summary>
    /// Категория изделия
    /// </summary>
    public required ItemCategoryDto Category { get; set; }

}
