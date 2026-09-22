namespace DryCleaning.Domain;

/// <summary>
/// Статус заказа в химчистке.
/// Может быть одним из следующих значений: Принят, В обработке, Выполнен, Выдан, Отменён.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Заказ принят
    /// </summary>
    Accepted = 0,
    /// <summary>
    /// Заказ находится в обработке
    /// </summary>
    InProgress = 1,
    /// <summary>
    /// Заказ выполнен
    /// </summary>
    Completed = 2,
    /// <summary>
    /// Заказ выдан
    /// </summary>
    Issued = 3,
    /// <summary>
    /// Заказ отменен
    /// </summary>
    Cancelled = 4
}
