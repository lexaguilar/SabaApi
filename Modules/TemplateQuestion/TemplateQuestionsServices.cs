using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ITemplateQuestionsServices
{
    Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Add(TemplateQuestionRequestModel m);
    Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Update(TemplateQuestionRequestModel m);
    Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Disable(int id);
    Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Enable(int id);
    Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> GetById(int id);
    Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Remove(int id);
    Task<(bool success, TemplateQuestionPageResponseModel templateQuestionResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class TemplateQuestionsServices : ITemplateQuestionsServices
{
    private readonly ITemplateQuestionRepository _templateQuestionRepository;
    private readonly AppSettings _appSettings;

    public TemplateQuestionsServices(ITemplateQuestionRepository templateQuestionRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _templateQuestionRepository = templateQuestionRepository;
    }

    private TemplateQuestionResponseModel MapToTemplateQuestionResponseModel(TemplateQuestion templateQuestion)
    {
        return new TemplateQuestionResponseModel
        {
            Id = templateQuestion.Id,
            TemplateId = templateQuestion.TemplateId,
            QuestionTypeId = templateQuestion.QuestionTypeId,
            Name = templateQuestion.Name,
            IsRequired = templateQuestion.IsRequired,
            ParentId = templateQuestion.ParentId,
            CatalogNameId = templateQuestion.CatalogNameId,
            Active = templateQuestion.Active,
        };
    }

    public async Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Add(TemplateQuestionRequestModel m)
    {
        var existing = await _templateQuestionRepository.GetAsync(x => x.Name == m.Name && x.TemplateId == m.TemplateId && x.QuestionTypeId == m.QuestionTypeId && x.ParentId == m.ParentId);
        if (existing != null) return (false, null, "Ya existe un templatequestion con ese nombre.");

        var newTemplateQuestion = new TemplateQuestion
        {
            Id = m.Id,
            TemplateId = m.TemplateId,
            QuestionTypeId = m.QuestionTypeId,
            Name = m.Name,
            IsRequired = m.IsRequired,
            ParentId = m.ParentId,
            CatalogNameId = m.CatalogNameId,
            Active = m.Active,
           
        };

        await _templateQuestionRepository.AddAsync(newTemplateQuestion);
        await _templateQuestionRepository.SaveChangesAsync();

        return (true, MapToTemplateQuestionResponseModel(newTemplateQuestion), null);
    }

    public async Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Update(TemplateQuestionRequestModel m)
    {
        var item = await _templateQuestionRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _templateQuestionRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id && x.TemplateId == m.TemplateId && x.QuestionTypeId == m.QuestionTypeId && x.ParentId == m.ParentId);
        if (existing != null) return (false, null, "Ya existe un templatequestion con ese nombre.");

        item.TemplateId = m.TemplateId;
        item.QuestionTypeId = m.QuestionTypeId;
        item.Name = m.Name;
        item.IsRequired = m.IsRequired;
        item.ParentId = m.ParentId;
        item.CatalogNameId = m.QuestionTypeId == 2 ? m.CatalogNameId : null;
        item.Active = m.Active;

        await _templateQuestionRepository.UpdateAsync(item);
        await _templateQuestionRepository.SaveChangesAsync();

        return (true, MapToTemplateQuestionResponseModel(item), null);
    }

    public async Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Disable(int id)
    {
        var item = await _templateQuestionRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _templateQuestionRepository.UpdateAsync(item);
        await _templateQuestionRepository.SaveChangesAsync();
        return (true, MapToTemplateQuestionResponseModel(item), null);
    }

    public async Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Enable(int id)
    {
        var item = await _templateQuestionRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _templateQuestionRepository.UpdateAsync(item);
        await _templateQuestionRepository.SaveChangesAsync();
        return (true, MapToTemplateQuestionResponseModel(item), null);
    }

    public async Task<(bool success, TemplateQuestionResponseModel ? templateQuestion, string? message)> GetById(int id)
    {
        var item = await _templateQuestionRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToTemplateQuestionResponseModel (item), null);
    }

    public async Task<(bool success, TemplateQuestionResponseModel? templateQuestion, string? message)> Remove(int id)
    {
        var item = await _templateQuestionRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        await _templateQuestionRepository.RemoveAsync(item);
        await _templateQuestionRepository.SaveChangesAsync();
        return (true, MapToTemplateQuestionResponseModel(item), null);
    }

    public async Task<(bool success, TemplateQuestionPageResponseModel templateQuestionResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _templateQuestionRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "templateId")
                {
                    if (int.TryParse(filter.Value, out var templateId))
                    {
                        items = items.Where(x => x.TemplateId == templateId);
                    }
                }
            }
        }

        var totalCount = items.Count();
       // items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToTemplateQuestionResponseModel(x));

        return (true, new TemplateQuestionPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
