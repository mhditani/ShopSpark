using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity.Models;
using Services.IRepositories;

namespace Services.Repositories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _db;

        public OrderRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

            // Calculate total amount from order items
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> CheckoutAsync(string userId)
        {
            // Since we don't have a cart, this can create an order directly
            // You might want to remove this method or modify it based on your needs
            var order = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = new List<OrderItem>() // Empty order for now
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetMyOrdersAsync(string userId)
        {
            return await _db.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> UpdateStatusAsync(int id, Order order)
        {
            var existingOrder = await _db.Orders.FindAsync(id);
            if (existingOrder == null)
                return null;

            existingOrder.Status = order.Status;
            await _db.SaveChangesAsync();
            return existingOrder;
        }
    }
}