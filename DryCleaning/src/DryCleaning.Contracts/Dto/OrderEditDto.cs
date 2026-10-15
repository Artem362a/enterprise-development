namespace DryCleaning.Contracts.Dto;
using System.ComponentModel.DataAnnotations;
using DryCleaning.Domain.Enum;

/// <summary>
/// Данные для изменения заказа
/// </summary>
public class OrderEditDto
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
    /// Плановый срок обрработки изделия
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Срок обработки должен быть больше нуля")]
    public required int ProcessingDays { get; set; }
    /// <summary>
    /// Статус заказа
    /// </summary>
    [Required]
    [EnumDataType(typeof(OrderStatus),
    ErrorMessage = "Недопустимый статус заказа")]
    public required OrderStatus Status { get; set; }
    /// <summary>
    /// Дата выполнения заказа
    /// </summary>
    public DateTime? CompletedDate { get; set; }
}
