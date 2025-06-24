using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface IReportsServices
{

    Task<ReportResponseModel> GetById(int id);
}

public class ReportsServices : IReportsServices
{
    private readonly IReportRepository _surveyRepository;
    private readonly AppSettings _appSettings;

    public ReportsServices(IReportRepository surveyRepository, IOptions<AppSettings> appSettings)
    {
        _surveyRepository = surveyRepository;
        _appSettings = appSettings.Value;
    }

    
    public async Task<ReportResponseModel> GetById(int id)
    {
        var surveyUserResponses = await _surveyRepository.GetAllAsync(id);

        return surveyUserResponses;
    }
}