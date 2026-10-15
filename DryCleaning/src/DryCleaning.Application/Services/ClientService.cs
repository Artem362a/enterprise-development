using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain;
using DryCleaning.Domain.Enum;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>Сервис для работы с клиентами химчистки.</summary>
/// <param name="repository">Репозиторий клиентов.</param>
/// <param name="orderRepository">Репозиторий заказов.</param>
/// <param name="relationshipLock">Защита операций со связанными данными</param>
public class ClientService(IClientRepository repository, IOrderRepository orderRepository, IRelationshipLock relationshipLock) : IClientService
{
    private readonly IRelationshipLock _relationshipLock = relationshipLock;
    private readonly IClientRepository _repository = repository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    /// <summary>Возвращает всех клиентов.</summary>
    /// <returns>Список клиентов.</returns>
    public async Task<IReadOnlyList<ClientDto>> GetAllAsync()
    {
        var clients = await _repository.GetAllAsync();
        return [.. clients.Select(MapToDto)];
    }

    /// <summary>Возвращает клиента по идентификатору.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>Данные клиента или null, если он не найден.</returns>
    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client is null)
        {
            return null;
        }

        return MapToDto(client);
    }

    /// <summary>Создаёт клиента.</summary>
    /// <param name="client">Данные нового клиента.</param>
    /// <returns>Созданный клиент с назначенным идентификатором.</returns>
    public async Task<ClientDto> AddAsync(ClientEditDto client)
    {
        var newClient = new Client
        {
            FullName = client.FullName,
            PhoneNumber = client.PhoneNumber
        };

        var createdClient = await _repository.AddAsync(newClient);
        return MapToDto(createdClient);
    }

    /// <summary>Обновляет данные клиента.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <param name="client">Новые данные клиента.</param>
    /// <returns>true — клиент обновлён; false — не найден.</returns>
    public async Task<bool> UpdateAsync(int id, ClientEditDto client)
    {
        var updatedClient = new Client
        {
            ClientId = id,
            FullName = client.FullName,
            PhoneNumber = client.PhoneNumber
        };

        return await _repository.UpdateAsync(updatedClient);
    }

    /// <summary>Удаляет клиента по идентификатору.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>true — клиент удалён; false — не найден.</returns>
    /// <exception cref="ClientHasOrdersException">У клиента есть заказы.</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        var client = await _repository.GetByIdAsync(id);
        if (client is null)
        {
            return false;
        }

        if (await _orderRepository.HasOrdersForClientAsync(id))
        {
            throw new ClientHasOrdersException(id);
        }

        return await _repository.DeleteAsync(id);
    }

    /// <summary>Возвращает пять клиентов с наибольшим числом неотменённых заказов за период.</summary>
    /// <param name="start">Начало периода, включаемое в расчёт.</param>
    /// <param name="end">Конец периода, не включаемый в расчёт.</param>
    /// <returns>Клиенты и количество их заказов.</returns>
    /// <exception cref="ArgumentException">Начало периода не раньше конца.</exception>
    public async Task<IReadOnlyList<TopClientDto>> GetTopClientsAsync(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Начало периода должно быть раньше конца.", nameof(start));
        }

        var orders = await _orderRepository.GetAllAsync();
        return orders
            .Where(order => order.AcceptDate >= start && order.AcceptDate < end)
            .Where(order => order.Status != OrderStatus.Cancelled)
            .GroupBy(order => order.Client.ClientId)
            .Select(group => new { Client = group.First().Client, ItemCount = group.Count() })
            .OrderByDescending(result => result.ItemCount)
            .ThenBy(result => result.Client.FullName, StringComparer.Ordinal)
            .ThenBy(result => result.Client.ClientId)
            .Take(5)
            .Select(result => new TopClientDto
            {
                ClientId = result.Client.ClientId,
                FullName = result.Client.FullName,
                PhoneNumber = result.Client.PhoneNumber,
                ItemCount = result.ItemCount
            })
            .ToList();
    }

    /// <summary>Возвращает клиента с наибольшей суммой выданных заказов по текущим тарифам.</summary>
    /// <returns>Клиент и сумма расходов или null, если выданных заказов нет.</returns>
    public async Task<TopSpenderDto?> GetTopSpenderAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        var topSpender = orders
            .Where(order => order.Status == OrderStatus.Issued)
            .GroupBy(order => order.Client.ClientId)
            .Select(group => new
            {
                Client = group.First().Client,
                TotalSpent = group.Sum(order => order.Item.Category.Price)
            })
            .OrderByDescending(result => result.TotalSpent)
            .ThenBy(result => result.Client.FullName, StringComparer.Ordinal)
            .ThenBy(result => result.Client.ClientId)
            .FirstOrDefault();

        if (topSpender is null)
        {
            return null;
        }

        return new TopSpenderDto
        {
            Client = MapToDto(topSpender.Client),
            TotalSpent = topSpender.TotalSpent
        };
    }

    /// <summary>Возвращает клиентов с максимальной фактической длительностью завершённого заказа.</summary>
    /// <returns>Длительность и список клиентов или null, если подходящих заказов нет.</returns>
    public async Task<LongestProcessingDto?> GetLongestProcessingOrderAsync()
    {
        using var operation = await _relationshipLock.AcquireAsync();

        var orders = await _orderRepository.GetAllAsync();
        var completedOrders = orders
            .Where(order => (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Issued) &&
                            order.CompletedDate.HasValue)
            .Select(order => new
            {
                order.Client,
                Duration = order.CompletedDate!.Value - order.AcceptDate
            })
            .ToList();

        if (completedOrders.Count == 0)
        {
            return null;
        }

        var maxDuration = completedOrders.Max(order => order.Duration);
        var clients = completedOrders
            .Where(order => order.Duration == maxDuration)
            .Select(order => order.Client)
            .DistinctBy(client => client.ClientId)
            .OrderBy(client => client.FullName, StringComparer.Ordinal)
            .ThenBy(client => client.ClientId)
            .Select(MapToDto)
            .ToList();

        return new LongestProcessingDto
        {
            DurationDays = maxDuration.TotalDays,
            Clients = clients
        };
    }

    /// <summary>Преобразует клиента в DTO.</summary>
    /// <param name="client">Клиент химчистки.</param>
    /// <returns>Данные клиента для ответа API.</returns>
    private static ClientDto MapToDto(Client client)
    {
        return new ClientDto
        {
            ClientId = client.ClientId,
            FullName = client.FullName,
            PhoneNumber = client.PhoneNumber
        };
    }
}
