using DryCleaning.Domain;

namespace DryCleaning.Tests;

/// <summary>
/// Тесты аналитических запросов по заказам химчистки.
/// </summary>
/// <param name="data">Общий набор тестовых данных.</param>
public class OrderQueriesTests(TestData data) : IClassFixture<TestData>
{
    /// <summary>
    /// Проверяет вывод заказов, находящихся в обработке,
    /// как они сортируются по дате приёма.
    /// </summary>
    [Fact]
    public void InProgressOrdersAreSortedByAcceptDate()
    {
        int[] expectedIds = [1, 12, 9];

        var orders = data.Orders
            .Where(order => order.Status == OrderStatus.InProgress)
            .OrderBy(order => order.AcceptDate)
            .ThenBy(order => order.Id)
            .ToList();

        Assert.Equal(expectedIds, orders.Select(order => order.Id));
    }
}
