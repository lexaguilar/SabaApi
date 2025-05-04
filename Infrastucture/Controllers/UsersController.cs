namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Repository;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersServices _usersServices;

    public UsersController(IUsersServices usersServices)
    {
        _usersServices = usersServices;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Get(int skip, int take, Dictionary<string, string> filters)
    {
        var result = await _usersServices.GetAll(skip, take, filters);
        if (!result.Success)
            return NotFound(new { message = "No users found." });

        return new JsonResult(result.Users);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetById(string username)
    {
        var result = await _usersServices.GetByUserName(username);
        if (!result.Success)
            return NotFound(new { message = "User not found." });

        return new JsonResult(result.User);
    }
}
