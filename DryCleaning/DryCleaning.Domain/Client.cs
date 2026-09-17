namespace DryCleaning.Domain;

/// <summary>
/// Класс, описывающий клиента химчистки.
/// Содержит уникальный идентификатор, полное имя и номер телефона клиента.
/// </summary>
public class Client
{
    public int ClientId { get; set; }
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
}
