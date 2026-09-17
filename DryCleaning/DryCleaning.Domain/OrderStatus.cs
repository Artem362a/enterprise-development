namespace DryCleaning.Domain;

/// <summary>
/// Статус заказа в химчистке.
/// Может быть одним из следующих значений: Принят, В обработке, Выполнен, Выдан, Отменён.
/// </summary>
public enum OrderStatus
{
    Accepted = 0,
    InProgress = 1,
    Completed = 2,
    Issued = 3,
    Cancelled = 4
}
