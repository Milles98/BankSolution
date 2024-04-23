using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MeController(ICustomerService customerService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await customerService.GetCustomerDetails(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}
