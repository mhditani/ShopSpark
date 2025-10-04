using AutoMapper;
using Entity.DTO_s;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IRepositories;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepo _orderRepo;

        public OrdersController(IMapper mapper, IOrderRepo orderRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
        }

        // GET: api/orders
        [HttpGet]
        [Authorize(Policy = "ReadOrders")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderRepo.GetAllAsync();
            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        // GET: api/orders/my-orders
        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orders = await _orderRepo.GetMyOrdersAsync(userId);
            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            return Ok(orderDtos);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ReadOrders")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found");

            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }

        // POST: api/orders
        [HttpPost]
        [Authorize(Policy = "ManageOrders")]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var order = _mapper.Map<Order>(createOrderDto);

                // Set customer ID from current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                order.CustomerId = userId;

                var createdOrder = await _orderRepo.CreateAsync(order);
                var orderDto = _mapper.Map<OrderDto>(createdOrder);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, orderDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating order: {ex.Message}");
            }
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderDto>> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var order = await _orderRepo.CheckoutAsync(userId);
                if (order == null)
                    return BadRequest("Unable to create order");

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during checkout: {ex.Message}");
            }
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        [Authorize(Policy = "ManageOrders")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = _mapper.Map<Order>(statusDto);
            var updatedOrder = await _orderRepo.UpdateStatusAsync(id, order);

            if (updatedOrder == null)
                return NotFound($"Order with ID {id} not found");

            var orderDto = _mapper.Map<OrderDto>(updatedOrder);
            return Ok(orderDto);
        }
    }
}