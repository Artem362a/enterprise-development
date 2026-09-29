namespace DryCleaning.Api.Requests;

/// <summary>
/// Новое описание изделия и его принадлежность к категории.
/// </summary>
public class UpdateItemRequest
{
    /// <summary>
    /// Название изделия после обновления
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Материал, указанный для изделия после обновления
    /// </summary>
    public required string Material { get; set; }
    /// <summary>
    /// Идентификатор категории изделия
    /// </summary>
    public required int CategoryId { get; set; }
}
