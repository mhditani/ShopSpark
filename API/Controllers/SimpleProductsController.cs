using Microsoft.AspNetCore.Mvc;

namespace ShopSpark.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Simple products controller works!");
        }
    }
}