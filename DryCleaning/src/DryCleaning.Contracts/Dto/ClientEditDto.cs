using System.ComponentModel.DataAnnotations;
namespace DryCleaning.Contracts.Dto;

/// <summary>
/// Данные для создания или изменения клиента
/// </summary>
public class ClientEditDto
{
    /// <summary>
    /// Полное имя клиента
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string FullName { get; set; }

    /// <summary>
    /// Номер телефона клиента
    /// </summary>
    [Required]
    [MaxLength(20)]
    [Phone]
    public required string PhoneNumber { get; set; }
}
