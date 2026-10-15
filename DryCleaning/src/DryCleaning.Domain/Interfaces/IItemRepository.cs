namespace DryCleaning.Domain.Interfaces;

/// <summary>Описывает доступ к изделиям химчистки.</summary>
public interface IItemRepository
{
    /// <summary>Возвращает все изделия.</summary>
    /// <returns>Список изделий.</returns>
    public Task<IReadOnlyList<Item>> GetAllAsync();

    /// <summary>
    /// Возвращает изделия категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Список изделий</returns>
    public Task<IReadOnlyList<Item>> GetByCategoryIdAsync(int categoryId);

    /// <summary>Возвращает изделие по идентификатору.</summary>
    /// <param name="id">Идентификатор изделия.</param>
    /// <returns>Изделие или null, если оно не найдено.</returns>
    public Task<Item?> GetByIdAsync(int id);

    /// <summary>Добавляет изделие.</summary>
    /// <param name="item">Новое изделие.</param>
    /// <returns>Изделие с назначенным идентификатором.</returns>
    public Task<Item> AddAsync(Item item);

    /// <summary>Обновляет данные изделия.</summary>
    /// <param name="item">Изделие с идентификатором и новыми данными.</param>
    /// <returns>true — изделие обновлено; false — не найдено.</returns>
    public Task<bool> UpdateAsync(Item item);

    /// <summary>Удаляет изделие по идентификатору.</summary>
    /// <param name="id">Идентификатор изделия.</param>
    /// <returns>true — изделие удалено; false — не найдено.</returns>
    public Task<bool> DeleteAsync(int id);

    /// <summary>Проверяет наличие изделий в категории.</summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <returns>true — изделия есть; false — изделий нет.</returns>
    public Task<bool> HasItemsForCategoryAsync(int categoryId);
}
