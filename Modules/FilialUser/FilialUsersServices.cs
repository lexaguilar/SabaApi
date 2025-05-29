using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface IFilialUsersServices
{
    Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Add(FilialUserRequestModel m);
    Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Update(FilialUserRequestModel m);
    Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Disable(int id);
    Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Enable(int id);
    Task<(bool success, FilialUserResponseModel? filialUser, string? message)> GetById(int id);
    Task<(bool success, FilialUserPageResponseModel filialUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
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

    public async Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Add(FilialUserRequestModel m)
    {
        var existing = await _filialUserRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null) return (false, null, "Ya existe un filialuser con ese nombre.");

        var newFilialUser = new FilialUser
        {
           UserId = m.UserId,
           FilialId = m.FilialId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId
        };

        await _filialUserRepository.AddAsync(newFilialUser);
        await _filialUserRepository.SaveChangesAsync();

        return (true, MapToFilialUserResponseModel(newFilialUser), null);
    }

    public async Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Update(FilialUserRequestModel m)
    {
        var item = await _filialUserRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _filialUserRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        if (existing != null) return (false, null, "Ya existe un filialuser con ese nombre.");

        item.UserId = m.UserId;
        item.FilialId = m.FilialId;
        item.EditedAt = DateTime.UtcNow;
        item.EditedByUserId = m.UserId;

        await _filialUserRepository.UpdateAsync(item);
        await _filialUserRepository.SaveChangesAsync();

        return (true, MapToFilialUserResponseModel(item), null);
    }

    public async Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Disable(int id)
    {
        var item = await _filialUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _filialUserRepository.UpdateAsync(item);
        await _filialUserRepository.SaveChangesAsync();
        return (true, MapToFilialUserResponseModel(item), null);
    }

    public async Task<(bool success, FilialUserResponseModel? filialUser, string? message)> Enable(int id)
    {
        var item = await _filialUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _filialUserRepository.UpdateAsync(item);
        await _filialUserRepository.SaveChangesAsync();
        return (true, MapToFilialUserResponseModel(item), null);
    }

    public async Task<(bool success, FilialUserResponseModel ? filialUser, string? message)> GetById(int id)
    {
        var item = await _filialUserRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToFilialUserResponseModel (item), null);
    }

    public async Task<(bool success, FilialUserPageResponseModel filialUserResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _filialUserRepository.GetAllAsync();
        items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
            }
        }

        var totalCount = items.Count();

        if(filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToFilialUserResponseModel(x));

        return (true, new FilialUserPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
