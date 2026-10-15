using DryCleaning.Domain;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Domain.Seeds;

namespace DryCleaning.Infrastructure.InMemory.Repositories;
/// <summary>
/// Репозиторий для категорий изделий в памяти
/// </summary>
/// <param name="data">Данные химчистки</param>
public class ItemCategoryRepository(TestData data) : IItemCategoryRepository
{
    /// <summary>Данные химчистки</summary>
    private readonly TestData _data = data;
    /// <summary>
    /// Добавляет категорию
    /// </summary>
    /// <param name="category">Категория изделия</param>
    /// <returns>Категория с назначенным идентификатором</returns>
    public Task<ItemCategory> AddAsync(ItemCategory category)
    {
        lock (_data.ItemCategories)
        {
            category.CategoryId = _data.ItemCategories
                .Select(c => c.CategoryId)
                .DefaultIfEmpty(0)
                .Max() + 1;
            _data.ItemCategories.Add(category);
            return Task.FromResult(category);
        }
    }
    /// <summary>
    /// Удаляет категорию по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор категории</param>
    /// <returns>True - успешное удаление, False - иначе</returns>
    public Task<bool> DeleteAsync(int id)
    {
        lock (_data.ItemCategories)
        {
            var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return Task.FromResult(false);

            _data.ItemCategories.Remove(category);
            return Task.FromResult(true);
        }
    }
    /// <summary>
    /// Возвращает все категории изделий
    /// </summary>
    /// <returns>Список категорий</returns>
    public Task<IReadOnlyList<ItemCategory>> GetAllAsync()
    {
        lock (_data.ItemCategories)
        {
            var category = _data.ItemCategories.ToList();
            return Task.FromResult<IReadOnlyList<ItemCategory>>(category);
        }
    }
    /// <summary>
    /// Возвращает категорию по идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор категории</param>
    /// <returns>Категория или null, если она не найдена</returns>
    public Task<ItemCategory?> GetByIdAsync(int id)
    {
        lock (_data.ItemCategories)
        {
            var category = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == id);
            return Task.FromResult(category);
        }
    }
    /// <summary>
    /// Обновляет данные категории
    /// </summary>
    /// <param name="category">Категория с новыми данными для обновления</param>
    /// <returns>True — категория обновлена, False — иначе</returns>
    public Task<bool> UpdateAsync(ItemCategory category)
    {
        lock (_data.ItemCategories)
        {
            var existingCategory = _data.ItemCategories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
                return Task.FromResult(false);

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.RecommendedCleaningType = category.RecommendedCleaningType;
            existingCategory.Price = category.Price;
            return Task.FromResult(true);
        }
    }
}
