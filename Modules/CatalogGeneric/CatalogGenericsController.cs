namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CatalogGenericsController : ControllerBase
{
    private readonly ICatalogGenericsServices _catalogGenericsServices;

    public CatalogGenericsController(ICatalogGenericsServices catalogGenericsServices)
    {
        _catalogGenericsServices = catalogGenericsServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, catalogs, message) = await _catalogGenericsServices.GetAll(skip, take, filters);
        if (!success)
            return Ok(Array.Empty<GenericCatalogRequestModel>());

        return Ok(new {
            items = catalogs.Items,
            totalCount = catalogs.TotalCount
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, catalog, message) = await _catalogGenericsServices.GetById(id);
        if (!success)
            return NotFound(new { message });

        return Ok(catalog);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GenericCatalogRequestModel catalogRequestModel)  
    {
        var (success, catalog, message) = await _catalogGenericsServices.Add(catalogRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(catalog);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GenericCatalogRequestModel catalogRequestModel)
    {
        if (id != catalogRequestModel.Id)
            return BadRequest(new { message = "Catalog ID mismatch." });

        var (success, catalog, message) = await _catalogGenericsServices.Update(catalogRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(catalog);
    }
}