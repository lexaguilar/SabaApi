using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface IFilialUsersServices
{
    Task<(bool success, FilialUserPageResponseModel filialUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
    Task<(bool success, FilialUserPageResponseModel filialUserResult, string? message)> GetByUserAll(int userId);
}

public class FilialUsersServices : IFilialUsersServices
{
    private readonly IFilialUserRepository _filialUserRepository;
    private readonly AppSettings _appSettings;

    public FilialUsersServices(IFilialUserRepository filialUserRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _filialUserRepository = filialUserRepository;
    }

    private FilialUserResponseModel MapToFilialUserResponseModel(FilialUser filialUser)
    {
        return new FilialUserResponseModel
        {
            UserId = filialUser.UserId,
            FilialId = filialUser.FilialId,
        };
    }


    public async Task<(bool success, FilialUserPageResponseModel filialUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _filialUserRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
            }
        }

        var totalCount = items.Count();

        if (filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToFilialUserResponseModel(x));

        return (true, new FilialUserPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }

    public async Task<(bool success, FilialUserPageResponseModel filialUserResult, string? message)> GetByUserAll(int userId)
    {
        var items = await _filialUserRepository.GetAllAsync();
        items = items.Where(x => x.UserId == userId);

        var totalCount = items.Count();

        items = items.Skip(0).Take(totalCount);
        var list = items.ToList().Select(x => MapToFilialUserResponseModel(x));

        return (true, new FilialUserPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
