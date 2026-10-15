using DryCleaning.Domain;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Domain.Seeds;

namespace DryCleaning.Infrastructure.InMemory.Repositories;
/// <summary>Репозиторий клиентов в памяти</summary>
/// <param name="data">Данные химчистки</param>
public class ClientRepository(TestData data) : IClientRepository
{
    /// <summary>Данные химчистки</summary>
    private readonly TestData _data = data;

    /// <summary>Добавляет клиента</summary>
    /// <param name="client">Новый клиент</param>
    /// <returns>Клиент с назначенным идентификатором</returns>
    public Task<Client> AddAsync(Client client)
    {
        lock (_data.Clients)
        {
            client.ClientId = _data.Clients
                .Select(c => c.ClientId)
                .DefaultIfEmpty(0)
                .Max() + 1;

            _data.Clients.Add(client);
        }

        return Task.FromResult(client);
    }

    /// <summary>Удаляет клиента по идентификатору</summary>
    /// <param name="id">Идентификатор клиента</param>
    /// <returns>true — клиент удалён; false — не найден</returns>
    public Task<bool> DeleteAsync(int id)
    {
        lock (_data.Clients)
        {
            var client = _data.Clients.FirstOrDefault(c => c.ClientId == id);
            if (client is null)
            {
                return Task.FromResult(false);
            }
            _data.Clients.Remove(client);
            return Task.FromResult(true);
        }
    }

    /// <summary>Возвращает всех клиентов.</summary>
    /// <returns>Список клиентов.</returns>
    public Task<IReadOnlyList<Client>> GetAllAsync()
    {
        lock (_data.Clients)
        {
            var clients = _data.Clients.ToList();
            return Task.FromResult<IReadOnlyList<Client>>(clients);
        }
    }

    /// <summary>Возвращает клиента по идентификатору.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>Клиент или null, если он не найден.</returns>
    public Task<Client?> GetByIdAsync(int id)
    {
        lock (_data.Clients)
        {
            return Task.FromResult(_data.Clients.FirstOrDefault(c => c.ClientId == id));
        }
    }

    /// <summary>Обновляет данные клиента.</summary>
    /// <param name="client">Клиент с новыми данными</param>
    /// <returns>true — клиент обновлён; false — не найден.</returns>
    public Task<bool> UpdateAsync(Client client)
    {
        lock (_data.Clients)
        {
            var existingClient = _data.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);
            if (existingClient is null)
            {
                return Task.FromResult(false);
            }
            existingClient.FullName = client.FullName;
            existingClient.PhoneNumber = client.PhoneNumber;
            return Task.FromResult(true);
        }
    }
}

