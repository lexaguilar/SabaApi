using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ISurveyUsersServices
{
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Add(SurveyUserRequestModel m);
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Update(SurveyUserRequestModel m);
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> UpdateSurveyUserState(int id, SurveyStates state);
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> GetById(int id);
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Remove(int id);
    Task<(bool success, SurveyUserPageResponseModel surveyUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class SurveyUsersServices : ISurveyUsersServices
{
    private readonly ISurveyUserRepository _surveyUserRepository;
    private readonly AppSettings _appSettings;

    public SurveyUsersServices(ISurveyUserRepository surveyUserRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _surveyUserRepository = surveyUserRepository;
    }

    private SurveyUserResponseModel MapToSurveyUserResponseModel(SurveyUser surveyUser)
    {
        return new SurveyUserResponseModel
        {
            Id = surveyUser.Id,
            SurveyId = surveyUser.SurveyId,
            UserId = surveyUser.UserId,
            FilialId = surveyUser.FilialId,
            SurveyUserStateId = surveyUser.SurveyUserStateId,
            Observation = surveyUser.Observation,
            CreatedAt = surveyUser.CreatedAt,
            CreatedByUserId = surveyUser.CreatedByUserId,
            EditedAt = surveyUser.EditedAt,
            EditedByUserId = surveyUser.EditedByUserId,
            SurveyName = surveyUser.Survey?.Name,
            FilialName = surveyUser.Filial?.Name,
            UserName = surveyUser.User?.UserName,
            TotalQuestions = surveyUser.SurveyUserResponses?.Where(x => x.Question.QuestionTypeId != 4).Count() ?? 0,
            TotalResponses = surveyUser.SurveyUserResponses?.Where(x => x.Question.QuestionTypeId != 4).Sum(x => x.CompletedAt != null ? 1 : 0) ?? 0
        };
    }

    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Add(SurveyUserRequestModel m)
    {

        var newSurveyUser = new SurveyUser
        {
            Id = m.Id,
            SurveyId = m.SurveyId,
            UserId = m.UserId,
            FilialId = m.FilialId,
            SurveyUserStateId = m.SurveyUserStateId,
            Observation = m.Observation,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserEditId
        };

        await _surveyUserRepository.AddAsync(newSurveyUser);
        await _surveyUserRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseModel(newSurveyUser), null);
    }

    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Update(SurveyUserRequestModel m)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        item.SurveyId = m.SurveyId;
        item.UserId = m.UserId;
        item.FilialId = m.FilialId;
        item.SurveyUserStateId = m.SurveyUserStateId;
        item.Observation = m.Observation;
        item.EditedAt = DateTime.UtcNow;
        item.EditedByUserId = m.UserEditId;

        await _surveyUserRepository.UpdateAsync(item);
        await _surveyUserRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseModel(item), null);
    }


    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> GetById(int id)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToSurveyUserResponseModel(item), null);
    }

    public async Task<(bool success, SurveyUserPageResponseModel surveyUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _surveyUserRepository.GetAllAsync();
        items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "userId")
                    items = items.Where(x => x.UserId == int.Parse(filter.Value));

                if (filter.Key == "surveyId")
                    items = items.Where(x => x.SurveyId == int.Parse(filter.Value));
            }
        }

        var totalCount = items.Count();

        if (filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToSurveyUserResponseModel(x));

        return (true, new SurveyUserPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }

    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Remove(int id)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        await _surveyUserRepository.RemoveAsync(item);
        await _surveyUserRepository.SaveChangesAsync();
        return (true, MapToSurveyUserResponseModel(item), null);
    }

    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> UpdateSurveyUserState(int id, SurveyStates state)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");

        item.SurveyUserStateId = (int)state;


        await _surveyUserRepository.UpdateAsync(item);
        await _surveyUserRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseModel(item), null);
    }
}
