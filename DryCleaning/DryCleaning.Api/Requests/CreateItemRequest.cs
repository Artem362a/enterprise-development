namespace DryCleaning.Api.Requests;

/// <summary>
/// Данные изделия, принимаемого в химчистку.
/// </summary>
public class CreateItemRequest
{
    /// <summary>
    /// Название вещи
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Материал, указанный при приёме вещи
    /// </summary>
    public required string Material { get; set; }
    /// <summary>
    /// Идентификатор существующей категории изделия
    /// </summary>
    public required int CategoryId { get; set; }
}
