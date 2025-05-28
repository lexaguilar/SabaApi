namespace Saba.Infrastructure.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TemplateQuestionsController : ControllerBase
{
    private readonly ITemplateQuestionsServices _templateQuestionServices;
    public TemplateQuestionsController(ITemplateQuestionsServices templateQuestionServices)
    {
        _templateQuestionServices = templateQuestionServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery] Dictionary<string, string> filters)
    {
        var (success, templateQuestions, message) = await _templateQuestionServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<TemplateQuestionResponseModel>());
        return Ok(templateQuestions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, templateQuestion, message) = await _templateQuestionServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(templateQuestion);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TemplateQuestionRequestModel model)
    {

        var (success, templateQuestion, message) = await _templateQuestionServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(templateQuestion);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] IDictionary<string, object> modelDict)
    {
        if (id <= 0)
            return BadRequest(new { message = "ID no proporcionado o inválido" });

        var currentModel = await _templateQuestionServices.GetById(id);
        if (!currentModel.success) return NotFound(new { message = currentModel.message });

        foreach (var kvp in modelDict)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            if (key == "id") continue;

            switch (key)
            {
                case "name":
                    currentModel.templateQuestion.Name = value?.ToString();
                    break;
                case "questionTypeId":
                    currentModel.templateQuestion.QuestionTypeId = value is JsonElement el1 ? el1.GetInt32() : Convert.ToInt32(value);
                    break;
                case "isRequired":
                    currentModel.templateQuestion.IsRequired = value is JsonElement el2 ? el2.GetBoolean() : Convert.ToBoolean(value);
                    break;
                case "parentId":
                    currentModel.templateQuestion.ParentId = value is JsonElement el3 ? el3.GetInt32() : Convert.ToInt32(value);
                    break;
                case "catalogNameId":
                    currentModel.templateQuestion.CatalogNameId = value is JsonElement el4 ? el4.GetInt32() : Convert.ToInt32(value);
                    break;
                case "active":
                    currentModel.templateQuestion.Active = value is JsonElement el5 ? el5.GetBoolean() : Convert.ToBoolean(value);
                    break;
            }


        }

        var (success, templateQuestion, message) = await _templateQuestionServices.Update(currentModel.templateQuestion);
        if (!success) return BadRequest(new { message });
        return Ok(templateQuestion);
    }
    // {
    //     if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

    //     var (success, templateQuestion, message) = await _templateQuestionServices.Update(model);
    //     if (!success) return BadRequest(new { message });
    //     return Ok(templateQuestion);
    // }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, templateQuestion, message) = await _templateQuestionServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(templateQuestion);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, templateQuestion, message) = await _templateQuestionServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(templateQuestion);
    }

    [HttpGet("{id}/remove")]
    public async Task<IActionResult> Remove(int id)
    {
        var (success, templateQuestion, message) = await _templateQuestionServices.Remove(id);
        if (!success) return BadRequest(new { message });
        return Ok(templateQuestion);
    }
}
