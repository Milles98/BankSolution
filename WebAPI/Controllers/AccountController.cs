using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public AccountController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{id}/{limit}/{offset}")]
        public async Task<IActionResult> Get(int id, int limit, int offset)
        {
            var transactions = await _transactionService.GetTransactionsForAccount(id, null);
            if (transactions == null || transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions.Skip(offset).Take(limit));
        }
    }
}
