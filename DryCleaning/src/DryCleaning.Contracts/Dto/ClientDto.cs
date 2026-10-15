namespace DryCleaning.Contracts.Dto;

/// <summary>
/// Данные клиента в ответе API
/// </summary>

public class ClientDto
{
    /// <summary>
    /// Уникальный идентификатор клиента
    /// </summary>
    public int ClientId { get; set; }
    /// <summary>
    /// Полное имя клиента
    /// </summary>
    public required string FullName { get; set; }
    /// <summary>
    /// Номер телефона клиента
    /// </summary>
    public required string PhoneNumber { get; set; }

}