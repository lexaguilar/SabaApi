namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Domain.ViewModels;
using Saba.Application.Services;
using Saba.Application.Extensions;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;

    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await accountService.Login(model);

        if (!result.success)
            return BadRequest(new { message = result.message });

        return Ok(result.User);
    }

    [Authorize]
    [HttpGet("sign-in-with-token")]
    public IActionResult SignInWithToken()
    {
        return Ok(new { message = "sign-in-with-token" });
    }
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var user = this.GetUser();

        var result = await accountService.ChangePassword(model);

        if (!result.success)
            return BadRequest(new { message = result.message });

        return Ok(new { message = "Password changed successfully." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel userModel)
    {
        var result = await accountService.ResetPassword(userModel.Username);

        if (!result.success)
            return BadRequest(new { message = result.message });

        return Ok(new { message = "Password reset email sent." });
    }
}
