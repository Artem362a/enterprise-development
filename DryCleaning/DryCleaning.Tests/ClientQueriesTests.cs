using DryCleaning.Domain.Enum;
using DryCleaning.Domain.Seeds;

namespace DryCleaning.Tests;

/// <summary>
/// Тесты аналитических запросов по клиентам химчистки.
/// </summary>
/// <param name="data">Общий набор тестовых данных.</param>
public class ClientQueriesTests(TestData data) : IClassFixture<TestData>
{
    /// <summary>
    /// Проверяет пять клиентов, сдавших больше всего изделий
    /// за заданный период.
    /// </summary>
    [Fact]
    public void TopClientsAreSortedByItemCount()
    {
        var start = new DateTime(2025, 9, 15);
        var end = new DateTime(2026, 9, 15);
        int[] expectedIds = [1, 2, 3, 4, 5];
        int[] expectedCounts = [4, 4, 3, 2, 1];

        var clients = data.Orders.AsEnumerable().Reverse()
            .Where(order => order.AcceptDate >= start && order.AcceptDate < end)
            .Where(order => order.Status != OrderStatus.Cancelled)
            .GroupBy(order => order.Client.ClientId)
            .Select(group => new { group.First().Client, ItemCount = group.Count() })
            .OrderByDescending(client => client.ItemCount)
            .ThenBy(client => client.Client.FullName, StringComparer.Ordinal)
            .ThenBy(client => client.Client.ClientId)
            .Take(5)
            .ToList();

        Assert.Equal(expectedIds, clients.Select(client => client.Client.ClientId));
        Assert.Equal(expectedCounts, clients.Select(client => client.ItemCount));
    }

    /// <summary>
    /// Проверяет клиентов с наибольшей длительностью обработки заказов,
    /// отсутствие повторов и сортировку по полному имени.
    /// </summary>
    [Fact]
    public void ClientsWithLongestProcessingAreSortedByName()
    {
        int[] expectedIds = [1, 4];
        var expectedDuration = TimeSpan.FromDays(19);

        var completedOrders = data.Orders.AsEnumerable().Reverse()
            .Where(order => order.Status == OrderStatus.Completed || order.Status == OrderStatus.Issued)
            .Where(order => order.CompletedDate.HasValue)
            .Select(order => new { order.Client, Duration = order.CompletedDate!.Value - order.AcceptDate })
            .ToList();

        var longestDuration = completedOrders.Max(order => order.Duration);
        var clients = completedOrders
            .Where(order => order.Duration == longestDuration)
            .Select(order => order.Client)
            .DistinctBy(client => client.ClientId)
            .OrderBy(client => client.FullName, StringComparer.Ordinal)
            .ThenBy(client => client.ClientId)
            .ToList();

        Assert.Equal(expectedDuration, longestDuration);
        Assert.Equal(expectedIds, clients.Select(client => client.ClientId));
    }

    /// <summary>
    /// Проверяет клиента, потратившего наибольшую сумму
    /// по всем выданным заказам.
    /// </summary>
    [Fact]
    public void HighestSpendingClientIsCalculatedFromIssuedOrders()
    {
        var expectedId = 2;
        var expectedTotal = 6700m;

        var client = data.Orders
            .Where(order => order.Status == OrderStatus.Issued)
            .GroupBy(order => order.Client.ClientId)
            .Select(group => new { group.First().Client, TotalSpent = group.Sum(order => order.Item.Category.Price) })
            .OrderByDescending(client => client.TotalSpent)
            .ThenBy(client => client.Client.FullName, StringComparer.Ordinal)
            .ThenBy(client => client.Client.ClientId)
            .FirstOrDefault();

        Assert.NotNull(client);
        Assert.Equal(expectedId, client.Client.ClientId);
        Assert.Equal(expectedTotal, client.TotalSpent);
    }
}
