namespace DryCleaning.Contracts.Exceptions;

/// <summary>
/// Ошибка удаления категории, которая используется изделиями
/// </summary>
/// <param name="categoryId">Идентификатор категории</param>
public class CategoryHasItemsException(int categoryId)
    : InvalidOperationException("Нельзя удалить категорию, которая используется изделиями")
{
    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public int CategoryId { get; } = categoryId;
}
