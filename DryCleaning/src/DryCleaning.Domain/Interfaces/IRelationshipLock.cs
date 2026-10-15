namespace DryCleaning.Domain.Interfaces;

/// <summary>
/// Защита операций со связанными данными
/// </summary>
public interface IRelationshipLock
{
    /// <summary>
    /// Ожидает завершения текущей операции
    /// </summary>
    /// <returns>Объект для освобождения блокировки после операции</returns>
    public Task<IDisposable> AcquireAsync();
}
