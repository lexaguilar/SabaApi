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
        var result = await _filialsServices.GetAll(skip, take, filters);
        if (!result.success)
            return NotFound(new { message = "No filials found." });

        return new JsonResult(new {
            items = result.filials,
            totalCount = result.filials?.Count() ?? 0
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _filialsServices.GetById(id);
        if (!result.success)
            return NotFound(new { message = "Filial not found." });

        return new JsonResult(result.filial);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FilialRequestModel filialRequestModel)
    {
        var result = await _filialsServices.Add(filialRequestModel);
        if (!result.success)
            return BadRequest(new { message = result.errorMsg });

        return new JsonResult(result.filial);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FilialRequestModel filial)
    {
        if (id != filial.Id)
            return BadRequest(new { message = "Filial ID mismatch." });

        var result = await _filialsServices.Update(filial);
        if (!result.success)
            return NotFound(new { message = result.errorMsg });

        return new JsonResult(result.filial);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {

        await _filialsServices.Disable(id);
        return new JsonResult(new { message = "Filial disabled." });

    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {

        await _filialsServices.Enable(id);
        return new JsonResult(new { message = "Filial disabled." });

    }
}