using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity.Models;
using Services.IRepositories;

namespace Services.Repositories
{
    public class ProductRepo : IProductRepo
    {
        private readonly ApplicationDbContext _db;

        public ProductRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _db.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = true;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateAsync(Product product, int id)
        {
            var existingProduct = await _db.Products.FindAsync(id);
            if (existingProduct == null || !existingProduct.IsActive)
                return null;

            // Update only the allowed properties
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<Product?> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null || !product.IsActive)
                return null;

            // Soft delete (recommended)
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            // Or hard delete if you prefer:
            // _db.Products.Remove(product);

            await _db.SaveChangesAsync();
            return product;
        }

        // Additional useful methods
        public async Task<List<Product>> GetProductsByStockAsync(int minStock = 0)
        {
            return await _db.Products
                .Where(p => p.IsActive && p.StockQuantity >= minStock)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _db.Products
                .AnyAsync(p => p.Id == id && p.IsActive);
        }

        public async Task UpdateStockAsync(int productId, int quantityChange)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product != null && product.IsActive)
            {
                product.StockQuantity += quantityChange;
                product.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
    }
}