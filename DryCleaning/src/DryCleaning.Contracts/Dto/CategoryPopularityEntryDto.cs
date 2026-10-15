namespace DryCleaning.Contracts.Dto;

/// <summary>
/// Категория и количество принятых изделий за период
/// </summary>
public class CategoryPopularityEntryDto
{
    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Название категории
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// Количество изделий по неотменённым заказам за период
    /// </summary>
    public int ItemCount { get; set; }
}
