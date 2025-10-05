using Entity.Data;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML.Runtime;
using MLTrainer.Services;


// Simple setup
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer("Data Source=localhost;Initial Catalog=ShopSparkDB;Integrated Security=True;Trust Server Certificate=True"));
    })
    .Build();

try
{
    Console.WriteLine("🛠️ COMPLETE SAMPLE DATA SETUP");
    Console.WriteLine("==============================");

    using var scope = host.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Check current data
    Console.WriteLine("📊 CURRENT DATA:");
    Console.WriteLine($"Products: {await context.Products.CountAsync()}");
    Console.WriteLine($"Users: {await context.Users.CountAsync()}");
    Console.WriteLine($"Orders: {await context.Orders.CountAsync()}");
    Console.WriteLine($"Order Items: {await context.OrderItems.CountAsync()}");

    // If no users, we'll create sample orders with fake user IDs
    var users = await context.Users.ToListAsync();

    if (!users.Any())
    {
        Console.WriteLine("\n⚠️ No real users found. Creating sample data with test user IDs...");

        // Create sample orders with test user IDs (these should match real users you'll create later)
        var testUserIds = new List<string>
        {
            "test-user-1",  // This should match a real user ID later
            "test-user-2"   // This should match a real user ID later
        };

        Console.WriteLine("🛒 Adding sample orders...");

        var orders = new List<Order>
        {
            new Order { CustomerId = testUserIds[0], TotalAmount = 1299.98m, Status = "Delivered", OrderDate = DateTime.UtcNow.AddDays(-30) },
            new Order { CustomerId = testUserIds[0], TotalAmount = 2399.99m, Status = "Delivered", OrderDate = DateTime.UtcNow.AddDays(-15) },
            new Order { CustomerId = testUserIds[1], TotalAmount = 699.99m, Status = "Processing", OrderDate = DateTime.UtcNow.AddDays(-2) }
        };

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();

        // Add order items
        var savedOrders = await context.Orders.ToListAsync();
        var savedProducts = await context.Products.ToListAsync();

        var orderItems = new List<OrderItem>
        {
            // Order 1: iPhone + Headphones
            new OrderItem { OrderId = savedOrders[0].Id, ProductId = savedProducts[0].Id, Quantity = 1, UnitPrice = 999.99m },
            new OrderItem { OrderId = savedOrders[0].Id, ProductId = savedProducts[3].Id, Quantity = 1, UnitPrice = 299.99m },
            
            // Order 2: MacBook Pro
            new OrderItem { OrderId = savedOrders[1].Id, ProductId = savedProducts[2].Id, Quantity = 1, UnitPrice = 2399.99m },
            
            // Order 3: Samsung Phone (by user 2)
            new OrderItem { OrderId = savedOrders[2].Id, ProductId = savedProducts[1].Id, Quantity = 1, UnitPrice = 699.99m }
        };

        await context.OrderItems.AddRangeAsync(orderItems);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Added sample orders and order items");

        // Final data count
        Console.WriteLine("\n📊 FINAL DATA COUNT:");
        Console.WriteLine($"Products: {await context.Products.CountAsync()}");
        Console.WriteLine($"Users: {await context.Users.CountAsync()}");
        Console.WriteLine($"Orders: {await context.Orders.CountAsync()}");
        Console.WriteLine($"Order Items: {await context.OrderItems.CountAsync()}");

        Console.WriteLine("\n💡 IMPORTANT: When you register real users later,");
        Console.WriteLine("   update the CustomerId in Orders table to match real user IDs.");
    }
    else
    {
        Console.WriteLine("\n✅ Users already exist. You can now run the ML Trainer!");
    }

}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();