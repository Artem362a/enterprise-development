using DryCleaning.Domain;

namespace DryCleaning.Tests;

/// <summary>Verifies dry-cleaning order queries.</summary>
/// <param name="data">The shared data set created by xUnit.</param>
public class OrderQueriesTests(TestData data) : IClassFixture<TestData>
{
    /// <summary>Verifies that orders in progress are sorted by acceptance date.</summary>
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
