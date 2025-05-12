namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

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

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, users, message) = await _usersServices.GetAll(skip, take, filters);
        if (!success)
            return Ok(Array.Empty<UserResponseModel>());

        return Ok(new {
            items = users,
            totalCount = users?.Count() ?? 0
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, user, message) = await _usersServices.GetById(id);
        if (!success)
            return NotFound(new { message });

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserRequestModel userRequestModel)
    {
        var (success, user, message) = await _usersServices.Add(userRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(user);
    }
    

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserRequestModel userRequestModel)
    {
        if (id != userRequestModel.Id)
            return BadRequest(new { message = "User ID mismatch." });

        var (success, user, message) = await _usersServices.Update(userRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(user);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {

        var (success, user, message) = await _usersServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(user);

    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {

        var (success, user, message) = await _usersServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(user);

    }
}
