namespace DryCleaning.Contracts.Exceptions;

/// <summary>
/// Ошибка удаления клиента, с которым связаны заказы
/// </summary>
/// <param name="clientId">Уникальный идентификатор клиента</param>
public class ClientHasOrdersException(int clientId)
    : InvalidOperationException("Нельзя удалить клиента, у которого есть заказы")
{
    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    public int ClientId { get; } = clientId;
}
