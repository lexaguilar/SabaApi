using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ISurveysServices
{
    Task<(bool success, SurveyResponseModel? survey, string? message)> Add(SurveyRequestModel m);
    Task<(bool success, SurveyResponseModel? survey, string? message)> Update(SurveyRequestModel m);
    Task<(bool success, SurveyResponseModel? survey, string? message)> Disable(int id);
    Task<(bool success, SurveyResponseModel? survey, string? message)> Enable(int id);
    Task<(bool success, SurveyResponseModel? survey, string? message)> GetById(int id);
    Task<(bool success, SurveyPageResponseModel surveyResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class SurveysServices : ISurveysServices
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFilialUserRepository _filialUserRepository;
    private readonly AppSettings _appSettings;

    public SurveysServices(ISurveyRepository surveyRepository, IUserRepository userRepository, IFilialUserRepository filialUserRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _surveyRepository = surveyRepository;
        _userRepository = userRepository;
        _filialUserRepository = filialUserRepository;
    }

    private SurveyResponseModel MapToSurveyResponseModel(Survey survey)
    {
        return new SurveyResponseModel
        {
            Id = survey.Id,
            Name = survey.Name,
            TemplateId = survey.TemplateId,
            StartDate = survey.StartDate,
            EndDate = survey.EndDate,
            ApplyAllUser = survey.ApplyAllUser,
            MinGoal = survey.MinGoal,
            ExpectedGoal = survey.ExpectedGoal,
            CreatedAt = survey.CreatedAt,
            CreatedByUserId = survey.CreatedByUserId,
            EditedAt = survey.EditedAt,
            EditedByUserId = survey.EditedByUserId,
            Active = survey.Active,
        };
    }

    public async Task<(bool success, SurveyResponseModel? survey, string? message)> Add(SurveyRequestModel m)
    {
        var existing = await _surveyRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null) return (false, null, "Ya existe un survey con ese nombre.");

        var newSurvey = new Survey
        {
            Id = m.Id,
            Name = m.Name,
            TemplateId = m.TemplateId,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            ApplyAllUser = m.ApplyAllUser,
            MinGoal = m.MinGoal,
            ExpectedGoal = m.ExpectedGoal,
            Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId
        };

        if (newSurvey.ApplyAllUser)
        {
            
            var filialUsers = await _filialUserRepository.GetAllAsync();
            foreach (var filialUser in filialUsers)
            {
                newSurvey.SurveyUsers.Add(new SurveyUser
                {
                    UserId = filialUser.UserId,
                    SurveyId = newSurvey.Id,
                    FilialId = filialUser.FilialId,
                    SurveyUserStateId = 1, //Pending
                    Observation = "",
                    CreatedByUserId = m.UserId,
                    CreatedAt = DateTime.UtcNow,
                });
            }
        }

        await _surveyRepository.AddAsync(newSurvey);
        await _surveyRepository.SaveChangesAsync();

        return (true, MapToSurveyResponseModel(newSurvey), null);
    }

    public async Task<(bool success, SurveyResponseModel? survey, string? message)> Update(SurveyRequestModel m)
    {
        var item = await _surveyRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _surveyRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        if (existing != null) return (false, null, "Ya existe un survey con ese nombre.");

        item.Name = m.Name;
        item.TemplateId = m.TemplateId;
        item.StartDate = m.StartDate;
        item.EndDate = m.EndDate;
        item.ApplyAllUser = m.ApplyAllUser;
        item.MinGoal = m.MinGoal;
        item.ExpectedGoal = m.ExpectedGoal;
        item.Active = m.Active;
        item.EditedAt = DateTime.UtcNow;
        item.EditedByUserId = m.UserId;

        await _surveyRepository.UpdateAsync(item);
        await _surveyRepository.SaveChangesAsync();

        return (true, MapToSurveyResponseModel(item), null);
    }

    public async Task<(bool success, SurveyResponseModel? survey, string? message)> Disable(int id)
    {
        var item = await _surveyRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _surveyRepository.UpdateAsync(item);
        await _surveyRepository.SaveChangesAsync();
        return (true, MapToSurveyResponseModel(item), null);
    }

    public async Task<(bool success, SurveyResponseModel? survey, string? message)> Enable(int id)
    {
        var item = await _surveyRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _surveyRepository.UpdateAsync(item);
        await _surveyRepository.SaveChangesAsync();
        return (true, MapToSurveyResponseModel(item), null);
    }

    public async Task<(bool success, SurveyResponseModel? survey, string? message)> GetById(int id)
    {
        var item = await _surveyRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToSurveyResponseModel(item), null);
    }

    public async Task<(bool success, SurveyPageResponseModel surveyResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _surveyRepository.GetAllAsync();
        items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "id")
                    items = items.Where(x => x.Id == int.Parse(filter.Value));
                if (filter.Key == "name")
                    items = items.Where(x => x.Name.Contains(filter.Value));
                if (filter.Key == "templateId")
                    items = items.Where(x => x.TemplateId == int.Parse(filter.Value));
                if (filter.Key == "active" && bool.TryParse(filter.Value, out bool isActive))
                    items = items.Where(x => x.Active == isActive);
            }
        }

        var totalCount = items.Count();

        if (filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToSurveyResponseModel(x));

        return (true, new SurveyPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
