using AutoMapper;
using Entity.DTO_s;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IRepositories;

namespace ShopSpark.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepo productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        // GET: api/products
        [HttpGet]
        [Authorize(Policy = "ReadProducts")]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            var products = await _productRepo.GetAllAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ReadProducts")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
                return NotFound($"Product with ID {id} not found");

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Policy = "ManageProducts")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = _mapper.Map<Product>(createDto);
                var createdProduct = await _productRepo.CreateAsync(product);
                var productDto = _mapper.Map<ProductDto>(createdProduct);

                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, productDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating product: {ex.Message}");
            }
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        [Authorize(Policy = "ManageProducts")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = await _productRepo.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound($"Product with ID {id} not found");

            // Use AutoMapper to update the existing product
            _mapper.Map(updateDto, existingProduct);
            var updatedProduct = await _productRepo.UpdateAsync(existingProduct, id);

            if (updatedProduct == null)
                return NotFound($"Product with ID {id} not found");

            var productDto = _mapper.Map<ProductDto>(updatedProduct);
            return Ok(productDto);
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedProduct = await _productRepo.DeleteAsync(id);
            if (deletedProduct == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(new { message = "Product deleted successfully" });
        }

        // GET: api/products/stock?minStock=10
        [HttpGet("stock")]
        [Authorize(Policy = "ReadProducts")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByStock([FromQuery] int minStock = 0)
        {
            var products = await _productRepo.GetProductsByStockAsync(minStock);
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }

        // PATCH: api/products/5/stock
        [HttpPatch("{id}/stock")]
        [Authorize(Policy = "ManageProducts")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateDto stockDto)
        {
            await _productRepo.UpdateStockAsync(id, stockDto.QuantityChange);

            var updatedProduct = await _productRepo.GetByIdAsync(id);
            if (updatedProduct == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(new
            {
                message = "Stock updated successfully",
                currentStock = updatedProduct.StockQuantity
            });
        }
    }
}