using DryCleaning.Domain.Enum;

namespace DryCleaning.Domain.Seeds;

/// <summary>Класс создаёт тестовые данные для химчистки.</summary>
public class TestData
{
    /// <summary>Клиенты химчистки.</summary>
    public List<Client> Clients { get; } =
    [
        new Client { ClientId = 1, FullName = "Alexander Kuznetsov", PhoneNumber = "+7 927 416 5823" },
        new Client { ClientId = 2, FullName = "Vadim Petrov", PhoneNumber = "+7 919 813 0876" },
        new Client { ClientId = 3, FullName = "Sergey Piratov", PhoneNumber = "+7 927 903 1946" },
        new Client { ClientId = 4, FullName = "Anna Smirnova", PhoneNumber = "+7 903 572 1846" },
        new Client { ClientId = 5, FullName = "Mikhail Volkov", PhoneNumber = "+7 960 348 2917" },
        new Client { ClientId = 6, FullName = "Elena Sokolova", PhoneNumber = "+7 902 681 4359" },
        new Client { ClientId = 7, FullName = "Dmitry Morozov", PhoneNumber = "+7 937 154 8620" },
        new Client { ClientId = 8, FullName = "Olga Novikova", PhoneNumber = "+7 987 429 3065" },
        new Client { ClientId = 9, FullName = "Alexey Popov", PhoneNumber = "+7 905 736 2184" },
        new Client { ClientId = 10, FullName = "Maria Vasilyeva", PhoneNumber = "+7 919 582 6473" }
    ];

    /// <summary>Категории изделий и их цены.</summary>
    public List<ItemCategory> ItemCategories { get; } =
    [
        new ItemCategory { CategoryId = 1, CategoryName = "Clothing", RecommendedCleaningType = CleaningType.DryCleaning, Price = 1500m },
        new ItemCategory { CategoryId = 2, CategoryName = "Shoes", RecommendedCleaningType = CleaningType.WetCleaning, Price = 2000m },
        new ItemCategory { CategoryId = 3, CategoryName = "Accessories", RecommendedCleaningType = CleaningType.SteamCleaning, Price = 1000m },
        new ItemCategory { CategoryId = 4, CategoryName = "Carpets", RecommendedCleaningType = CleaningType.WetCleaning, Price = 3500m },
        new ItemCategory { CategoryId = 5, CategoryName = "Curtains", RecommendedCleaningType = CleaningType.SteamCleaning, Price = 1800m },
        new ItemCategory { CategoryId = 6, CategoryName = "Bedding", RecommendedCleaningType = CleaningType.WetCleaning, Price = 1200m },
        new ItemCategory { CategoryId = 7, CategoryName = "Evening Wear", RecommendedCleaningType = CleaningType.DryCleaning, Price = 2500m },
        new ItemCategory { CategoryId = 8, CategoryName = "Leather Goods", RecommendedCleaningType = CleaningType.HandWash, Price = 3000m },
        new ItemCategory { CategoryId = 9, CategoryName = "Soft Toys", RecommendedCleaningType = CleaningType.HandWash, Price = 900m },
        new ItemCategory { CategoryId = 10, CategoryName = "Shirts", RecommendedCleaningType = CleaningType.Ironing, Price = 700m }
    ];

    /// <summary>Изделия, принадлежащие предопределённым категориям.</summary>
    public List<Item> Items { get; }

    /// <summary>Заказы, связанные с клиентами и изделиями в этом наборе данных.</summary>
    public List<Order> Orders { get; }

