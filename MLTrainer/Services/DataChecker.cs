using Entity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTrainer.Services
{
    public class DataChecker
    {
        private readonly ApplicationDbContext _context;

        public DataChecker(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task CheckDataAsync()
        {
            Console.WriteLine("📊 CHECKING DATABASE DATA");
            Console.WriteLine("=========================");

            var products = await _context.Products.ToListAsync();
            var orders = await _context.Orders.ToListAsync();
            var orderItems = await _context.OrderItems.ToListAsync();
            var users = await _context.Users.ToListAsync();

            Console.WriteLine($"📦 Products: {products.Count}");
            foreach (var product in products)
            {
                Console.WriteLine($"   - {product.Name} (ID: {product.Id}, Active: {product.IsActive})");
            }

            Console.WriteLine($"🛒 Orders: {orders.Count}");
            foreach (var order in orders)
            {
                Console.WriteLine($"   - Order {order.Id} by User {order.CustomerId} - ${order.TotalAmount}");
            }

            Console.WriteLine($"📋 Order Items: {orderItems.Count}");
            foreach (var item in orderItems)
            {
                Console.WriteLine($"   - Order {item.OrderId}, Product {item.ProductId}, Qty: {item.Quantity}");
            }

            Console.WriteLine($"👥 Users: {users.Count}");
            foreach (var user in users.Take(5)) // Show first 5 users
            {
                Console.WriteLine($"   - {user.UserName} (ID: {user.Id})");
            }

            Console.WriteLine("=========================");
        }
    }
}