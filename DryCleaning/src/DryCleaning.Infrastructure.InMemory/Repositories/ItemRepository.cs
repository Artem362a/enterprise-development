using DryCleaning.Domain;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Domain.Seeds;

namespace DryCleaning.Infrastructure.InMemory.Repositories;

/// <summary>Репозиторий изделий</summary>
/// <param name="data">Данные химчистки</param>
public class ItemRepository(TestData data) : IItemRepository
{
    /// <summary>Данные химчистки</summary>
    private readonly TestData _data = data;

    /// <summary>Добавляет изделие</summary>
    /// <param name="item">Новое изделие</param>
    /// <returns>Изделие с назначенным идентификатором</returns>
    public Task<Item> AddAsync(Item item)
    {
        lock (_data.Items)
        {
            item.ItemId = _data.Items
                .Select(existing => existing.ItemId)
                .DefaultIfEmpty(0)
                .Max() + 1;
            _data.Items.Add(item);
        }

        return Task.FromResult(item);
    }

    /// <summary>Удаляет изделие по идентификатору</summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <returns>True — изделие удалено False — иначе</returns>
    public Task<bool> DeleteAsync(int id)
    {
        lock (_data.Items)
        {
            var item = _data.Items.FirstOrDefault(existing => existing.ItemId == id);
            if (item is null)
            {
                return Task.FromResult(false);
            }

            _data.Items.Remove(item);
            return Task.FromResult(true);
        }
    }

    /// <summary>Возвращает все изделия</summary>
    /// <returns>Список изделий</returns>
    public Task<IReadOnlyList<Item>> GetAllAsync()
    {
        lock (_data.Items)
        {
            return Task.FromResult<IReadOnlyList<Item>>(_data.Items.ToList());
        }
    }

    /// <summary>
    /// Возвращает изделия категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Список изделий</returns>
    public Task<IReadOnlyList<Item>> GetByCategoryIdAsync(int categoryId)
    {
        lock (_data.Items)
        {
            return Task.FromResult<IReadOnlyList<Item>>(_data.Items
                .Where(item => item.Category.CategoryId == categoryId)
                .ToList());
        }
    }

    /// <summary>Возвращает изделие по идентификатору</summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <returns>Изделие или null, если оно не найден</returns>
    public Task<Item?> GetByIdAsync(int id)
    {
        lock (_data.Items)
        {
            return Task.FromResult(_data.Items.FirstOrDefault(item => item.ItemId == id));
        }
    }

    /// <summary>Проверяет наличие изделий в категории</summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>True — изделия есть, False — иначе</returns>
    public Task<bool> HasItemsForCategoryAsync(int categoryId)
    {
        lock (_data.Items)
        {
            return Task.FromResult(_data.Items.Any(item => item.Category.CategoryId == categoryId));
        }
    }

    /// <summary>Обновляет данные изделия</summary>
    /// <param name="item">Изделие с идентификатором и новыми данными</param>
    /// <returns>True — изделие обновлено, False — иначе</returns>
    public Task<bool> UpdateAsync(Item item)
    {
        lock (_data.Items)
        {
            var existingItem = _data.Items.FirstOrDefault(existing => existing.ItemId == item.ItemId);
            if (existingItem is null)
            {
                return Task.FromResult(false);
            }

            existingItem.Name = item.Name;
            existingItem.Material = item.Material;
            existingItem.Category = item.Category;
            return Task.FromResult(true);
        }
    }
}
