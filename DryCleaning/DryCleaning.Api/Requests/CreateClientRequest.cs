namespace DryCleaning.Api.Requests;

/// <summary>
/// Данные для регистрации клиента химчистки.
/// </summary>
public class CreateClientRequest
{
    /// <summary>
    /// Полное имя клиента
    /// </summary>
    public required string FullName { get; set; }
    /// <summary>
    /// Номер телефона клиента
    /// </summary>
    public required string PhoneNumber { get; set; }
}
