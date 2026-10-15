namespace DryCleaning.Contracts.Dto;

/// <summary>Клиент с наибольшей суммой выданных заказов.</summary>
public class TopSpenderDto
{
    /// <summary>Данные клиента.</summary>
    public required ClientDto Client { get; set; }

    /// <summary>Сумма расходов по текущим тарифам категорий.</summary>
    public decimal TotalSpent { get; set; }
}
