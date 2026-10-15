using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Exceptions;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain;
using DryCleaning.Domain.Enum;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>
/// Сервис категорий изделий химчистки
/// </summary>
/// <param name="repository">Репозиторий категорий</param>
/// <param name="itemRepository">Репозиторий изделий</param>
/// <param name="orderRepository">Репозиторий заказов</param>
/// <param name="relationshipLock">Защита операций со связанными данными</param>
public class ItemCategoryService(
    IItemCategoryRepository repository,
    IItemRepository itemRepository,
    IOrderRepository orderRepository, IRelationshipLock relationshipLock) : IItemCategoryService
{
    private readonly IRelationshipLock _relationshipLock = relationshipLock;
    private readonly IItemCategoryRepository _repository = repository;
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    /// <summary>
    /// Возвращает все категории
    /// </summary>
    /// <returns>Список категорий</returns>
    public async Task<IReadOnlyList<ItemCategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return [.. categories.Select(MapToDto)];
    }

    /// <summary>
    /// Возвращает категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Категория или null, если она не найдена</returns>
    public async Task<ItemCategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category is null)
        {
            return null;
        }

        return MapToDto(category);
    }

    /// <summary>
    /// Создаёт категорию после проверки входных данных
    /// </summary>
    /// <param name="category">Данные новой категории</param>
    /// <returns>Созданная категория с назначенным идентификатором</returns>
    /// <exception cref="ValidationException">Входные данные не прошли проверку</exception>
    public async Task<ItemCategoryDto> AddAsync(ItemCategoryEditDto category)
    {
        ValidateCategory(category);

        var newCategory = new ItemCategory
        {
            CategoryName = category.CategoryName,
            RecommendedCleaningType = category.RecommendedCleaningType,
            Price = category.Price
        };

        var createdCategory = await _repository.AddAsync(newCategory);
        return MapToDto(createdCategory);
    }

    /// <summary>
    /// Обновляет данные категории
    /// </summary>
    /// <param name="id">Идентификатор изменяемой категории</param>
    /// <param name="category">Новые данные категории</param>
    /// <returns>True — категория обновлена, False — иначе</returns>
    /// <exception cref="ValidationException">Входные данные не прошли проверку</exception>
    public async Task<bool> UpdateAsync(int id, ItemCategoryEditDto category)
    {
        ValidateCategory(category);

        var updatedCategory = new ItemCategory
        {
            CategoryId = id,
            CategoryName = category.CategoryName,
            RecommendedCleaningType = category.RecommendedCleaningType,
            Price = category.Price
        };

        return await _repository.UpdateAsync(updatedCategory);
    }

    /// <summary>
    /// Удаляет категорию, если она не используется изделиями
    /// </summary>
    /// <param name="id">Идентификатор удаляемой категории</param>
    /// <returns>True - категория удалена, False - иначе</returns>
    /// <exception cref="CategoryHasItemsException">Категория используется изделиями</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        using var operation = await _relationshipLock.AcquireAsync();

        var category = await _repository.GetByIdAsync(id);
        if (category is null)
        {
            return false;
        }

        if (await _itemRepository.HasItemsForCategoryAsync(id))
        {
            throw new CategoryHasItemsException(id);
        }

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Возвращает пять наиболее и пять наименее популярных категорий за предыдущий год
    /// </summary>
    /// <param name="end">Конец периода, не включаемый в расчёт</param>
    /// <returns>Два рейтинга категорий с количеством принятых изделий</returns>
    /// <remarks>Начало периода включается. Отменённые заказы исключаются, категории без заказов учитываются</remarks>
    /// <exception cref="ArgumentOutOfRangeException">Нельзя вычислить начало предыдущего года</exception>
    public async Task<CategoryPopularityDto> GetPopularityAsync(DateTime end)
    {
        if (end.Year <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(end),
                "Укажите дату окончания периода, позволяющую вычислить предшествующий год.");
        }

        var start = end.AddYears(-1);
        var categories = await _repository.GetAllAsync();
        var orders = await _orderRepository.GetAllAsync();
        var periodOrders = orders
            .Where(order => order.AcceptDate >= start && order.AcceptDate < end)
            .Where(order => order.Status != OrderStatus.Cancelled)
            .ToList();

        var entries = categories
            .Select(category => new CategoryPopularityEntryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ItemCount = periodOrders.Count(order => order.Item.Category.CategoryId == category.CategoryId)
            })
            .ToList();

        return new CategoryPopularityDto
        {
            MostPopular = entries
                .OrderByDescending(entry => entry.ItemCount)
                .ThenBy(entry => entry.CategoryName, StringComparer.Ordinal)
                .ThenBy(entry => entry.CategoryId)
                .Take(5)
                .ToList(),
            LeastPopular = entries
                .OrderBy(entry => entry.ItemCount)
                .ThenBy(entry => entry.CategoryName, StringComparer.Ordinal)
                .ThenBy(entry => entry.CategoryId)
                .Take(5)
                .ToList()
        };
    }

    /// <summary>
    /// Проверяет данные категории по атрибутам DTO
    /// </summary>
    /// <param name="category">Данные для создания или изменения категории</param>
    /// <exception cref="ValidationException">Входные данные не прошли проверку</exception>
    private static void ValidateCategory(ItemCategoryEditDto category)
    {
        ArgumentNullException.ThrowIfNull(category);
        Validator.ValidateObject(category, new ValidationContext(category), validateAllProperties: true);
    }

    /// <summary>
    /// Преобразует категорию в DTO
    /// </summary>
    /// <param name="category">Категория изделий</param>
    /// <returns>Данные категории для ответа API</returns>
    private static ItemCategoryDto MapToDto(ItemCategory category)
    {
        return new ItemCategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            RecommendedCleaningType = category.RecommendedCleaningType,
            Price = category.Price
        };
    }
}
