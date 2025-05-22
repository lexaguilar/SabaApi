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
    private readonly ITemplatesServices _templateServices;
    public TemplatesController(ITemplatesServices templateServices)
    {
        _templateServices = templateServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, templates, message) = await _templateServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<TemplateResponseModel>());
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, template, message) = await _templateServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TemplateRequestModel model)
    {
        var (success, template, message) = await _templateServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(template);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TemplateRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });
        var (success, template, message) = await _templateServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(template);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, template, message) = await _templateServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(template);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, template, message) = await _templateServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(template);
    }
}
