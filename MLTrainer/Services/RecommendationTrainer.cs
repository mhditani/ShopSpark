using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLTrainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTrainer.Services
{
    public class RecommendationTrainer
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;

        public RecommendationTrainer(ApplicationDbContext context)
        {
            _context = context;
            _mlContext = new MLContext(seed: 0);

        }



        public async Task TrainAndSaveModelAsync()
        {
            Console.WriteLine("Step 1: Getting data from database...");

            // Get order data
            var interactions = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Product.IsActive)
                .Select(oi => new ProductInteraction
                {
                    UserId = oi.Order.CustomerId,
                    ProductId = oi.ProductId,
                    Rating = 3.0f // Simple rating for now
                })
                .ToListAsync();

            Console.WriteLine($"Found {interactions.Count} records");

            if (interactions.Count == 0)
            {
                Console.WriteLine("No data found. Please add some orders to your database.");
                return;
            }

            Console.WriteLine("Step 2: Training model...");

            // Convert to CSV
            var csv = "UserId,ProductId,Rating\n";
            foreach (var item in interactions)
            {
                csv += $"{item.UserId},{item.ProductId},{item.Rating}\n";
            }

            File.WriteAllText("training.csv", csv);

            // Load data into ML.NET
            var dataView = _mlContext.Data.LoadFromTextFile<ProductInteraction>(
                "training.csv",
                hasHeader: true,
                separatorChar: ',');

            // FIXED: This is the correct way to build the pipeline
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "UserIdEncoded",
                    inputColumnName: nameof(ProductInteraction.UserId))

                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "ProductIdEncoded",
                    inputColumnName: nameof(ProductInteraction.ProductId)))

                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    labelColumnName: nameof(ProductInteraction.Rating),
                    matrixColumnIndexColumnName: "UserIdEncoded",
                    matrixRowIndexColumnName: "ProductIdEncoded"));

            // Train model
            var model = pipeline.Fit(dataView);

            Console.WriteLine("Step 3: Saving model...");

            // Save model
            var modelDir = Path.Combine("..", "ShopSpark.API", "MLModels");
            Directory.CreateDirectory(modelDir);

            var modelPath = Path.Combine(modelDir, "ProductRecommendationModel.zip");
            _mlContext.Model.Save(model, dataView.Schema, modelPath);

            Console.WriteLine($"Model saved to: {modelPath}");
            Console.WriteLine("Model saved successfully!");

        }
    }
}
