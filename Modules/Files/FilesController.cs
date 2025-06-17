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

    public FilesController(IFilesServices filesServices)
    {
        _filesServices = filesServices;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromQuery] int id, IFormFile file, [FromQuery] int surveyUserResponseId)
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