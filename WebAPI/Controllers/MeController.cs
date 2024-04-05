using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public MeController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _customerService.GetCustomerDetails(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}
