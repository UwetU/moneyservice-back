using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyService.Entities;
using MoneyService.Helpers;
using MoneyService.Models;
using MoneyService.Services;

namespace MoneyService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private ITransactionService _transactionService;
        private IMapper _mapper;

        public TransactionsController(
            ITransactionService transactionService,
            IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost("transfer")]
        public IActionResult TransferMoney([FromBody] TransactionModel model)
        {
            var transaction = _mapper.Map<Transaction>(model);

            try
            {
                _transactionService.Transfer(transaction);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        [HttpPost("refill")]
        public IActionResult RefillAccount([FromBody] TransactionModel model)
        {
            var transaction = _mapper.Map<Transaction>(model);

            try
            {
                _transactionService.Refill(transaction);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        [HttpGet("{userId}")]
        public IActionResult GetUserTransactions(int userId)
        {
            var accounts = _transactionService.GetUserTransactions(userId);
            var model = _mapper.Map<IList<TransactionModel>>(accounts);
            return Ok(model);
        }
    }
    
}