using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IRepositories
{
    public interface IRecommendationService
    {
        Task<List<Product>> GetRecommendedProductsAsync(string userId, int topK = 5);
        float PredictProductRating(string userId, int productId);
        bool IsModelLoaded();
    }
}
