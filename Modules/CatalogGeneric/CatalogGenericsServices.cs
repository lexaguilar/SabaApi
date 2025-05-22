using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface ICatalogGenericsServices
{
    Task<(bool success, GenericCatalogRequestModel? catalogRequestModel, string? message)> Add(GenericCatalogRequestModel m);

    Task<(bool success, GenericCatalogRequestModel? catalogRequestModel, string? message)> Update(GenericCatalogRequestModel m);

    Task<(bool success, GenericCatalogRequestModel? catalogRequestModel, string? message)> GetById(int id);

    Task<(bool success, GenericCatalogPageResponseModel catalogPageResponseModels, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class CatalogGenericsServices : ICatalogGenericsServices
{
    private readonly ICatalogGenericRepository _catalogGenericRepository;
    private readonly AppSettings _appSettings;

    public CatalogGenericsServices(ICatalogGenericRepository catalogGenericRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _catalogGenericRepository = catalogGenericRepository;
    }

    private GenericCatalogRequestModel MapToCatalogPageResponseModel(GenericCatalog catalogGeneric)
    {
        return new GenericCatalogRequestModel
        {
            Id = catalogGeneric.Id,
            CatalogName = catalogGeneric.CatalogName,
            CatalogValue = catalogGeneric.CatalogValue
        };
    }

    public async Task<(bool success, GenericCatalogRequestModel? catalogRequestModel, string? message)> Add(GenericCatalogRequestModel m)
    {
        var existing = await _catalogGenericRepository.GetAsync(x => x.CatalogName == m.CatalogName && x.CatalogValue == m.CatalogValue);
        if (existing != null)        
            return (false, null, "Ya existe un catálogo con ese nombre y valor.");

        var newCatalog = new GenericCatalog
        {
            CatalogName = m.CatalogName,
            CatalogValue = m.CatalogValue
        };

        await _catalogGenericRepository.AddAsync(newCatalog);
        await _catalogGenericRepository.SaveChangesAsync();

        return (true, m, null);
    }

    public async Task<(bool success, GenericCatalogRequestModel? catalogRequestModel, string? message)> Update(GenericCatalogRequestModel m)
    {
        var catalog = await _catalogGenericRepository.GetAsync(x => x.Id == m.Id);

        if (catalog == null)
            return (false, null, "Catalog not found.");

        var existing = await _catalogGenericRepository.GetAsync(x => x.CatalogName == m.CatalogName && x.CatalogValue == m.CatalogValue);
        if (existing != null)        
            return (false, null, "Ya existe un catálogo con ese nombre y valor.");

        catalog.CatalogName = m.CatalogName;
        catalog.CatalogValue = m.CatalogValue;        

        await _catalogGenericRepository.UpdateAsync(catalog);
        await _catalogGenericRepository.SaveChangesAsync();

        return (true, m, null);
    }

    public async Task<(bool success, GenericCatalogPageResponseModel catalogPageResponseModels, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var catalogs = await _catalogGenericRepository.GetAllAsync();



        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {

                if (filter.Key == "CatalogName")
                {
                    catalogs = catalogs.Where(x => x.CatalogName.Contains(filter.Value));
                }
                else if (filter.Key == "CatalogValue")
                {
                    catalogs = catalogs.Where(x => x.CatalogValue.Contains(filter.Value));
                }

            }
        }
        var totalCount = catalogs.Count();
        catalogs = catalogs.Skip(page).Take(pageSize);

        var catalogsModels = catalogs.ToArray().Select(x => MapToCatalogPageResponseModel(x));

        var catalogPageResponseModel = new GenericCatalogPageResponseModel
        {
            Items = catalogsModels,
            TotalCount = totalCount
        };

        return (true, catalogPageResponseModel, null);
    }

    public async Task<(bool success, GenericCatalogRequestModel? catalogRequestModel, string? message)> GetById(int id)
    {
        var catalog = await _catalogGenericRepository.GetAsync(x => x.Id == id);

        if (catalog == null)
            return (false, null, "Catalog not found.");

        var catalogModel = MapToCatalogPageResponseModel(catalog);

        return (true, catalogModel, null);
    }
}