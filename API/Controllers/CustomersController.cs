using AutoMapper;
using Entity.DTO_s;
using Entity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IRepositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepo _customerRepo;

        public CustomersController(IMapper mapper, ICustomerRepo customerRepo)
        {
            _mapper = mapper;
            _customerRepo = customerRepo;
        }

        // GET: api/customers
        [HttpGet]
        [Authorize(Policy = "ReadCustomers")]
        public async Task<ActionResult<List<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _customerRepo.GetAllAsync();
            var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
            return Ok(customerDtos);
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        [Authorize(Policy = "ReadCustomers")]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(string id)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with ID {id} not found");

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return Ok(customerDto);
        }

        // PUT: api/customers/5
        [HttpPut("{id}")]
        [Authorize(Policy = "ManageCustomers")]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(string id, [FromBody] UpdateCustomerDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = _mapper.Map<ApplicationUser>(updateDto);
            var updatedCustomer = await _customerRepo.UpdateAsync(id, customer);

            if (updatedCustomer == null)
                return NotFound($"Customer with ID {id} not found");

            var customerDto = _mapper.Map<CustomerDto>(updatedCustomer);
            return Ok(customerDto);
        }

        // DELETE: api/customers/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManageCustomers")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var result = await _customerRepo.DeleteAsync(id);
            if (!result)
                return NotFound($"Customer with ID {id} not found");

            return Ok(new { message = "Customer deleted successfully" });
        }

        // GET: api/customers/me
        [HttpGet("me")]
        public async Task<ActionResult<CustomerDto>> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customer = await _customerRepo.GetByIdAsync(userId);
            if (customer == null)
                return NotFound("Customer profile not found");

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return Ok(customerDto);
        }

        // PUT: api/customers/me
        [HttpPut("me")]
        public async Task<ActionResult<CustomerDto>> UpdateMyProfile([FromBody] UpdateCustomerDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var customer = _mapper.Map<ApplicationUser>(updateDto);
            var updatedCustomer = await _customerRepo.UpdateAsync(userId, customer);

            if (updatedCustomer == null)
                return NotFound("Customer profile not found");

            var customerDto = _mapper.Map<CustomerDto>(updatedCustomer);
            return Ok(customerDto);
        }
    }
}