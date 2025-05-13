namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilialsController : ControllerBase
{
    private readonly IFilialsServices _filialsServices;

    public FilialsController(IFilialsServices filialsServices)
    {
        _filialsServices = filialsServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, filials, message) = await _filialsServices.GetAll(skip, take, filters);
        if (!success)
            return Ok(Array.Empty<FilialRequestModel>());

        return Ok(new {
            items = filials.Items,
            totalCount = filials.TotalCount
        });
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