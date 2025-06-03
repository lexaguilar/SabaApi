namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SurveyUserResponsesController : ControllerBase
{
    private readonly ISurveyUserResponsesServices _surveyUserResponseServices;
    public SurveyUserResponsesController(ISurveyUserResponsesServices surveyUserResponseServices)
    {
        _surveyUserResponseServices = surveyUserResponseServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, surveyUserResponses, message) = await _surveyUserResponseServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<SurveyUserResponseResponseModel>());
        return Ok(surveyUserResponses);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        filters ??= new Dictionary<string, string>();
        filters.TryAdd("all-items", "true");
        var (success, surveyUserResponses, message) = await _surveyUserResponseServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<SurveyUserResponseResponseModel>());
        return Ok(surveyUserResponses.Items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, surveyUserResponse, message) = await _surveyUserResponseServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(surveyUserResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SurveyUserResponseRequestModel model)
    {

        var (success, surveyUserResponse, message) = await _surveyUserResponseServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(surveyUserResponse);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SurveyUserResponseRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

        var (success, surveyUserResponse, message) = await _surveyUserResponseServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(surveyUserResponse);
    }
}
