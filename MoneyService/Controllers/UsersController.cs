using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using MoneyService.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MoneyService.Services;
using MoneyService.Entities;
using MoneyService.Models.Users;

namespace MoneyService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Неправильно введен email или пароль"});
            
            var authClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _appSettings.Issuer,
                audience: _appSettings.Audience,
                expires: DateTime.Now.AddMinutes(_appSettings.ExpiresMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecureKey)),
                    SecurityAlgorithms.HmacSha256Signature)
            );
            
            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterModel model)
        {
            var user = _mapper.Map<User>(model);

            try
            {
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /*[AllowAnonymous]
        [HttpPost("addAccount")]
        public IActionResult AddAccount()
        {
            var account = new Account();
            account.UserId = GetUserId();

            try
            {
                _userService.CreateAccount(account);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }*/
        
        /*[AllowAnonymous]
        [HttpPost("refillAccount")]
        public IActionResult RefillAccount([FromBody] TransactionModel model)
        {
            var transaction = _mapper.Map<Transaction>(model);

            try
            {
                _userService.Refill(transaction);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }*/

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }
    
        /*[HttpGet("accounts/{userId}")]
        public IActionResult GetUserAccounts(int userId)
        {
            var accounts = _userService.GetUserAccounts(userId);
            var model = _mapper.Map<IList<AccountModel>>(accounts);
            return Ok(model);
        }*/

        /*[HttpGet("transactions/{userId}")]
        public IActionResult GetUserTransactions(int userId)
        {
            var accounts = _userService.GetUserTransactions(userId);
            var model = _mapper.Map<IList<TransactionModel>>(accounts);
            return Ok(model);
        }*/

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UpdateModel model)
        {
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                _userService.Update(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
        
        /*public int GetUserId()
        {
            var userId = HttpContext.User?.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("UserId пуст");
            }

            return int.Parse(userId);
        }*/
    }
}