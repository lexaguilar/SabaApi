namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilialsController : ControllerBase
{
    private readonly IFilialsServices _filialsServices;
    private readonly IFilialUsersServices _filialUsersServices;

    public FilialsController(IFilialsServices filialsServices, IFilialUsersServices filialUsersServices)
    {
        _filialsServices = filialsServices;
        _filialUsersServices = filialUsersServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var user = this.GetUser();
        filters = ObjectExtensions.AddCountry(filters, user.Resources, user.CountryId);
        
        var (success, filials, message) = await _filialsServices.GetAll(skip, take, filters);
        if (!success)
            return Ok(Array.Empty<TemplateResponseModel>());

        return Ok(new {
            items = filials.Items,
            totalCount = filials.TotalCount
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var user = this.GetUser();

        filters = ObjectExtensions.AddCountry(filters, user.Resources, user.CountryId);

        filters.TryAdd("all-items", "true");

        var (success, filials, message) = await _filialsServices.GetAll(skip, take, filters);

        var userFilials = await _filialUsersServices.GetByUserAll(user.Id);

        if (userFilials.success)
        {
            var filialsFiltred = filials.Items.Where(f => userFilials.filialUserResult.Items.Any(uf => uf.FilialId == f.Id)).ToList();
            return Ok(filialsFiltred);
        }
            

        if (!success)
            return Ok(Array.Empty<FilialResponseModel>());

        return Ok(filials.Items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, filial, message) = await _filialsServices.GetById(id);
        if (!success)
            return NotFound(new { message });

        return Ok(filial);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FilialRequestModel filialRequestModel)
    {
        var user = this.GetUser();
        var (success, filial, message) = await _filialsServices.Add(filialRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(filial);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FilialRequestModel filialRequestModel)
    {
        if (id != filialRequestModel.Id)
            return BadRequest(new { message = "Filial ID mismatch." });

        var (success, filial, message) = await _filialsServices.Update(filialRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(filial);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {

        var (success, role, message) = await _filialsServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(role);

    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {

        var (success, role, message) = await _filialsServices.Enable(id);
        if (!success) return BadRequest(message);
        return Ok(role);

    }
}