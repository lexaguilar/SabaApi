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
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> UpdateSurveyUserState(int id, SurveyStates state, decimal? Latitude = null, decimal? Longitude = null);
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> GetById(int id);
    Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Remove(int id);
    Task<(bool success, SurveyUserResponseModel? survey, string? message)> FinishSurvey(FinishSurveyUserRequestModel model, int userId);
    Task<(bool success, SurveyUserResponseModel? survey, string? message)> ResumeSurvey(FinishSurveyUserRequestModel model, int userId);
    Task<(bool success, SurveyUserPageResponseModel surveyUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
    Task<(bool success, int total, string? message)> GetTotalCountByState(int? userId, int stateId);
    Task<SurveyUserIssuesResponseModel[]> GetIssuesFoundAsync(int countryId, DateTime date);
    Task<SurveyUserResponseIssuesResponseModel[]> GetIssuesFoundDetailsAsync(int countryId, int filialId, DateTime date);
}

public class SurveyUsersServices : ISurveyUsersServices
{
    private readonly ISurveyUserRepository _surveyUserRepository;
    private readonly IFilialsServices _filialsServices;
    private readonly ITemplateQuestionsServices _templateQuestionsServices;
    private readonly ISurveysServices _surveysServices;
    private readonly AppSettings _appSettings;

