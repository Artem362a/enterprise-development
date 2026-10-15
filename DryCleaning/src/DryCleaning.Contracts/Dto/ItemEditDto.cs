using System.ComponentModel.DataAnnotations;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// Данные для создания или изменения изделия
/// </summary>
public class ItemEditDto
{
    /// <summary>
    /// Название изделия
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    /// <summary>
    /// Материал изделия
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Material { get; set; }
    /// <summary>
    /// Идентификатор категории изделия
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Идентификатор категории должен быть больше 0")]
    public required int CategoryId { get; set; }
}
