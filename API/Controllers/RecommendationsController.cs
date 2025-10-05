using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IRepositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet("for-me")]
        public async Task<IActionResult> GetRecommendedProducts([FromQuery] int count = 5)
        {
            try
            {
                // For testing - use a hardcoded user ID
                var userId = "test-user-1"; // Change this later to get from JWT token

                var recommendations = await _recommendationService.GetRecommendedProductsAsync(userId, count);

                return Ok(new
                {
                    Success = true,
                    Message = $"Found {recommendations.Count} products",
                    ModelLoaded = _recommendationService.IsModelLoaded(),
                    Products = recommendations.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Price,
                        p.Description
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Success = true,
                ModelLoaded = _recommendationService.IsModelLoaded(),
                Message = "API is working - ML model not loaded yet"
            });
        }
    }
}