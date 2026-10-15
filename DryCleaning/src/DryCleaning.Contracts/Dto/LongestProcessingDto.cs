namespace DryCleaning.Contracts.Dto;

/// <summary>Клиенты с максимальной фактической длительностью завершённого заказа.</summary>
public class LongestProcessingDto
{
    /// <summary>Максимальная длительность обработки в днях.</summary>
    public double DurationDays { get; set; }

    /// <summary>Клиенты без повторов, отсортированные по полному имени и идентификатору.</summary>
    public required IReadOnlyList<ClientDto> Clients { get; set; }
}
