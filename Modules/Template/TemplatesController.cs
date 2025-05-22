namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplatesServices _templatesServices;

    public TemplatesController(ITemplatesServices templatesServices)
    {
        _templatesServices = templatesServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, templates, message) = await _templatesServices.GetAll(skip, take, filters);
        if (!success)
            return Ok(Array.Empty<TemplateResponseModel>());

        return Ok(new {
            items = templates.Items,
            totalCount = templates.TotalCount
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, template, message) = await _templatesServices.GetById(id);
        if (!success)
            return NotFound(new { message });

        return Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TemplateRequestModel templateRequestModel)
    {
        var (success, template, message) = await _templatesServices.Add(templateRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(template);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TemplateRequestModel templateRequestModel)
    {
        if (id != templateRequestModel.Id)
            return BadRequest(new { message = "Filial ID mismatch." });

        var (success, template, message) = await _templatesServices.Update(templateRequestModel);
        if (!success)
            return BadRequest(new { message });

        return Ok(template);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {

        var (success, role, message) = await _templatesServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(role);

    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {

        var (success, role, message) = await _templatesServices.Enable(id);
        if (!success) return BadRequest(message);
        return Ok(role);

    }
}