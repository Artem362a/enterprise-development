namespace DryCleaning.Contracts.Dto;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Данные для создания заказа
/// </summary>
public class OrderCreateDto
{
    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Неправильный идентификатор клиента")]
    public required int ClientId { get; set; }
    /// <summary>
    /// Уникальный идентификатор изделия
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Неправильный идентификатор изделия")]
    public required int ItemId { get; set; }
    /// <summary>
    /// Плановый срок обработки в днях
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Срок обработки должен быть больше нуля")]
    public required int ProcessingDays { get; set; }
}
