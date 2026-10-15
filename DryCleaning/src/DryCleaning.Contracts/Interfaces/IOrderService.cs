using DryCleaning.Contracts.Dto;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Описывает операции с заказами химчистки
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Возвращает все заказы
    /// </summary>
    /// <returns>Список заказов</returns>
    public Task<IReadOnlyList<OrderDto>> GetAllAsync();
    /// <summary>
    /// Возвращает заказы клиента
    /// </summary>
    /// <param name="clientId">Идентификатор клиента</param>
    /// <returns>Список заказов или null, если клиент не найден</returns>
    public Task<IReadOnlyList<OrderDto>?> GetByClientIdAsync(int clientId);

    /// <summary>
    /// Возвращает заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>Заказ, или null, если заказ не найден</returns>
    public Task<OrderDto?> GetByIdAsync(int id);
    /// <summary>
    /// Создает заказ 
    /// </summary>
    /// <param name="order">Данные заказа</param>
    /// <returns>Возвращает заказ с уникальным идентификатором</returns>
    public Task<OrderDto> AddAsync(OrderCreateDto order);
    /// <summary>
    /// Обновляет данные заказа по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <param name="order">Новые данные для заказа</param>
    /// <returns>True - если заказ обновлен, False - иначе</returns>
    public Task<bool> UpdateAsync(int id, OrderEditDto order);
    /// <summary>
    /// Удаляет заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>True - если заказ удален, False - иначе</returns>
    public Task<bool> DeleteAsync(int id);
    /// <summary>
    /// Возвращает список заказов, находящихся в обработке
    /// </summary>
    /// <returns>Список заказов</returns>
    public Task<IReadOnlyList<OrderDto>> GetInProgressAsync();
}
