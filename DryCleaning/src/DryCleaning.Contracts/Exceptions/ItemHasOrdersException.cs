namespace DryCleaning.Contracts.Exceptions;

/// <summary>
/// Ошибка удаления изделия, с которым связаны заказы
/// </summary>
/// <param name="itemId">Уникальный идентификатор изделия</param>
public class ItemHasOrdersException(int itemId)
    : InvalidOperationException("Нельзя удалить изделие, с которым связаны заказы")
{
    /// <summary>
    /// Уникальный идентификатор изделия
    /// </summary>
    public int ItemId { get; } = itemId;
}
