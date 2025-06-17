using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;
using System.Data;

namespace Saba.Application.Services;

public interface ISurveyUserResponsesServices
{
    Task<(bool success, SurveyUserResponseResponseModel? surveyUserResponse, string? message)> Add(SurveyUserResponseRequestModel m);
    Task<(bool success, SurveyUserResponseResponseModel? surveyUserResponse, string? message)> Update(SurveyUserResponseRequestModel m);
    Task<(bool success, SurveyUserResponseResponseModel? surveyUserResponse, string? message)> GetById(int id);
    Task<(bool success, SurveyUserResponsePageResponseModel surveyUserResponseResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
    Task<(bool success, DataTable surveyUserResponseResult, string? message)> GetPivotAll(int surveyId);
}

public class SurveyUserResponsesServices : ISurveyUserResponsesServices
{
    private readonly ISurveyUserResponseRepository _surveyUserResponseRepository;
    private readonly ISurveyUsersServices _surveyUsersServices;
    private readonly AppSettings _appSettings;

    public SurveyUserResponsesServices(ISurveyUserResponseRepository surveyUserResponseRepository, ISurveyUsersServices surveyUsersServices, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _surveyUserResponseRepository = surveyUserResponseRepository;
        _surveyUsersServices = surveyUsersServices;
    }

    private SurveyUserResponseResponseModel MapToSurveyUserResponseResponseModel(SurveyUserResponse surveyUserResponse)
    {
        return new SurveyUserResponseResponseModel
        {
            Id = surveyUserResponse.Id,
            SurveyUserId = surveyUserResponse.SurveyUserId,
            QuestionId = surveyUserResponse.QuestionId,
            Response = surveyUserResponse.Response,
            CompletedAt = surveyUserResponse.CompletedAt,
            Question = new TemplateQuestionResponseModel
            {
                Id = surveyUserResponse.Question.Id,
                Name = surveyUserResponse.Question.Name,
                QuestionTypeId = surveyUserResponse.Question.QuestionTypeId,
                CatalogNameId = surveyUserResponse.Question.CatalogNameId,
                ParentId = surveyUserResponse.Question.ParentId,
                IsRequired = surveyUserResponse.Question.IsRequired,
                Files = surveyUserResponse.SurveyUserResponseFiles.Count(),

            },
            CatalogName = surveyUserResponse.Question.CatalogName?.Name,

        };
    }

    public async Task<(bool success, SurveyUserResponseResponseModel? surveyUserResponse, string? message)> Add(SurveyUserResponseRequestModel m)
    {

        var newSurveyUserResponse = new SurveyUserResponse
        {
            Id = m.Id,
            SurveyUserId = m.SurveyUserId,
            QuestionId = m.QuestionId,
            Response = m.Response,
            CompletedAt = DateTime.UtcNow,
        };

        await _surveyUserResponseRepository.AddAsync(newSurveyUserResponse);
        await _surveyUserResponseRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseResponseModel(newSurveyUserResponse), null);
    }

    public async Task<(bool success, SurveyUserResponseResponseModel? surveyUserResponse, string? message)> Update(SurveyUserResponseRequestModel m)
    {
        var item = await _surveyUserResponseRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        //item.SurveyUserId = m.SurveyUserId;
        //item.QuestionId = m.QuestionId;
        item.Response = m.Response;
        item.CompletedAt = DateTime.UtcNow;

        await _surveyUserResponseRepository.UpdateAsync(item);
        await _surveyUserResponseRepository.SaveChangesAsync();

        await _surveyUsersServices.UpdateSurveyUserState(item.SurveyUserId, SurveyStates.EnProgreso);

        return (true, MapToSurveyUserResponseResponseModel(item), null);
    }

    public async Task<(bool success, SurveyUserResponseResponseModel? surveyUserResponse, string? message)> GetById(int id)
    {
        var item = await _surveyUserResponseRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToSurveyUserResponseResponseModel(item), null);
    }

    public async Task<(bool success, SurveyUserResponsePageResponseModel surveyUserResponseResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _surveyUserResponseRepository.GetAllAsync();
        items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "surveyUserId")
                    items = items.Where(x => x.SurveyUserId == int.Parse(filter.Value));
            }
        }

        var totalCount = items.Count();

        if (filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToSurveyUserResponseResponseModel(x));

        return (true, new SurveyUserResponsePageResponseModel { Items = list, TotalCount = totalCount }, null);
    }

    public async Task<(bool success, DataTable surveyUserResponseResult, string? message)> GetPivotAll(int surveyId)
    {
        var items = await _surveyUserResponseRepository.GetAllPivotAsync(surveyId);

        return (true, items, null);
    }
}
