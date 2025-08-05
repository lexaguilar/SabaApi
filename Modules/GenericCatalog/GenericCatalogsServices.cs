using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface IGenericCatalogsServices
{
    Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Add(GenericCatalogRequestModel m);
    Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Update(GenericCatalogRequestModel m);
    Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Disable(int id);
    Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Enable(int id);
    Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> GetById(int id);
    Task<(bool success, GenericCatalogPageResponseModel genericCatalogResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class GenericCatalogsServices : IGenericCatalogsServices
{
    private readonly IGenericCatalogRepository _genericCatalogRepository;
    private readonly AppSettings _appSettings;

    public GenericCatalogsServices(IGenericCatalogRepository genericCatalogRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _genericCatalogRepository = genericCatalogRepository;
    }

    private GenericCatalogResponseModel MapToGenericCatalogResponseModel(GenericCatalog genericCatalog)
    {
        return new GenericCatalogResponseModel
        {
            Id = genericCatalog.Id,
            CatalogNameId = genericCatalog.CatalogNameId,
            CatalogValue = genericCatalog.CatalogValue,
            CreatedAt = genericCatalog.CreatedAt,
            CreatedByUserId = genericCatalog.CreatedByUserId,
            EditedAt = genericCatalog.EditedAt,
            EditedByUserId = genericCatalog.EditedByUserId,
            Active = genericCatalog.Active,
        };
    }

    public async Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Add(GenericCatalogRequestModel m)
    {
        var existing = await _genericCatalogRepository.GetAsync(x => x.CatalogValue == m.CatalogValue);
        if (existing != null) return (false, null, "Ya existe un genericcatalog con ese valor.");

        var newGenericCatalog = new GenericCatalog
        {
            Id = m.Id,
            CatalogNameId = m.CatalogNameId,
            CatalogValue = m.CatalogValue,
            Active = m.Active,
            CreatedAt = DatetimeHelper.getDateTimeZoneInfo(),
            CreatedByUserId = m.UserId
        };

        await _genericCatalogRepository.AddAsync(newGenericCatalog);
        await _genericCatalogRepository.SaveChangesAsync();

        return (true, MapToGenericCatalogResponseModel(newGenericCatalog), null);
    }

    public async Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Update(GenericCatalogRequestModel m)
    {
        var item = await _genericCatalogRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _genericCatalogRepository.GetAsync(x => x.CatalogValue == m.CatalogValue);
        if (existing != null) return (false, null, "Ya existe un genericcatalog con ese nombre.");

        item.CatalogNameId = m.CatalogNameId;        
        item.CatalogValue = m.CatalogValue;
        item.Active = m.Active;
        item.EditedAt = DatetimeHelper.getDateTimeZoneInfo();
        item.EditedByUserId = m.UserId;

        await _genericCatalogRepository.UpdateAsync(item);
        await _genericCatalogRepository.SaveChangesAsync();

        return (true, MapToGenericCatalogResponseModel(item), null);
    }

    public async Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Disable(int id)
    {
        var item = await _genericCatalogRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _genericCatalogRepository.UpdateAsync(item);
        await _genericCatalogRepository.SaveChangesAsync();
        return (true, MapToGenericCatalogResponseModel(item), null);
    }

    public async Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> Enable(int id)
    {
        var item = await _genericCatalogRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _genericCatalogRepository.UpdateAsync(item);
        await _genericCatalogRepository.SaveChangesAsync();
        return (true, MapToGenericCatalogResponseModel(item), null);
    }

    public async Task<(bool success, GenericCatalogResponseModel? genericCatalog, string? message)> GetById(int id)
    {
        var item = await _genericCatalogRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToGenericCatalogResponseModel(item), null);
    }

    public async Task<(bool success, GenericCatalogPageResponseModel genericCatalogResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _genericCatalogRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "catalogNameId")
                    items = items.Where(x => x.CatalogNameId == int.Parse(filter.Value));
                if (filter.Key == "catalogValue")
                    items = items.Where(x => x.CatalogValue.Contains(filter.Value));             
            }
        }

        var totalCount = items.Count();

        if (filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToGenericCatalogResponseModel(x));

        return (true, new GenericCatalogPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
