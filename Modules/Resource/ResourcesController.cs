namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly IResourcesServices _resourceServices;
    public ResourcesController(IResourcesServices resourceServices)
    {
        _resourceServices = resourceServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, resources, message) = await _resourceServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<ResourceResponseModel>());
        return Ok(resources.Items);
    }
   
   
}
