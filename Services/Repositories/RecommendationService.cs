using Entity.Data;
using Entity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Services.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Services.Repositories
{
    public class RecommendationService : IRecommendationService
    {
        private readonly PredictionEngine<ProductInteraction, ProductPrediction>? _predictionEngine;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(ApplicationDbContext context, ILogger<RecommendationService> logger)
        {
            _context = context;
            _logger = logger;

            // We'll load the ML model here later
            _logger.LogInformation("Recommendation service created");
        }

        public bool IsModelLoaded()
        {
            return _predictionEngine != null;
        }

        public float PredictProductRating(string userId, int productId)
        {
            // We'll implement this later
            return 3.0f; // Default rating for now
        }

        public async Task<List<Product>> GetRecommendedProductsAsync(string userId, int topK = 5)
        {
            // For now, just return popular products as fallback
            return await _context.Products
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.StockQuantity)
                .Take(topK)
                .ToListAsync();
        }
    }

    // ML data models - add these inside the class
    public class ProductInteraction
    {
        public string UserId { get; set; } = string.Empty;
        public float ProductId { get; set; }
        public float Rating { get; set; }
    }

    public class ProductPrediction
    {
        [ColumnName("Score")]
        public float PredictedRating { get; set; }
    }
}

