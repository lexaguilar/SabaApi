using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

namespace Saba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRolesServices _rolesServices;

    public RolesController(IRolesServices rolesServices)
    {
        _rolesServices = rolesServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int page = 0, int pageSize = 10)
    {
        var user = this.GetUser();
        var filters = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
        filters = ObjectExtensions.AddCountry(filters, user.Resources, user.CountryId);
        
        var (success, roles, message) = await _rolesServices.GetAll(page, pageSize, filters);

        if (!success) return Ok(Array.Empty<FilialRequestModel>());

        return Ok(new
        {
            items = roles.Items,
            totalCount = roles.TotalCount
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        var user = this.GetUser();

        filters = ObjectExtensions.AddCountry(filters, user.Resources, user.CountryId);

        filters.TryAdd("all-items", "true");

        var (success, roles, message) = await _rolesServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<RoleResponseModel>());
        return Ok(roles.Items);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, role, message) = await _rolesServices.GetById(id);
        if (!success) return BadRequest(new { message });
        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleRequestModel model)
    {
        var (success, role, message) = await _rolesServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(role);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RoleRequestModel model)
    {
        var user = this.GetUser();

        model.UserId = user.Id;

        var (success, role, message) = await _rolesServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(role);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, role, message) = await _rolesServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(role);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, role, message) = await _rolesServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(role);
    }

    [HttpPost("UpdateResources/{id}")]
    public async Task<IActionResult> UpdateResources(int id, [FromBody] ResourcesRequestModel model)
    {

        var (success, role, message) = await _rolesServices.UpdateResources(id, model);
        if (!success) return BadRequest(new { message });

        return Ok(role);
    }
}