using AutoMapper;
using Entity.DTO_s;
using Entity.Models;
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


        [HttpGet("{id}")]
        [Authorize(Policy = "ReadProducts")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await productRepo.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<ProductDto>(product));
        }




        [HttpPost]
        [Authorize(Policy = "ManageProducts")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            var product = mapper.Map<Product>(productDto);
            await productRepo.CreateAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, mapper.Map<ProductDto>(product));
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "ManageProducts")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
           var productDomain =  mapper.Map<Product>(updateProductDto);
            if (productDomain == null)
            {
                return NotFound();
            }
            productDomain = await productRepo.UpdateAsync(productDomain, id);
            return Ok(mapper.Map<ProductDto>(productDomain));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageProducts")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedProduct = await productRepo.DeleteAsync(id);
            if (deletedProduct == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<ProductDto>(deletedProduct));
        }
    }
}
