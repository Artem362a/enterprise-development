using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>Описывает операции с клиентами химчистки.</summary>
public interface IClientService
{
    /// <summary>Возвращает всех клиентов.</summary>
    /// <returns>Список клиентов.</returns>
    public Task<IReadOnlyList<ClientDto>> GetAllAsync();

    /// <summary>Возвращает клиента по идентификатору.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>Клиент или null, если он не найден.</returns>
    public Task<ClientDto?> GetByIdAsync(int id);

    /// <summary>Создаёт клиента.</summary>
    /// <param name="client">Данные нового клиента.</param>
    /// <returns>Созданный клиент с назначенным идентификатором.</returns>
    public Task<ClientDto> AddAsync(ClientEditDto client);

    /// <summary>Обновляет данные клиента.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <param name="client">Новые данные клиента.</param>
    /// <returns>true — клиент обновлён; false — не найден.</returns>
    public Task<bool> UpdateAsync(int id, ClientEditDto client);

    /// <summary>Удаляет клиента по идентификатору.</summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>true — клиент удалён; false — не найден.</returns>
    /// <exception cref="ClientHasOrdersException">У клиента есть заказы.</exception>
    public Task<bool> DeleteAsync(int id);

    /// <summary>Возвращает пять клиентов с наибольшим числом неотменённых заказов за период.</summary>
    /// <param name="start">Начало периода, включаемое в расчёт.</param>
    /// <param name="end">Конец периода, не включаемый в расчёт.</param>
    /// <returns>Клиенты и количество их заказов.</returns>
    /// <exception cref="ArgumentException">Начало периода не раньше конца.</exception>
    public Task<IReadOnlyList<TopClientDto>> GetTopClientsAsync(DateTime start, DateTime end);

    /// <summary>Возвращает клиента с наибольшей суммой выданных заказов.</summary>
    /// <returns>Клиент и сумма расходов или null, если выданных заказов нет.</returns>
    public Task<TopSpenderDto?> GetTopSpenderAsync();

    /// <summary>Возвращает клиентов с максимальной длительностью завершённого заказа.</summary>
    /// <returns>Длительность и список клиентов или null, если подходящих заказов нет.</returns>
    public Task<LongestProcessingDto?> GetLongestProcessingOrderAsync();
}
