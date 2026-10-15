using DryCleaning.Domain;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Domain.Seeds;

namespace DryCleaning.Infrastructure.InMemory.Repositories;

/// <summary>Репозиторий заказов в памяти.</summary>
/// <param name="data">Данные химчистки.</param>
public class OrderRepository(TestData data) : IOrderRepository
{
    private readonly TestData _data = data;

    /// <summary>Возвращает все заказы.</summary>
    /// <returns>Список заказов.</returns>
    public Task<IReadOnlyList<Order>> GetAllAsync()
    {
        lock (_data.Orders)
        {
            return Task.FromResult<IReadOnlyList<Order>>(_data.Orders.ToList());
        }
    }

    /// <summary>
    /// Возвращает заказы клиента
    /// </summary>
    /// <param name="clientId">Идентификатор клиента</param>
    /// <returns>Список заказов</returns>
    public Task<IReadOnlyList<Order>> GetByClientIdAsync(int clientId)
    {
        lock (_data.Orders)
        {
            return Task.FromResult<IReadOnlyList<Order>>(_data.Orders
                .Where(order => order.Client.ClientId == clientId)
                .ToList());
        }
    }

    /// <summary>Возвращает заказ по идентификатору.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Заказ или null, если он не найден.</returns>
    public Task<Order?> GetByIdAsync(int id)
    {
        lock (_data.Orders)
        {
            return Task.FromResult(_data.Orders.FirstOrDefault(order => order.Id == id));
        }
    }

    /// <summary>Добавляет заказ.</summary>
    /// <param name="order">Новый заказ.</param>
    /// <returns>Заказ с назначенным идентификатором.</returns>
    public Task<Order> AddAsync(Order order)
    {
        lock (_data.Orders)
        {
            order.Id = _data.Orders
                .Select(existing => existing.Id)
                .DefaultIfEmpty(0)
                .Max() + 1;
            _data.Orders.Add(order);
        }

        return Task.FromResult(order);
    }

    /// <summary>Обновляет данные заказа.</summary>
    /// <param name="order">Заказ с идентификатором и новыми данными.</param>
    /// <returns>true — заказ обновлён; false — не найден.</returns>
    public Task<bool> UpdateAsync(Order order)
    {
        lock (_data.Orders)
        {
            var existingOrder = _data.Orders.FirstOrDefault(existing => existing.Id == order.Id);
            if (existingOrder is null)
            {
                return Task.FromResult(false);
            }

            existingOrder.Client = order.Client;
            existingOrder.Item = order.Item;
            existingOrder.Status = order.Status;
            existingOrder.ProcessingDays = order.ProcessingDays;
            existingOrder.AcceptDate = order.AcceptDate;
            existingOrder.CompletedDate = order.CompletedDate;
            return Task.FromResult(true);
        }
    }

    /// <summary>Удаляет заказ по идентификатору.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>true — заказ удалён; false — не найден.</returns>
    public Task<bool> DeleteAsync(int id)
    {
        lock (_data.Orders)
        {
            var order = _data.Orders.FirstOrDefault(existing => existing.Id == id);
            if (order is null)
            {
                return Task.FromResult(false);
            }

            _data.Orders.Remove(order);
            return Task.FromResult(true);
        }
    }

    /// <summary>Проверяет наличие заказов у клиента.</summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <returns>true — заказы есть; false — заказов нет.</returns>
    public Task<bool> HasOrdersForClientAsync(int clientId)
    {
        lock (_data.Orders)
        {
            return Task.FromResult(_data.Orders.Any(order => order.Client.ClientId == clientId));
        }
    }

    /// <summary>Проверяет наличие заказов с изделием.</summary>
    /// <param name="itemId">Идентификатор изделия.</param>
    /// <returns>true — заказы есть; false — заказов нет.</returns>
    public Task<bool> HasOrdersForItemAsync(int itemId)
    {
        lock (_data.Orders)
        {
            return Task.FromResult(_data.Orders.Any(order => order.Item.ItemId == itemId));
        }
    }
}
