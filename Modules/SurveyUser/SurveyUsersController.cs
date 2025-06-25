namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SurveyUsersController : ControllerBase
{
    private readonly ISurveyUsersServices _surveyUserServices;
    public SurveyUsersController(ISurveyUsersServices surveyUserServices)
    {
        _surveyUserServices = surveyUserServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        var (success, surveyUsers, message) = await _surveyUserServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<SurveyUserResponseModel>());
        return Ok(surveyUsers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, surveyUser, message) = await _surveyUserServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(surveyUser);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SurveyUserRequestModel model)
    {

        var user = this.GetUser();
        model.UserEditId = user.Id;

        var (success, surveyUser, message) = await _surveyUserServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(surveyUser);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SurveyUserRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

        var user = this.GetUser();
        model.UserEditId = user.Id;

        var (success, surveyUser, message) = await _surveyUserServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(surveyUser);
    }

    [HttpGet("{id}/remove")]
    public async Task<IActionResult> Remove(int id)
    {
        var (success, surveyUser, message) = await _surveyUserServices.Remove(id);
        if (!success) return BadRequest(new { message });
        return Ok(surveyUser);
    }

    [HttpGet("getTotalCountByState")]
    public async Task<IActionResult> GetTotalCountByState(int stateId)
    {
        var user = this.GetUser();

        if (user.RoleId != 1) // Assuming 1 is the Admin role
        {
            var (success, totalCount, message) = await _surveyUserServices.GetTotalCountByState(user.Id, stateId);
            return Ok(totalCount);
        }
        else
        {
            var (success, totalCount, message) = await _surveyUserServices.GetTotalCountByState(null, stateId);
            return Ok(totalCount);
        }
    }

    [HttpPost("{id}/FinishSurvey")]
    public async Task<IActionResult> FinishSurvey([FromBody] FinishSurveyUserRequestModel model)
    {
        var user = this.GetUser();
        var (success, survey, message) = await _surveyUserServices.FinishSurvey(model, user.Id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }

    [HttpPost("{id}/ResumeSurvey")]
    public async Task<IActionResult> ResumeSurvey([FromBody] FinishSurveyUserRequestModel model)
    {
        var user = this.GetUser();
        var (success, survey, message) = await _surveyUserServices.ResumeSurvey(model, user.Id);
        if (!success) return BadRequest(new { message });
        return Ok(survey);
    }
}
