using Microsoft.AspNetCore.Mvc;
using UserConfirmation.Services.Accounts;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _accountService.RegisterUserAsync(model);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var callbackUrl = await _accountService.LoginUserAsync(model);
            if (callbackUrl != null)
            {
                return Ok(new { callbackUrl });
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromQuery] string userId, [FromQuery] string code)
        {
            var result = await _accountService.ConfirmUserAsync(userId, code);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest("Invalid confirmation code.");
        }
    }
}
