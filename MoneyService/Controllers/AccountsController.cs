using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyService.Entities;
using MoneyService.Helpers;
using MoneyService.Models.Accounts;
using MoneyService.Services;

namespace MoneyService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private IAccountService _accountService;
        private IMapper _mapper;

        public AccountsController(
            IAccountService accountService,
            IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public IActionResult Create()
        {
            var account = new Account();
            account.UserId = GetUserId();

            try
            {
                _accountService.Create(account);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserAccounts(int userId)
        {
            var accounts = _accountService.GetUserAccounts(userId);
            var model = _mapper.Map<IList<AccountModel>>(accounts);
            return Ok(model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CloseModel model)
        {
            var account = _mapper.Map<Account>(model);
            account.Id = id;
            
            try
            {
                _accountService.Close(account);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        
        public int GetUserId()
        {
            var userId = HttpContext.User?.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("UserId пуст");
            }

            return int.Parse(userId);
        }
    }
}