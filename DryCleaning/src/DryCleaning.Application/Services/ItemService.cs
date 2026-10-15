using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain;
using DryCleaning.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Exceptions;

namespace DryCleaning.Application.Services;

/// <summary>
/// Сервис изделий химчистки
/// </summary>
/// <param name="itemRepository">Репозиторий изделий</param>
/// <param name="categoryRepository">Репозиторий категорий</param>
/// <param name="orderRepository">Репозиторий заказов</param>
/// <param name="relationshipLock">Защита операций со связанными данными</param>
public class ItemService(IItemRepository itemRepository, IItemCategoryRepository categoryRepository, IOrderRepository orderRepository, IRelationshipLock relationshipLock) : IItemService
{
    private readonly IRelationshipLock _relationshipLock = relationshipLock;
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IItemCategoryRepository _categoryRepository = categoryRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    /// <summary>
    /// Возвращает изделия категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Список изделий или null, если категория не найдена</returns>
    public async Task<IReadOnlyList<ItemDto>?> GetByCategoryIdAsync(int categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null)
            return null;

        var items = await _itemRepository.GetByCategoryIdAsync(categoryId);
        return [.. items.Select(MapToDto)];
    }

    /// <summary>
    /// Создает изделие
    /// </summary>
    /// <param name="item">Данные изделия</param>
    /// <returns>Новое изделие с присвоенным идентификатором</returns>
    /// <exception cref="ValidationException">Некорректные данные изделия или категория не найдена</exception>
    public async Task<ItemDto> AddAsync(ItemEditDto item)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        ValidateItem(item);
        var category = await _categoryRepository.GetByIdAsync(item.CategoryId) ?? throw new ValidationException("Категория не найдена");
        var createdItem = await _itemRepository.AddAsync(new Item
        {
            Name= item.Name,
            Material = item.Material,
            Category = category
        });
        return MapToDto(createdItem);
    }
    /// <summary>
    /// Удаляет изделие по идентификатору, если с ним не связаны заказы
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <returns>True - если изделие удалено, False - изделие не найдено</returns>
    /// <exception cref="ItemHasOrdersException">С изделием связаны заказы</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        var item = await _itemRepository.GetByIdAsync(id);
        if (item is null)
            return false;

        if (await _orderRepository.HasOrdersForItemAsync(id))
            throw new ItemHasOrdersException(id);

        return await _itemRepository.DeleteAsync(id);
    }
    /// <summary>
    /// Возвращает список изделий
    /// </summary>
    /// <returns>Список изделий</returns>
    public async Task<IReadOnlyList<ItemDto>> GetAllAsync()
    {
        var items = await _itemRepository.GetAllAsync();
        return [.. items.Select(MapToDto)];
    }
    /// <summary>
    /// Возвращает изделие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <returns>Изделие или null, если оно не найдено</returns>
    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        return item != null ? MapToDto(item) : null;
    }
    /// <summary>
    /// Обновляет данные изделия
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <param name="item">Новые данные изделия</param>
    /// <returns>True - если изделие обновлено, False - изделие не найдено</returns>
    /// <exception cref="ValidationException">Некорректные данные изделия или категория не найдена</exception>
    public async Task<bool> UpdateAsync(int id, ItemEditDto item)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        ValidateItem(item);
        var existingItem = await _itemRepository.GetByIdAsync(id);
        if (existingItem == null)
        {
            return false;
        }
        var category = await _categoryRepository.GetByIdAsync(item.CategoryId) ?? throw new ValidationException("Категория не найдена");
        var updatedItem = new Item
        {
            ItemId = id,
            Name = item.Name,
            Material = item.Material,
            Category = category
        };
        return await _itemRepository.UpdateAsync(updatedItem);
    }
    /// <summary>
    /// Преобразует изделие в DTO
    /// </summary>
    /// <param name="item">Изделие</param>
    /// <returns>Данные изделия для ответа API</returns>
    private static ItemDto MapToDto(Item item)
    {
        return new ItemDto
        {
            ItemId = item.ItemId,
            Name = item.Name,
            Material = item.Material,
            Category = new ItemCategoryDto
            {
                CategoryId = item.Category.CategoryId,
                CategoryName = item.Category.CategoryName,
                RecommendedCleaningType = item.Category.RecommendedCleaningType,
                Price = item.Category.Price
            }
        };
    }
    /// <summary>
    /// Валидирует данные изделия
    /// </summary>
    /// <param name="item">Валидируемое изделие</param>
    /// <exception cref="ValidationException">Данные изделия не прошли проверку</exception>
    private static void ValidateItem(ItemEditDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        Validator.ValidateObject(
            item,
            new ValidationContext(item),
            validateAllProperties: true);
    }
}
