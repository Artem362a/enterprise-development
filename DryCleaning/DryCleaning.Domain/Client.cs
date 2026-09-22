namespace DryCleaning.Domain;

/// <summary>
/// Класс, описывающий клиента химчистки
/// Содержит уникальный идентификатор, полное имя и номер телефона клиента
/// </summary>
public class Client
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
