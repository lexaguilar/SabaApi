using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ITemplatesServices
{
    Task<(bool success, TemplateResponseModel? template, string? message)> Add(TemplateRequestModel m);
    Task<(bool success, TemplateResponseModel? template, string? message)> Update(TemplateRequestModel m);
    Task<(bool success, TemplateResponseModel? template, string? message)> Disable(int id);
    Task<(bool success, TemplateResponseModel? template, string? message)> Enable(int id);
    Task<(bool success, TemplateResponseModel? template, string? message)> GetByUserName(string userName);
    Task<(bool success, TemplateResponseModel? template, string? message)> GetById(int id);
    Task<(bool success, TemplatePageResponseModel templateResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class TemplatesServices : ITemplatesServices
{
    private readonly ITemplateRepository _templateRepository;
    private readonly AppSettings _appSettings;

    public TemplatesServices(ITemplateRepository templateRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _templateRepository = templateRepository;
    }

    private TemplateResponseModel MapToTemplateResponseModel(Template template)
    {
        return new TemplateResponseModel
        {
            Id = template.Id,
            TemplateCode = template.TemplateCode,
            Name = template.Name,
            Description = template.Description,
            CreatedAt = template.CreatedAt,
            CreatedByUserId = template.CreatedByUserId,
            EditedAt = template.EditedAt,
            EditedByUserId = template.EditedByUserId,
            Active = template.Active,
        };
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Add(TemplateRequestModel m)
    {
        var existing = await _templateRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null) return (false, null, "Ya existe un template con ese nombre.");

        var newTemplate = new Template
        {
           Id = m.Id,
           TemplateCode = m.TemplateCode,
           Name = m.Name,
           Description = m.Description,
           Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId
        };

        await _templateRepository.AddAsync(newTemplate);
        await _templateRepository.SaveChangesAsync();

        return (true, MapToTemplateResponseModel(newTemplate), null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Update(TemplateRequestModel m)
    {
        var item = await _templateRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _templateRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        if (existing != null) return (false, null, "Ya existe un template con ese nombre.");

        item.TemplateCode = m.TemplateCode;
        item.Name = m.Name;
        item.Description = m.Description;
        item.Active = m.Active;
        item.EditedAt = DateTime.UtcNow;
        item.EditedByUserId = m.UserId;

        await _templateRepository.UpdateAsync(item);
        await _templateRepository.SaveChangesAsync();

        return (true, MapToTemplateResponseModel(item), null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Disable(int id)
    {
        var item = await _templateRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _templateRepository.UpdateAsync(item);
        await _templateRepository.SaveChangesAsync();
        return (true, MapToTemplateResponseModel(item), null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Enable(int id)
    {
        var item = await _templateRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _templateRepository.UpdateAsync(item);
        await _templateRepository.SaveChangesAsync();
        return (true, MapToTemplateResponseModel(item), null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> GetByUserName(string userName)
    {
        var item = await _templateRepository.GetAsync(x => x.Name == userName);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToTemplateResponseModel (item), null);
    }

    public async Task<(bool success, TemplateResponseModel ? template, string? message)> GetById(int id)
    {
        var item = await _templateRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToTemplateResponseModel (item), null);
    }

    public async Task<(bool success, TemplatePageResponseModel templateResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _templateRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "templateCode")
                    items = items.Where(x => x.TemplateCode != null && x.TemplateCode.Contains(filter.Value));
                if (filter.Key == "name")
                    items = items.Where(x => x.Name.Contains(filter.Value));
            }
        }

        var totalCount = items.Count();
        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToTemplateResponseModel(x));

        return (true, new TemplatePageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
