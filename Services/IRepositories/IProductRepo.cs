using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IRepositories
{
    public interface IProductRepo
    {
        Task<List<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product product);

        Task<Product?> UpdateAsync(Product product, int id);

        Task<Product?> DeleteAsync(int id);


        // Optional: Add these to your interface if you want to use them
        Task<List<Product>> GetProductsByStockAsync(int minStock = 0);
        Task<bool> ProductExistsAsync(int id);
        Task UpdateStockAsync(int productId, int quantityChange);
    }
}
