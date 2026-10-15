using DryCleaning.Contracts.Dto;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Описывает операции с изделиями химчистки
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Возвращает все изделия химчистки
    /// </summary>
    /// <returns>Список изделий</returns>
    public Task<IReadOnlyList<ItemDto>> GetAllAsync();
    /// <summary>
    /// Возвращает изделия категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Список изделий или null, если категория не найдена</returns>
    public Task<IReadOnlyList<ItemDto>?> GetByCategoryIdAsync(int categoryId);

    /// <summary>
    /// Возвращает изделие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <returns>Изделие или null, если не найдено</returns>
    public Task<ItemDto?> GetByIdAsync(int id);
    /// <summary>
    /// Создаёт изделие
    /// </summary>
    /// <param name="item">Новое изделие</param>
    /// <returns>Созданное изделие с назначенным идентификатором</returns>
    public Task<ItemDto> AddAsync(ItemEditDto item);
    /// <summary>
    /// Обновляет данные изделия
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <param name="item">Новые данные изделия</param>
    /// <returns>True - если изделие успешно обновлено, False - иначе</returns>
    public Task<bool> UpdateAsync(int id, ItemEditDto item);
    /// <summary>
    /// Удаляет изделие по идентификатору, если с ним не связаны заказы
    /// </summary>
    /// <param name="id">идентификатор изделия</param>
    /// <returns>True - если изделие успешно удалено, False - иначе</returns>
    public Task<bool> DeleteAsync(int id);
}
