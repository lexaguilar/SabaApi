namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Domain.ViewModels;
using Saba.Application.Services;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CatalogsController : ControllerBase
{
    private readonly ICatalogsServices _catalogsServices;

    public CatalogsController(ICatalogsServices catalogsServices)
    {
        _catalogsServices = catalogsServices;
    }

    [HttpGet("{name}")]
    public IActionResult GenericsCatalogs(string name)
    {
        var result =_catalogsServices.GetAll(name);

        return Ok(result);
    }
}
