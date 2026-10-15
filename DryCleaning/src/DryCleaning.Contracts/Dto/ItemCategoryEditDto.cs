using DryCleaning.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace DryCleaning.Contracts.Dto;

/// <summary>Данные для создания или изменения категории.</summary>
public class ItemCategoryEditDto
{
    /// <summary>
    /// Название (имя) категории
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string CategoryName { get; set; }
    /// <summary>
    /// Рекомендованный тип очистки
    /// </summary>
    [Required]
    [EnumDataType(typeof(CleaningType))]
    public required CleaningType RecommendedCleaningType { get; set; }
    /// <summary>
    /// Цена за очистку изделия этой категории (больше 0 и не выше 10 000 000)
    /// </summary>
    [Required]
    [Range(typeof(decimal), "0", "10000000", //мне кажется, что больше 10 миллионов за химчистку заплатить не получится
    MinimumIsExclusive = true,
    ErrorMessage = "Цена должна быть больше нуля и не превышать 10 000 000.")]
    public required decimal Price { get; set; }

}
