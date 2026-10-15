using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Описывает операции с категориями изделий химчистки
/// </summary>
public interface IItemCategoryService
{
    /// <summary>
    /// Возвращает все категории
    /// </summary>
    /// <returns>Список категорий</returns>
    public Task<IReadOnlyList<ItemCategoryDto>> GetAllAsync();

    /// <summary>
    /// Возвращает категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Категория или null, если она не найдена</returns>
    public Task<ItemCategoryDto?> GetByIdAsync(int id);

    /// <summary>
    /// Создаёт категорию после проверки входных данных
    /// </summary>
    /// <param name="category">Название, вид обработки и цена новой категории</param>
    /// <returns>Созданная категория с назначенным идентификатором</returns>
    /// <exception cref="ValidationException">Входные данные не прошли проверку</exception>
    public Task<ItemCategoryDto> AddAsync(ItemCategoryEditDto category);

    /// <summary>
    /// Обновляет данные категории
    /// </summary>
    /// <param name="id">Идентификатор изменяемой категории</param>
    /// <param name="category">Новые данные категории</param>
    /// <returns>Тrue — категория обновлена, False — иначе</returns>
    /// <exception cref="ValidationException">Входные данные не прошли проверку</exception>
    public Task<bool> UpdateAsync(int id, ItemCategoryEditDto category);

    /// <summary>
    /// Удаляет категорию, если она не используется изделиями
    /// </summary>
    /// <param name="id">Идентификатор удаляемой категории</param>
    /// <returns>Тrue - категория удалена, False - иначе</returns>
    /// <exception cref="CategoryHasItemsException">Категория используется изделиями</exception>
    public Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Возвращает пять наиболее и пять наименее популярных категорий за предыдущий год
    /// </summary>
    /// <param name="end">Конец периода, не включаемый в расчёт</param>
    /// <returns>Два рейтинга категорий с количеством принятых изделий</returns>
    /// <remarks>Начало периода включается. Отменённые заказы исключаются, категории без заказов учитываются</remarks>
    /// <exception cref="ArgumentOutOfRangeException">Нельзя вычислить начало предыдущего года</exception>
    public Task<CategoryPopularityDto> GetPopularityAsync(DateTime end);
}
