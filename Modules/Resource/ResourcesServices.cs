using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface IResourcesServices
{

    Task<(bool success, ResourceResponseModel? resource, string? message)> GetById(string resourceKey);
    Task<(bool success, ResourcePageResponseModel resourceResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class ResourcesServices : IResourcesServices
{
    private readonly IResourceRepository _resourceRepository;
    private readonly AppSettings _appSettings;

    public ResourcesServices(IResourceRepository resourceRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _resourceRepository = resourceRepository;
    }

    private ResourceResponseModel MapToResourceResponseModel(Resource resource)
    {
        return new ResourceResponseModel
        {
            ResourceKey = resource.ResourceKey,
            Name = resource.Name,
            Description = resource.Description,
            CreatedAt = resource.CreatedAt,
            ParentResourceKey = resource.ParentResourceKey,
        };
    }

    public async Task<(bool success, ResourceResponseModel ? resource, string? message)> GetById(string resourceKey)
    {
        var item = await _resourceRepository.GetAsync(x => x.ResourceKey == resourceKey);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToResourceResponseModel (item), null);
    }

    public async Task<(bool success, ResourcePageResponseModel resourceResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _resourceRepository.GetAllAsync();
        //items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "resourceKey")
                    items = items.Where(x => x.ResourceKey.Contains(filter.Value));
                if (filter.Key == "name")
                    items = items.Where(x => x.Name.Contains(filter.Value));
            }
        }

        var totalCount = items.Count();

        page = 0;
        pageSize = totalCount;

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToResourceResponseModel(x));

        return (true, new ResourcePageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