    public SurveyUsersServices(ISurveyUserRepository surveyUserRepository, ITemplateQuestionsServices templateQuestionsServices, ISurveysServices surveysServices, IOptions<AppSettings> appSettings, IFilialsServices filialsServices)
    {
        _appSettings = appSettings.Value;
        _surveyUserRepository = surveyUserRepository;
        _templateQuestionsServices = templateQuestionsServices;
        _surveysServices = surveysServices;
        _filialsServices = filialsServices;
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
            FilialLatitude = surveyUser.Filial?.Lat,
            FilialLongitude = surveyUser.Filial?.Lng,
            UserName = surveyUser.User?.UserName,
            TotalQuestions = surveyUser.SurveyUserResponses?.Where(x => x.Question.QuestionTypeId != 4).Count() ?? 0,
            TotalResponses = surveyUser.SurveyUserResponses?.Where(x => x.Question.QuestionTypeId != 4).Sum(x => x.CompletedAt != null ? 1 : 0) ?? 0,
            Latitude = surveyUser.Latitude,
            Longitude = surveyUser.Longitude,
            Evaluation = surveyUser.SurveyUserResponses?.Where(x => x.Question.QuestionTypeId != 4).Count(x => x.Response == "Si") ?? 0,
            Distance = surveyUser.Distance ?? null,
            AdministratorNameFilial = surveyUser.AdministratorNameFilial,
            OwnerFilial = surveyUser.OwnerFilial,
            StartDate = surveyUser.StartDate,
            EndDate = surveyUser.EndDate
        };
    }

    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> Add(SurveyUserRequestModel m)
    {

        if (m.SurveyUserStateId == 0)
            m.SurveyUserStateId = (int)SurveyStates.Pendiente;

        var surveyResult = await _surveysServices.GetById(m.SurveyId);

        if (!surveyResult.success || surveyResult.survey == null)
            return (false, null, surveyResult.message);

        var survey = surveyResult.survey;

        if (survey.TemplateId == 0)
            return (false, null, "La encuesta no tiene un template asociado.");

        if (survey.SurveyStateId != (int)SurveyStates.EnProgreso)
            return (false, null, "La encuesta no está en progreso.");

        var questions = await _templateQuestionsServices.GetAll(0, 0, new Dictionary<string, string> { { "active", "true" }, { "templateId", survey.TemplateId.ToString() } });

        var surveyUserResponses = questions.templateQuestionResult.Items.Select(q => new SurveyUserResponse
        {
            QuestionId = q.Id,
            Response = "",
            CompletedAt = null
        }).ToList();

        var newSurveyUser = new SurveyUser
        {
            Id = m.Id,
            SurveyId = m.SurveyId,
            UserId = m.UserId,
            FilialId = m.FilialId,
            SurveyUserStateId = m.SurveyUserStateId,
            Observation = m.Observation,
            CreatedAt = DatetimeHelper.getDateTimeZoneInfo(),
            CreatedByUserId = m.UserEditId,
            SurveyUserResponses = surveyUserResponses
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
        item.EditedAt = DatetimeHelper.getDateTimeZoneInfo();
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

                if (filter.Key == "filialId")
                    items = items.Where(x => x.FilialId == int.Parse(filter.Value));

                if (filter.Key == "surveyUserStateId")
                    items = items.Where(x => x.SurveyUserStateId == int.Parse(filter.Value));

                if (filter.Key == "countryId")
                    items = items.Where(x => x.Survey.CountryId == int.Parse(filter.Value));

                if (filter.Key == "reportDate" && DateTime.TryParse(filter.Value, out DateTime reportDate))
                {
                    var year = reportDate.Year;
                    var month = reportDate.Month;

                    items = items.Where(x => x.CreatedAt.Year == year && x.CreatedAt.Month == month);
                }
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

    public async Task<(bool success, SurveyUserResponseModel? surveyUser, string? message)> UpdateSurveyUserState(int id, SurveyStates state, decimal? Latitude = null, decimal? Longitude = null)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");

        item.SurveyUserStateId = (int)state;

        if (Latitude.HasValue)
            item.Latitude = Latitude.Value;
        if (Longitude.HasValue)
            item.Longitude = Longitude.Value;

        var startDate = item.StartDate;
        if (!startDate.HasValue)
            item.StartDate = DatetimeHelper.getDateTimeZoneInfo();

        if (Latitude.HasValue && Longitude.HasValue)
        {
            var filialResult = await _filialsServices.GetById(item.FilialId);
            if (filialResult.success)
            {
                var filial = filialResult.filial;
                if (filial.Lat.HasValue && filial.Lng.HasValue)
                {
                    item.Distance = Convert.ToDecimal(CalcularDistancia(Latitude.Value, Longitude.Value, filial.Lat.Value, filial.Lng.Value));
                }
                else
                {
                    item.Distance = null; // Si la filial no tiene coordenadas, se establece la distancia como nula.
                }
            }
        }

        await _surveyUserRepository.UpdateAsync(item);
        await _surveyUserRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseModel(item), null);
    }

    private double CalcularDistancia(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double R = 6371000; // Radio de la Tierra en metros
        double ToRadians(double grados) => grados * Math.PI / 180;

        double φ1 = ToRadians((double)lat1);
        double φ2 = ToRadians((double)lat2);
        double Δφ = ToRadians((double)(lat2 - lat1));
        double Δλ = ToRadians((double)(lon2 - lon1));

        double a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                   Math.Cos(φ1) * Math.Cos(φ2) *
                   Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distancia = R * c;
        return distancia; // en metros
    }

    public async Task<(bool success, SurveyUserResponseModel? survey, string? message)> FinishSurvey(FinishSurveyUserRequestModel model, int userId)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == model.Id);
        if (item == null) return (false, null, "No encontrado.");

        if (item.SurveyUserStateId != (int)SurveyStates.EnProgreso)
            return (false, null, "La encuesta no está en estado en progreso.");

        item.SurveyUserStateId = (int)SurveyStates.Finalizado;
        item.AdministratorNameFilial = model.AdministratorNameFilial;
        item.OwnerFilial = model.OwnerFilial;
        item.EditedAt = DatetimeHelper.getDateTimeZoneInfo();
        item.EditedByUserId = userId;
        item.EndDate = DatetimeHelper.getDateTimeZoneInfo();

        await _surveyUserRepository.UpdateAsync(item);
        await _surveyUserRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseModel(item), null);
    }
    
    public async Task<(bool success, SurveyUserResponseModel? survey, string? message)> ResumeSurvey(FinishSurveyUserRequestModel model, int userId)
    {
        var item = await _surveyUserRepository.GetAsync(x => x.Id == model.Id);
        if (item == null) return (false, null, "No encontrado.");

        if (item.SurveyUserStateId != (int)SurveyStates.Finalizado)
            return (false, null, "La encuesta no está en estado finalizado.");

        item.SurveyUserStateId = (int)SurveyStates.EnProgreso;
        item.EditedAt = DatetimeHelper.getDateTimeZoneInfo();
        item.EditedByUserId = userId;

        await _surveyUserRepository.UpdateAsync(item);
        await _surveyUserRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseModel(item), null);
    }

    public async Task<(bool success, int total, string? message)> GetTotalCountByState(int? userId, int stateId)
    {
        
        var totalCount = await _surveyUserRepository.GetAllAsync(x => x.SurveyUserStateId == stateId);

        var total = 0;
        if (userId.HasValue)        
            total = totalCount.Count(x => x.UserId == userId.Value);        
        else        
            total = totalCount.Count();
        
        return (true, total, null);
    }

    public Task<SurveyUserIssuesResponseModel[]> GetIssuesFoundAsync(int countryId, DateTime date)
    {
        return _surveyUserRepository.GetIssuesFoundAsync(countryId, date);
    }
    public Task<SurveyUserResponseIssuesResponseModel[]> GetIssuesFoundDetailsAsync(int countryId, int filialId, DateTime date)
    {
        return _surveyUserRepository.GetIssuesFoundDetailsAsync(countryId, filialId, date);
    }
}
