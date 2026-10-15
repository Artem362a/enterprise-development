namespace DryCleaning.Contracts.Dto;

/// <summary>
/// Наиболее и наименее популярные категории за период
/// </summary>
public class CategoryPopularityDto
{
    /// <summary>
    /// Наиболее популярные категории
    /// </summary>
    public required IReadOnlyList<CategoryPopularityEntryDto> MostPopular { get; set; }

    /// <summary>
    /// Наименее популярные категории
    /// </summary>
    public required IReadOnlyList<CategoryPopularityEntryDto> LeastPopular { get; set; }
}
