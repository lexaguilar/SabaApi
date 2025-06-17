using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ISurveyUserResponseFilesServices
{
    Task<(bool success, SurveyUserResponseFileResponseModel? surveyUserResponseFile, string? message)> Add(SurveyUserResponseFileRequestModel m);
    Task<(bool success, SurveyUserResponseFileResponseModel? surveyUserResponseFile, string? message)> Remove(int id);
    Task<(bool success, SurveyUserResponseFileResponseModel? surveyUserResponseFile, string? message)> GetById(int id);
    Task<(bool success, SurveyUserResponseFilePageResponseModel surveyUserResponseFileResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class SurveyUserResponseFilesServices : ISurveyUserResponseFilesServices
{
    private readonly ISurveyUserResponseFileRepository _surveyUserResponseFileRepository;
    private readonly AppSettings _appSettings;

    public SurveyUserResponseFilesServices(ISurveyUserResponseFileRepository surveyUserResponseFileRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _surveyUserResponseFileRepository = surveyUserResponseFileRepository;
    }

    private SurveyUserResponseFileResponseModel MapToSurveyUserResponseFileResponseModel(SurveyUserResponseFile surveyUserResponseFile)
    {
        return new SurveyUserResponseFileResponseModel
        {
            Id = surveyUserResponseFile.Id,
            SurveyUserResponseId = surveyUserResponseFile.SurveyUserResponseId,
            FileNameUploaded = surveyUserResponseFile.FileNameUploaded,
        };
    }

    public async Task<(bool success, SurveyUserResponseFileResponseModel? surveyUserResponseFile, string? message)> Add(SurveyUserResponseFileRequestModel m)
    {
        var existing = await _surveyUserResponseFileRepository.GetAsync(x => x.SurveyUserResponseId == m.SurveyUserResponseId && x.FileNameUploaded == m.FileNameUploaded);
        if (existing != null) return (false, null, "Ya existe un archivo con ese nombre.");

        var newSurveyUserResponseFile = new SurveyUserResponseFile
        {
            Id = m.Id,
            SurveyUserResponseId = m.SurveyUserResponseId,
            FileNameUploaded = m.FileNameUploaded,
            ContentType = m.ContentType,
        };

        await _surveyUserResponseFileRepository.AddAsync(newSurveyUserResponseFile);
        await _surveyUserResponseFileRepository.SaveChangesAsync();

        return (true, MapToSurveyUserResponseFileResponseModel(newSurveyUserResponseFile), null);
    }

    public async Task<(bool success, SurveyUserResponseFileResponseModel? surveyUserResponseFile, string? message)> Remove(int id)
    {
        var item = await _surveyUserResponseFileRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        await _surveyUserResponseFileRepository.RemoveAsync(item);
        await _surveyUserResponseFileRepository.SaveChangesAsync();
        return (true, MapToSurveyUserResponseFileResponseModel(item), null);
    }
    public async Task<(bool success, SurveyUserResponseFileResponseModel? surveyUserResponseFile, string? message)> GetById(int id)
    {
        var item = await _surveyUserResponseFileRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToSurveyUserResponseFileResponseModel (item), null);
    }

    public async Task<(bool success, SurveyUserResponseFilePageResponseModel surveyUserResponseFileResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _surveyUserResponseFileRepository.GetAllAsync();
        items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "surveyUserResponseId")
                {
                    if (int.TryParse(filter.Value, out int surveyUserResponseId))
                    {
                        items = items.Where(x => x.SurveyUserResponseId == surveyUserResponseId);
                    }                  
                }
            }
        }

        var totalCount = items.Count();
      
        page = 0;
        pageSize = totalCount;        

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToSurveyUserResponseFileResponseModel(x));

        return (true, new SurveyUserResponseFilePageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
