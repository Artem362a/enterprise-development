namespace DryCleaning.Domain.Interfaces;

/// <summary>Описывает доступ к заказам химчистки.</summary>
public interface IOrderRepository
{
    /// <summary>Возвращает все заказы.</summary>
    /// <returns>Список заказов.</returns>
    public Task<IReadOnlyList<Order>> GetAllAsync();

    /// <summary>
    /// Возвращает заказы клиента
    /// </summary>
    /// <param name="clientId">Идентификатор клиента</param>
    /// <returns>Список заказов</returns>
    public Task<IReadOnlyList<Order>> GetByClientIdAsync(int clientId);

    /// <summary>Возвращает заказ по идентификатору.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Заказ или null, если он не найден.</returns>
    public Task<Order?> GetByIdAsync(int id);

    /// <summary>Добавляет заказ.</summary>
    /// <param name="order">Новый заказ.</param>
    /// <returns>Заказ с назначенным идентификатором.</returns>
    public Task<Order> AddAsync(Order order);

    /// <summary>Обновляет данные заказа.</summary>
    /// <param name="order">Заказ с идентификатором и новыми данными.</param>
    /// <returns>true — заказ обновлён; false — не найден.</returns>
    public Task<bool> UpdateAsync(Order order);

    /// <summary>Удаляет заказ по идентификатору.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>true — заказ удалён; false — не найден.</returns>
    public Task<bool> DeleteAsync(int id);

    /// <summary>Проверяет наличие заказов у клиента.</summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <returns>true — заказы есть; false — заказов нет.</returns>
    public Task<bool> HasOrdersForClientAsync(int clientId);

    /// <summary>Проверяет наличие заказов с изделием.</summary>
    /// <param name="itemId">Идентификатор изделия.</param>
    /// <returns>true — заказы есть; false — заказов нет.</returns>
    public Task<bool> HasOrdersForItemAsync(int itemId);
}
