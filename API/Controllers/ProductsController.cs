using AutoMapper;
using Entity.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IRepositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IProductRepo productRepo;

        public ProductsController(IMapper mapper, IProductRepo productRepo)
        {
            this.mapper = mapper;
            this.productRepo = productRepo;
        }




        [HttpGet]
        [Authorize(Policy = "ReadProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await productRepo.GetAllAsync();
            return Ok(mapper.Map<List<ProductDto>>(products));
        }

    }
}
