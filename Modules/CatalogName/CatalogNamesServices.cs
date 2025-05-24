using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ICatalogNamesServices
{
    Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Add(CatalogNameRequestModel m);
    Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Update(CatalogNameRequestModel m);
    Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Disable(int id);
    Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Enable(int id);
    Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> GetByUserName(string userName);
    Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> GetById(int id);
    Task<(bool success, CatalogNamePageResponseModel catalogNameResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class CatalogNamesServices : ICatalogNamesServices
{
    private readonly ICatalogNameRepository _catalogNameRepository;
    private readonly AppSettings _appSettings;

    public CatalogNamesServices(ICatalogNameRepository catalogNameRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _catalogNameRepository = catalogNameRepository;
    }

    private CatalogNameResponseModel MapToCatalogNameResponseModel(CatalogName catalogName)
    {
        return new CatalogNameResponseModel
        {
            Id = catalogName.Id,
            Name = catalogName.Name,
            CreatedAt = catalogName.CreatedAt,
            CreatedByUserId = catalogName.CreatedByUserId,
            EditedAt = catalogName.EditedAt,
            EditedByUserId = catalogName.EditedByUserId,
            Active = catalogName.Active,
        };
    }

    public async Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Add(CatalogNameRequestModel m)
    {
        var existing = await _catalogNameRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null) return (false, null, "Ya existe un catalogname con ese nombre.");

        var newCatalogName = new CatalogName
        {
           Id = m.Id,
           Name = m.Name,
           Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId
        };

        await _catalogNameRepository.AddAsync(newCatalogName);
        await _catalogNameRepository.SaveChangesAsync();

        return (true, MapToCatalogNameResponseModel(newCatalogName), null);
    }

    public async Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Update(CatalogNameRequestModel m)
    {
        var item = await _catalogNameRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _catalogNameRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        if (existing != null) return (false, null, "Ya existe un catalogname con ese nombre.");

        item.Name = m.Name;
        item.Active = m.Active;
        item.EditedAt = DateTime.UtcNow;
        item.EditedByUserId = m.UserId;

        await _catalogNameRepository.UpdateAsync(item);
        await _catalogNameRepository.SaveChangesAsync();

        return (true, MapToCatalogNameResponseModel(item), null);
    }

    public async Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Disable(int id)
    {
        var item = await _catalogNameRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _catalogNameRepository.UpdateAsync(item);
        await _catalogNameRepository.SaveChangesAsync();
        return (true, MapToCatalogNameResponseModel(item), null);
    }

    public async Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> Enable(int id)
    {
        var item = await _catalogNameRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _catalogNameRepository.UpdateAsync(item);
        await _catalogNameRepository.SaveChangesAsync();
        return (true, MapToCatalogNameResponseModel(item), null);
    }

    public async Task<(bool success, CatalogNameResponseModel? catalogName, string? message)> GetByUserName(string userName)
    {
        var item = await _catalogNameRepository.GetAsync(x => x.Name == userName);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToCatalogNameResponseModel (item), null);
    }

    public async Task<(bool success, CatalogNameResponseModel ? catalogName, string? message)> GetById(int id)
    {
        var item = await _catalogNameRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToCatalogNameResponseModel (item), null);
    }

    public async Task<(bool success, CatalogNamePageResponseModel catalogNameResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _catalogNameRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
            }
        }

        var totalCount = items.Count();
        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToCatalogNameResponseModel(x));

        return (true, new CatalogNamePageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
