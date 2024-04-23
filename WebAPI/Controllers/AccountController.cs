using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(ITransactionService transactionService) : ControllerBase
    {
        [HttpGet("{id}/{limit}/{offset}")]
        public async Task<IActionResult> Get(int id, int limit, int offset)
        {
            var transactions = await transactionService.GetTransactionsForAccount(id);
            if (transactions == null || transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions.Skip(offset).Take(limit));
        }
    }
}
