namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IUsersServices _usersServices;
    private readonly IFilialsServices _filialsServices;
    private readonly ISurveysServices _surveysServices;
    private readonly ISurveyUsersServices _surveyUsersServices;

    public DashboardController(IUsersServices usersServices, IFilialsServices filialsServices, ISurveysServices surveysServices, ISurveyUsersServices surveyUsersServices)
    {
        _usersServices = usersServices;
        _filialsServices = filialsServices;
        _surveysServices = surveysServices;
        _surveyUsersServices = surveyUsersServices;
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var dictionary = new Dictionary<string, string>
        {
            { "all-items", "true" }
        };
        var (success, users, message) = await _usersServices.GetAll(0, 0, dictionary);
        if (!success)
            return NotFound(new { message });

        return Ok(new
        {
            Name = "Supervisores",
            Value = users.Items.Count(u => u.RoleId != 1),
            Extra = new
            {
                Name = "Total Usuarios",
                Value = users.Items.Count(),
            }
        });
    }

    [HttpGet("filials")]
    public async Task<IActionResult> Filials()
    {
        var dictionary = new Dictionary<string, string>
        {
            { "all-items", "true" }
        };
        var (success, filials, message) = await _filialsServices.GetAll(0, 1000, dictionary);
        if (!success)
            return NotFound(new { message });

        return Ok(new
        {
            Name = "Sucursales",
            Value = filials.Items.Count(),
            Extra = new
            {
                Name = "Zonas",
                Value = filials.Items.Count(),
            }
        });
    }

    [HttpGet("surveys")]
    public async Task<IActionResult> Surveys()
    {
        var dictionary = new Dictionary<string, string>
        {
            { "all-items", "true" }
        };
        var (success, surveys, message) = await _surveysServices.GetAll(0, 1000, dictionary);
        if (!success)
            return NotFound(new { message });

        return Ok(new
        {
            Name = "Formatos Encuestas",
            Value = surveys.Items.Count(),
            Extra = new
            {
                Name = "Activas",
                Value = surveys.Items.Count(s => s.Active),
            }
        });
    }

    [HttpGet("survey-users")]
    public async Task<IActionResult> SurveyUsers()
    {
        var dictionary = new Dictionary<string, string>
        {
            { "all-items", "true" }
        };
        var (success, surveyUsers, message) = await _surveyUsersServices.GetAll(0, 1000, dictionary);
        if (!success)
            return NotFound(new { message });

        return Ok(new
        {
            Name = "Encuestas Realizadas",
            Value = surveyUsers.Items.Count(),
            Extra = new
            {
                Name = "Pendientes",
                Value = surveyUsers.Items.Count(su => su.SurveyUserStateId == 1),
            }
        });
    }

}