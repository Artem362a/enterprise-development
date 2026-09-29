namespace DryCleaning.Api.Requests;

/// <summary>
/// Новые контактные данные существующего клиента
/// </summary>
public class UpdateClientRequest
{
    /// <summary>
    /// Новое имя клиента
    /// </summary>
    public required string FullName { get; set; }
    /// <summary>
    /// Новый номер телефона клиента
    /// </summary>
    public required string PhoneNumber { get; set; }
}