    /// <summary>Создаёт 10 клиентов, 10 категорий, 20 изделий и 20 заказов.</summary>
    public TestData()
    {
        Items =
        [
            new Item { ItemId = 1, Name = "Black Wool Coat", Material = "Wool", Category = ItemCategories[0] },
            new Item { ItemId = 2, Name = "Leather Boots", Material = "Leather", Category = ItemCategories[1] },
            new Item { ItemId = 3, Name = "Silk Scarf", Material = "Silk", Category = ItemCategories[2] },
            new Item { ItemId = 4, Name = "Living Room Carpet", Material = "Wool", Category = ItemCategories[3] },
            new Item { ItemId = 5, Name = "Velvet Curtains", Material = "Velvet", Category = ItemCategories[4] },
            new Item { ItemId = 6, Name = "Cotton Duvet", Material = "Cotton", Category = ItemCategories[5] },
            new Item { ItemId = 7, Name = "Evening Dress", Material = "Silk", Category = ItemCategories[6] },
            new Item { ItemId = 8, Name = "Leather Handbag", Material = "Leather", Category = ItemCategories[7] },
            new Item { ItemId = 9, Name = "Teddy Bear", Material = "Plush", Category = ItemCategories[8] },
            new Item { ItemId = 10, Name = "White Shirt", Material = "Cotton", Category = ItemCategories[9] },
            new Item { ItemId = 11, Name = "Winter Jacket", Material = "Polyester", Category = ItemCategories[0] },
            new Item { ItemId = 12, Name = "Suede Shoes", Material = "Suede", Category = ItemCategories[1] },
            new Item { ItemId = 13, Name = "Wool Hat", Material = "Wool", Category = ItemCategories[2] },
            new Item { ItemId = 14, Name = "Bedroom Carpet", Material = "Synthetic", Category = ItemCategories[3] },
            new Item { ItemId = 15, Name = "Linen Curtains", Material = "Linen", Category = ItemCategories[4] },
            new Item { ItemId = 16, Name = "Wool Blanket", Material = "Wool", Category = ItemCategories[5] },
            new Item { ItemId = 17, Name = "Cocktail Dress", Material = "Velvet", Category = ItemCategories[6] },
            new Item { ItemId = 18, Name = "Leather Belt", Material = "Leather", Category = ItemCategories[7] },
            new Item { ItemId = 19, Name = "Plush Rabbit", Material = "Plush", Category = ItemCategories[8] },
            new Item { ItemId = 20, Name = "Blue Shirt", Material = "Linen", Category = ItemCategories[9] }
        ];

        Orders =
        [
            new Order { Id = 1, Client = Clients[0], Item = Items[0], Status = OrderStatus.InProgress, ProcessingDays = 10, AcceptDate = new DateTime(2026, 9, 1) },
            new Order { Id = 2, Client = Clients[0], Item = Items[1], Status = OrderStatus.Issued, ProcessingDays = 5, AcceptDate = new DateTime(2026, 8, 1), CompletedDate = new DateTime(2026, 8, 20) },
            new Order { Id = 3, Client = Clients[0], Item = Items[2], Status = OrderStatus.Completed, ProcessingDays = 4, AcceptDate = new DateTime(2026, 7, 1), CompletedDate = new DateTime(2026, 7, 20) },
            new Order { Id = 4, Client = Clients[0], Item = Items[3], Status = OrderStatus.Issued, ProcessingDays = 7, AcceptDate = new DateTime(2026, 6, 1), CompletedDate = new DateTime(2026, 6, 15) },
            new Order { Id = 5, Client = Clients[0], Item = Items[4], Status = OrderStatus.Cancelled, ProcessingDays = 5, AcceptDate = new DateTime(2026, 5, 1) },
            new Order { Id = 6, Client = Clients[1], Item = Items[5], Status = OrderStatus.Issued, ProcessingDays = 5, AcceptDate = new DateTime(2026, 4, 1), CompletedDate = new DateTime(2026, 4, 5) },
            new Order { Id = 7, Client = Clients[1], Item = Items[6], Status = OrderStatus.Issued, ProcessingDays = 6, AcceptDate = new DateTime(2026, 3, 1), CompletedDate = new DateTime(2026, 3, 6) },
            new Order { Id = 8, Client = Clients[1], Item = Items[7], Status = OrderStatus.Issued, ProcessingDays = 8, AcceptDate = new DateTime(2026, 2, 1), CompletedDate = new DateTime(2026, 2, 11) },
            new Order { Id = 9, Client = Clients[1], Item = Items[8], Status = OrderStatus.InProgress, ProcessingDays = 7, AcceptDate = new DateTime(2026, 9, 10) },
            new Order { Id = 10, Client = Clients[2], Item = Items[9], Status = OrderStatus.Completed, ProcessingDays = 3, AcceptDate = new DateTime(2026, 1, 1), CompletedDate = new DateTime(2026, 1, 4) },
            new Order { Id = 11, Client = Clients[2], Item = Items[10], Status = OrderStatus.Issued, ProcessingDays = 5, AcceptDate = new DateTime(2025, 12, 1), CompletedDate = new DateTime(2025, 12, 8) },
            new Order { Id = 12, Client = Clients[2], Item = Items[11], Status = OrderStatus.InProgress, ProcessingDays = 6, AcceptDate = new DateTime(2026, 9, 5) },
            new Order { Id = 13, Client = Clients[3], Item = Items[12], Status = OrderStatus.Issued, ProcessingDays = 4, AcceptDate = new DateTime(2025, 11, 1), CompletedDate = new DateTime(2025, 11, 3) },
            new Order { Id = 14, Client = Clients[3], Item = Items[13], Status = OrderStatus.Issued, ProcessingDays = 8, AcceptDate = new DateTime(2025, 10, 1), CompletedDate = new DateTime(2025, 10, 20) },
            new Order { Id = 15, Client = Clients[4], Item = Items[14], Status = OrderStatus.Issued, ProcessingDays = 5, AcceptDate = new DateTime(2025, 9, 20), CompletedDate = new DateTime(2025, 9, 23) },
            new Order { Id = 16, Client = Clients[4], Item = Items[15], Status = OrderStatus.Completed, ProcessingDays = 5, AcceptDate = new DateTime(2025, 9, 1), CompletedDate = new DateTime(2025, 9, 5) },
            new Order { Id = 17, Client = Clients[5], Item = Items[16], Status = OrderStatus.Issued, ProcessingDays = 6, AcceptDate = new DateTime(2025, 8, 1), CompletedDate = new DateTime(2025, 8, 7) },
            new Order { Id = 18, Client = Clients[6], Item = Items[17], Status = OrderStatus.Issued, ProcessingDays = 7, AcceptDate = new DateTime(2025, 7, 1), CompletedDate = new DateTime(2025, 7, 9) },
            new Order { Id = 19, Client = Clients[7], Item = Items[18], Status = OrderStatus.Accepted, ProcessingDays = 5, AcceptDate = new DateTime(2026, 9, 12) },
            new Order { Id = 20, Client = Clients[8], Item = Items[19], Status = OrderStatus.Issued, ProcessingDays = 3, AcceptDate = new DateTime(2024, 12, 1), CompletedDate = new DateTime(2024, 12, 4) }
        ];
    }
}
