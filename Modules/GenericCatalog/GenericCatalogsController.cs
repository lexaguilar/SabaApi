namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GenericCatalogsController : ControllerBase
{
    private readonly IGenericCatalogsServices _genericCatalogServices;
    public GenericCatalogsController(IGenericCatalogsServices genericCatalogServices)
    {
        _genericCatalogServices = genericCatalogServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        var (success, genericCatalogs, message) = await _genericCatalogServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<GenericCatalogResponseModel>());
        return Ok(genericCatalogs);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        filters ??= new Dictionary<string, string>();
        filters.TryAdd("all-items", "true");
        var (success, genericCatalogs, message) = await _genericCatalogServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<GenericCatalogResponseModel>());
        return Ok(genericCatalogs.Items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, genericCatalog, message) = await _genericCatalogServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(genericCatalog);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GenericCatalogRequestModel model)
    {

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, genericCatalog, message) = await _genericCatalogServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(genericCatalog);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GenericCatalogRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, genericCatalog, message) = await _genericCatalogServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(genericCatalog);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, genericCatalog, message) = await _genericCatalogServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(genericCatalog);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, genericCatalog, message) = await _genericCatalogServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(genericCatalog);
    }
}
