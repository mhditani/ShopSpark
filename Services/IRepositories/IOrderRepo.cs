using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IRepositories
{
    public interface IOrderRepo
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> CheckoutAsync(string userId);              // atomic: cart -> order
        Task<List<Order>> GetMyOrdersAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task<Order?> UpdateStatusAsync(int id, Order order);
    }
}
