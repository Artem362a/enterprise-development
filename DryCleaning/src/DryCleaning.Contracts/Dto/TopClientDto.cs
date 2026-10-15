namespace DryCleaning.Contracts.Dto;

/// <summary>Клиент и количество его неотменённых заказов за период.</summary>
public class TopClientDto
{
    /// <summary>Идентификатор клиента.</summary>
    public int ClientId { get; set; }

    /// <summary>Полное имя клиента.</summary>
    public required string FullName { get; set; }

    /// <summary>Номер телефона клиента.</summary>
    public required string PhoneNumber { get; set; }

    /// <summary>Количество заказов клиента за период.</summary>
    public int ItemCount { get; set; }
}
