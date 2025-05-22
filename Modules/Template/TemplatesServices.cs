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

    Task<(bool success, TemplatePageResponseModel templates, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
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
            Active = template.Active
        };
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Add(TemplateRequestModel m)
    {
        var existingTemplate = await _templateRepository.GetAsync(x => x.Name == m.Name);
        if (existingTemplate != null)        
            return (false, null, "Ya existe una plantilla con ese nombre.");

        var newTemplate = new Template
        {
            Name = m.Name,
            TemplateCode = m.TemplateCode,
            Description = m.Description,            
            Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId,
        };

        await _templateRepository.AddAsync(newTemplate);
        await _templateRepository.SaveChangesAsync();

        var templateModel = MapToTemplateResponseModel(newTemplate);

        return (true, templateModel, null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Update(TemplateRequestModel m)
    {

        var template = await _templateRepository.GetAsync(x => x.Id == m.Id);

        if (template == null)
            return (false, null, "Template not found.");

        var existingTemplate = await _templateRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        // Check if another template with the same name exists, excluding the current one
        if (existingTemplate != null)        
            return (false, null, "Ya existe una template con ese nombre.");

        template.Name = m.Name;
        template.TemplateCode = m.TemplateCode;
        template.Description = m.Description;
        template.Active = m.Active;
        template.EditedAt = DateTime.UtcNow;
        template.EditedByUserId = m.UserId;

        await _templateRepository.UpdateAsync(template);
        await _templateRepository.SaveChangesAsync();

        var templateModel = MapToTemplateResponseModel(template);

        return (true, templateModel, null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Disable(int id)
    {
        var template = await _templateRepository.GetAsync(x => x.Id == id);

        if (template == null)
        {
            return (false, null, "Template not found.");
        }

        template.Active = false;

        await _templateRepository.UpdateAsync(template);
        await _templateRepository.SaveChangesAsync();

        var templateModel = MapToTemplateResponseModel(template);

        return (true, templateModel, null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> Enable(int id)
    {
        var template = await _templateRepository.GetAsync(x => x.Id == id);

        if (template == null)
        {
            return (false, null, "Template not found.");
        }

        template.Active = true;

        await _templateRepository.UpdateAsync(template);
        await _templateRepository.SaveChangesAsync();

        var templateModel = MapToTemplateResponseModel(template);

        return (true, templateModel, null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> GetByUserName(string userName)
    {
        var template = await _templateRepository.GetAsync(x => x.Name == userName);

        if (template == null)
        {
            return (false, null, "Template not found.");
        }

        var templateModel = MapToTemplateResponseModel(template);

        return (true, templateModel, null);
    }

    public async Task<(bool success, TemplatePageResponseModel templates, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {

        var templates = await _templateRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {

                if (filter.Key == "name")
                {
                    templates = templates.Where(x => x.Name.Contains(filter.Value));
                }
                else if (filter.Key == "templateCode")
                {
                    templates = templates.Where(x => x.TemplateCode.Contains(filter.Value));
                }
                else if (filter.Key == "active")
                {
                    if (bool.TryParse(filter.Value, out bool isActive))
                    {
                        templates = templates.Where(x => x.Active == isActive);
                    }
                }

            }
        }
        var totalCount = templates.Count();
        templates = templates.Skip(page).Take(pageSize);

        var templateModels = templates.ToArray().Select(x => MapToTemplateResponseModel(x));

        var templatePageResponseModel = new TemplatePageResponseModel
        {
            Items = templateModels,
            TotalCount = totalCount
        };

        return (true, templatePageResponseModel, null);
    }

    public async Task<(bool success, TemplateResponseModel? template, string? message)> GetById(int id)
    {
        var template = await _templateRepository.GetAsync(x => x.Id == id);

        if (template == null)
        {
            return (false, null, "Template not found.");
        }

        var templateModel = MapToTemplateResponseModel(template);

        return (true, templateModel, null);
    }
}