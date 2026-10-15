using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain;
using DryCleaning.Domain.Enum;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>
/// Сервис заказов химчистки
/// </summary>
/// <param name="orderRepository">Репозиторий заказов</param>
/// <param name="clientRepository">Репозиторий клиентов</param>
/// <param name="itemRepository">Репозиторий изделий</param>
/// <param name="relationshipLock">Защита операций со связанными данными</param>
public class OrderService( IOrderRepository orderRepository, IClientRepository clientRepository, IItemRepository itemRepository, IRelationshipLock relationshipLock) : IOrderService
{
    private readonly IRelationshipLock _relationshipLock = relationshipLock;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IItemRepository _itemRepository = itemRepository;

    /// <summary>
    /// Возвращает все заказы
    /// </summary>
    /// <returns>Список заказов</returns>
    public async Task<IReadOnlyList<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return [.. orders.Select(MapToDto)];
    }

    /// <summary>
    /// Возвращает заказы клиента
    /// </summary>
    /// <param name="clientId">Идентификатор клиента</param>
    /// <returns>Список заказов или null, если клиент не найден</returns>
    public async Task<IReadOnlyList<OrderDto>?> GetByClientIdAsync(int clientId)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client is null)
            return null;

        var orders = await _orderRepository.GetByClientIdAsync(clientId);
        return [.. orders.Select(MapToDto)];
    }

    /// <summary>
    /// Возвращает заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>Заказ или null, если он не найден</returns>
    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order is null ? null : MapToDto(order);
    }

    /// <summary>
    /// Создает заказ для существующих клиента и изделия
    /// </summary>
    /// <param name="order">Данные нового заказа</param>
    /// <returns>Созданный заказ с присвоенным идентификатором</returns>
    /// <exception cref="ValidationException">Некорректные данные заказа, клиент или изделие не найдены</exception>
    public async Task<OrderDto> AddAsync(OrderCreateDto order)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        ValidateOrder(order);

        var client = await _clientRepository.GetByIdAsync(order.ClientId)
            ?? throw new ValidationException("Клиент не найден");
        var item = await _itemRepository.GetByIdAsync(order.ItemId)
            ?? throw new ValidationException("Изделие не найдено");

        var newOrder = new Order
        {
            Client = client,
            Item = item,
            ProcessingDays = order.ProcessingDays,
            Status = OrderStatus.Accepted,
            AcceptDate = DateTime.UtcNow,
            CompletedDate = null
        };

        var createdOrder = await _orderRepository.AddAsync(newOrder);
        return MapToDto(createdOrder);
    }

    /// <summary>
    /// Обновляет данные заказа
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <param name="order">Новые данные заказа</param>
    /// <returns>True - если заказ обновлен, False - заказ не найден</returns>
    /// <exception cref="ValidationException">Некорректные данные или дата завершения, клиент или изделие не найдены</exception>
    public async Task<bool> UpdateAsync(int id, OrderEditDto order)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        ValidateOrder(order);

        var existingOrder = await _orderRepository.GetByIdAsync(id);
        if (existingOrder is null)
            return false;

        var completedDate = ValidateCompletedDate(order, existingOrder.AcceptDate);
        var client = await _clientRepository.GetByIdAsync(order.ClientId)
            ?? throw new ValidationException("Клиент не найден");
        var item = await _itemRepository.GetByIdAsync(order.ItemId)
            ?? throw new ValidationException("Изделие не найдено");

        var updatedOrder = new Order
        {
            Id = id,
            Client = client,
            Item = item,
            Status = order.Status,
            ProcessingDays = order.ProcessingDays,
            AcceptDate = existingOrder.AcceptDate,
            CompletedDate = completedDate
        };

        return await _orderRepository.UpdateAsync(updatedOrder);
    }

    /// <summary>
    /// Удаляет заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>True - если заказ удален, False - заказ не найден</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        return await _orderRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Возвращает заказы в обработке, отсортированные по дате приема, затем по идентификатору
    /// </summary>
    /// <returns>Список заказов в обработке</returns>
    public async Task<IReadOnlyList<OrderDto>> GetInProgressAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return [.. orders
            .Where(order => order.Status == OrderStatus.InProgress)
            .OrderBy(order => order.AcceptDate)
            .ThenBy(order => order.Id)
            .Select(MapToDto)];
    }

    /// <summary>
    /// Преобразует заказ в DTO
    /// </summary>
    /// <param name="order">Заказ</param>
    /// <returns>Данные заказа для ответа API</returns>
    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            Client = new ClientDto
            {
                ClientId = order.Client.ClientId,
                FullName = order.Client.FullName,
                PhoneNumber = order.Client.PhoneNumber
            },
            Item = new ItemDto
            {
                ItemId = order.Item.ItemId,
                Name = order.Item.Name,
                Material = order.Item.Material,
                Category = new ItemCategoryDto
                {
                    CategoryId = order.Item.Category.CategoryId,
                    CategoryName = order.Item.Category.CategoryName,
                    RecommendedCleaningType = order.Item.Category.RecommendedCleaningType,
                    Price = order.Item.Category.Price
                }
            },
            Status = order.Status,
            ProcessingDays = order.ProcessingDays,
            AcceptDate = order.AcceptDate,
            CompletedDate = order.CompletedDate
        };
    }

    /// <summary>
    /// Проверяет данные заказа по атрибутам DTO
    /// </summary>
    /// <param name="order">Данные для создания или изменения заказа</param>
    /// <exception cref="ValidationException">Данные заказа не прошли проверку</exception>
    private static void ValidateOrder(object order)
    {
        ArgumentNullException.ThrowIfNull(order);
        Validator.ValidateObject(order, new ValidationContext(order), validateAllProperties: true);
    }

    /// <summary>
    /// Проверяет дату завершения заказа
    /// </summary>
    /// <param name="order">Новые данные заказа</param>
    /// <param name="acceptDate">Дата приема заказа</param>
    /// <returns>Дата завершения в UTC или null для незавершенного заказа</returns>
    /// <exception cref="ValidationException">Дата завершения не соответствует статусу или дате приема</exception>
    private static DateTime? ValidateCompletedDate(OrderEditDto order, DateTime acceptDate)
    {
        var finished = order.Status is OrderStatus.Completed or OrderStatus.Issued;

        if (finished && order.CompletedDate is null)
            throw new ValidationException("Для завершенного или выданного заказа нужна дата завершения");

        if (!finished && order.CompletedDate is not null)
            throw new ValidationException("У незавершенного заказа не должно быть даты завершения");

        if (order.CompletedDate is not DateTime completed)
            return null;

        if (completed.Kind == DateTimeKind.Unspecified)
            throw new ValidationException("Укажите часовой пояс даты завершения: Z или смещение");

        var completedUtc = completed.ToUniversalTime();
        var acceptedUtc = acceptDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(acceptDate, DateTimeKind.Utc)
            : acceptDate.ToUniversalTime();

        if (completedUtc < acceptedUtc)
            throw new ValidationException("Дата завершения не может быть раньше даты приема");

        return completedUtc;
    }
}
