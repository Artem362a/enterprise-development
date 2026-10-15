namespace DryCleaning.Domain.Interfaces;
/// <summary>
/// Интерфейс клиентов химчистки
/// </summary>
public interface IClientRepository
{
    /// <summary>
    /// Возвращает всех клиентов химчистки
    /// </summary>
    /// <returns>Задача, результат которой содержит список всех клиентов</returns>
    public Task<IReadOnlyList<Client>> GetAllAsync();
    /// <summary>
    /// Возвращает клиента по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор клиента</param>
    /// <returns>Клиент или null, если он не найден</returns>
    public Task<Client?> GetByIdAsync(int id);
    /// <summary>
    /// Добавляет клиента
    /// </summary>
    /// <param name="client">Клиент химчистки</param>
    /// <returns>Клиент</returns>
    public Task<Client> AddAsync(Client client);
    /// <summary>
    ///Обновляет данные клиента
    /// </summary>
    /// <param name="client">Клиент с обновлёнными данными.</param>
    /// <returns>True - успешное обновление, False - в противном случае</returns>
    public Task<bool> UpdateAsync(Client client);
    /// <summary>
    /// Удаляет клиента по id
    /// </summary>
    /// <param name="id"> Уникальный идентификатор клиента</param>
    /// <returns>True - успешное удаление, False - в противном случае</returns>
    public Task<bool> DeleteAsync(int id);
}
