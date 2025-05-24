namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CatalogNamesController : ControllerBase
{
    private readonly ICatalogNamesServices _catalogNameServices;
    public CatalogNamesController(ICatalogNamesServices catalogNameServices)
    {
        _catalogNameServices = catalogNameServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, catalogNames, message) = await _catalogNameServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<CatalogNameResponseModel>());
        return Ok(catalogNames);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, catalogName, message) = await _catalogNameServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(catalogName);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CatalogNameRequestModel model)
    {

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, catalogName, message) = await _catalogNameServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(catalogName);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CatalogNameRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, catalogName, message) = await _catalogNameServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(catalogName);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, catalogName, message) = await _catalogNameServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(catalogName);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, catalogName, message) = await _catalogNameServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(catalogName);
    }
}
