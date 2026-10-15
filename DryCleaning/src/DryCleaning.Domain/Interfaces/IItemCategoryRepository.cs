namespace DryCleaning.Domain.Interfaces;
/// <summary>
/// Интерфейс репозитория для асинхронной работы с категориями изделий
/// </summary>
public interface IItemCategoryRepository
{
    /// <summary>
    /// Возвращает все категории изделий
    /// </summary>
    /// <returns>Список всех категорий изделий</returns>
    public Task<IReadOnlyList<ItemCategory>> GetAllAsync();
    /// <summary>
    /// Возвращает категорию по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор категории</param>
    /// <returns>Категория или null, если она не найдена</returns>
    public Task<ItemCategory?> GetByIdAsync(int id);
    /// <summary>
    /// Добавляет категорию
    /// </summary>
    /// <param name="category">Новая категория</param>
    /// <returns>Категория с назначенным идентификатором</returns>
    public Task<ItemCategory> AddAsync(ItemCategory category);
    /// <summary>
    /// Обновляет категорию
    /// </summary>
    /// <param name="category">Новые данные для категории</param>
    /// <returns>True - успешное обновление, False - иначе</returns>
    public Task<bool> UpdateAsync(ItemCategory category);
    /// <summary>
    /// Удаляет категорию
    /// </summary>
    /// <param name="id">Уникальный идентификатор категории</param>
    /// <returns>True - успешное удаление, False - иначе</returns>
    public Task<bool> DeleteAsync(int id);

}
