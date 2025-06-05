namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly ISurveysServices _surveyServices;
    public SurveysController(ISurveysServices surveyServices)
    {
        _surveyServices = surveyServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        var (success, surveys, message) = await _surveyServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<SurveyResponseModel>());
        return Ok(surveys);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        filters ??= new Dictionary<string, string>();
        filters.TryAdd("all-items", "true");

        var (success, surveys, message) = await _surveyServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<SurveyResponseModel>());
        return Ok(surveys.Items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, survey, message) = await _surveyServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(survey);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SurveyRequestModel model)
    {

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, survey, message) = await _surveyServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SurveyRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, survey, message) = await _surveyServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, survey, message) = await _surveyServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, survey, message) = await _surveyServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpGet("{id}/StartSurvey")]
    public async Task<IActionResult> StartSurvey(int id)
    {
        var user = this.GetUser();
        var (success, survey, message) = await _surveyServices.StartSurvey(id, user.Id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpGet("{id}/FinishSurvey")]
    public async Task<IActionResult> FinishSurvey(int id)
    {
        var user = this.GetUser();
        var (success, survey, message) = await _surveyServices.FinishSurvey(id, user.Id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpGet("{id}/PauseSurvey")]
    public async Task<IActionResult> PauseSurvey(int id)
    {
        var user = this.GetUser();
        var (success, survey, message) = await _surveyServices.PauseSurvey(id, user.Id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpGet("{id}/ResumeSurvey")]
    public async Task<IActionResult> ResumeSurvey(int id)
    {
        var user = this.GetUser();
        var (success, survey, message) = await _surveyServices.ResumeSurvey(id, user.Id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

}
