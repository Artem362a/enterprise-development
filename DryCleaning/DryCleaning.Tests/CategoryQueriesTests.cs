using DryCleaning.Domain;

namespace DryCleaning.Tests;

/// <summary>Verifies item category popularity queries.</summary>
/// <param name="data">The shared data set created by xUnit.</param>
public class CategoryQueriesTests(TestData data) : IClassFixture<TestData>
{
    /// <summary>Verifies the five most and least popular categories over the previous year.</summary>
    [Fact]
    public void CategoryPopularityIsCalculatedForLastYear()
    {
        var end = new DateTime(2026, 3, 1);
        var start = end.AddYears(-1);
        int[] expectedMostIds = [8, 3, 6, 4, 1];
        int[] expectedMostCounts = [2, 1, 1, 1, 1];
        int[] expectedLeastIds = [2, 9, 3, 6, 4];
        int[] expectedLeastCounts = [0, 0, 1, 1, 1];

        var orders = data.Orders
            .Where(order => order.AcceptDate >= start && order.AcceptDate < end)
            .Where(order => order.Status != OrderStatus.Cancelled)
            .ToList();

        var categories = data.ItemCategories
            .Select(category => new
            {
                Category = category,
                ItemCount = orders.Count(order => order.Item.Category.CategoryId == category.CategoryId)
            })
            .ToList();

        var mostPopular = categories
            .OrderByDescending(category => category.ItemCount)
            .ThenBy(category => category.Category.CategoryName, StringComparer.Ordinal)
            .ThenBy(category => category.Category.CategoryId)
            .Take(5)
            .ToList();

        var leastPopular = categories
            .OrderBy(category => category.ItemCount)
            .ThenBy(category => category.Category.CategoryName, StringComparer.Ordinal)
            .ThenBy(category => category.Category.CategoryId)
            .Take(5)
            .ToList();

        Assert.Equal(expectedMostIds, mostPopular.Select(category => category.Category.CategoryId));
        Assert.Equal(expectedMostCounts, mostPopular.Select(category => category.ItemCount));
        Assert.Equal(expectedLeastIds, leastPopular.Select(category => category.Category.CategoryId));
        Assert.Equal(expectedLeastCounts, leastPopular.Select(category => category.ItemCount));
    }
}
