using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

namespace Saba.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFilesServices _filesServices;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFilesServices filesServices, ILogger<FilesController> logger)
    {
        _logger = logger;
        _filesServices = filesServices;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromQuery] int id, IFormFile file, [FromQuery] int surveyUserResponseId)
    {
        _logger.LogInformation("Adding file for SurveyUserResponseId: {SurveyUserResponseId}", surveyUserResponseId);

        try
        {

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Archivo no proporcionado." });

            var model = new FileRequestModel
            {
                Id = id,
                File = file,
            };

            var (success, fileUploaded, message) = await _filesServices.Add(model);
            if (!success) return BadRequest(new { message });
            return Ok(new { fileUploaded });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar el archivo.");
            return StatusCode(500, new { message = "Error al procesar el archivo." });
        }
    }


    [HttpGet("{id}/remove")]
    public async Task<IActionResult> Remove(int id)
    {
        var (success, fileDeleted, message) = await _filesServices.Remove(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { fileDeleted });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var (success, fileInfo, message) = await _filesServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(fileInfo);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int surveyUserResponseId)
    {
        var (success, filesInfo, message) = await _filesServices.GetAll(surveyUserResponseId);
        if (!success) return NotFound(new { message });
        return Ok(filesInfo);
    }

}