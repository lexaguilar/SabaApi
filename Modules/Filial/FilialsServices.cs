using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;
namespace Saba.Application.Services;

public interface IFilialsServices
{
    Task<(bool success, FilialResponseModel? filial, string? message)> Add(FilialRequestModel m);

    Task<(bool success, FilialResponseModel? filial, string? message)> Update(FilialRequestModel m);

    Task<(bool success, FilialResponseModel? filial, string? message)> Disable(int id);

    Task<(bool success, FilialResponseModel? filial, string? message)> Enable(int id);

    Task<(bool success, FilialResponseModel? filial, string? message)> GetByUserName(string userName);

    Task<(bool success, FilialResponseModel? filial, string? message)> GetById(int id);

    Task<(bool success, FilialPageResponseModel filials, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class FilialsServices : IFilialsServices
{
    private readonly IFilialRepository _filialRepository;
    private readonly AppSettings _appSettings;

    public FilialsServices(IFilialRepository filialRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _filialRepository = filialRepository;
    }

    private FilialResponseModel MapToFilialResponseModel(Filial filial)
    {
        return new FilialResponseModel
        {
            Id = filial.Id,
            InternalCode = filial.InternalCode,
            Name = filial.Name,
            Address = filial.Address,
            Lat = filial.Lat,
            Lng = filial.Lng,
            Active = filial.Active
        };
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Add(FilialRequestModel m)
    {
        var existingFilial = await _filialRepository.GetAsync(x => x.Name == m.Name);
        if (existingFilial != null)        
            return (false, null, "Ya existe una filial con ese nombre.");

        var existingInternalCode = await _filialRepository.GetAsync(x => x.InternalCode == m.InternalCode);
        if (existingInternalCode != null)        
            return (false, null, "Ya existe una filial con ese código interno.");

        var newFilial = new Filial
        {
            Name = m.Name,
            InternalCode = m.InternalCode,
            Address = m.Address,
            Lat = m.Lat,
            Lng = m.Lng,
            Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId,
        };

        await _filialRepository.AddAsync(newFilial);
        await _filialRepository.SaveChangesAsync();

        var filialModel = MapToFilialResponseModel(newFilial);

        return (true, filialModel, null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Update(FilialRequestModel m)
    {

        var filial = await _filialRepository.GetAsync(x => x.Id == m.Id);

        if (filial == null)
            return (false, null, "Filial not found.");

        var existingFilial = await _filialRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        // Check if another filial with the same name exists, excluding the current one
        if (existingFilial != null)        
            return (false, null, "Ya existe una filial con ese nombre.");

        var existingInternalCode = await _filialRepository.GetAsync(x => x.InternalCode == m.InternalCode && x.Id != m.Id);
        // Check if another filial with the same internal code exists, excluding the current one
        if (existingInternalCode != null)        
            return (false, null, "Ya existe una filial con ese código interno.");

        filial.Name = m.Name;
        filial.InternalCode = m.InternalCode;
        filial.Address = m.Address;
        filial.Lat = m.Lat;
        filial.Lng = m.Lng;
        filial.Active = m.Active;
        filial.EditedAt = DateTime.UtcNow;
        filial.EditedByUserId = m.UserId;

        await _filialRepository.UpdateAsync(filial);
        await _filialRepository.SaveChangesAsync();

        var filialModel = MapToFilialResponseModel(filial);

        return (true, filialModel, null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Disable(int id)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        filial.Active = false;

        await _filialRepository.UpdateAsync(filial);
        await _filialRepository.SaveChangesAsync();

        var filialModel = MapToFilialResponseModel(filial);

        return (true, filialModel, null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Enable(int id)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        filial.Active = true;

        await _filialRepository.UpdateAsync(filial);
        await _filialRepository.SaveChangesAsync();

        var filialModel = MapToFilialResponseModel(filial);

        return (true, filialModel, null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> GetByUserName(string userName)
    {
        var filial = await _filialRepository.GetAsync(x => x.Name == userName);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        var filialModel = MapToFilialResponseModel(filial);

        return (true, filialModel, null);
    }

    public async Task<(bool success, FilialPageResponseModel filials, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var filials = await _filialRepository.GetAllAsync();



        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {

                if (filter.Key == "Name")
                {
                    filials = filials.Where(x => x.Name.Contains(filter.Value));
                }
                else if (filter.Key == "Address")
                {
                    filials = filials.Where(x => x.Address.Contains(filter.Value));
                }
                else if (filter.Key == "active")
                {
                    if (bool.TryParse(filter.Value, out bool isActive))
                    {
                        filials = filials.Where(x => x.Active == isActive);
                    }
                }

            }
        }
        var totalCount = filials.Count();
        filials = filials.Skip(page).Take(pageSize);

        var filialModels = filials.ToArray().Select(x => MapToFilialResponseModel(x));

        var filialPageResponseModel = new FilialPageResponseModel
        {
            Items = filialModels,
            TotalCount = totalCount
        };

        return (true, filialPageResponseModel, null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> GetById(int id)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        var filialModel = MapToFilialResponseModel(filial);

        return (true, filialModel, null);
    }
}