using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface IFilialsServices
{
    Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Add(FilialRequestModel m);

    Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Update(FilialRequestModel m);

    Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Disable(int id);

    Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Enable(int id);

    Task<(bool success, FilialRequestModel? filial, string? errorMsg)> GetByUserName(string userName);

    Task<(bool success, FilialRequestModel? filial, string? errorMsg)> GetById(int id);

    Task<(bool success, List<FilialRequestModel>? filials, string? errorMsg)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
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

    public async Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Add(FilialRequestModel m)
    {
        var existingFilial = await _filialRepository.GetAsync(x => x.Name == m.Name);
        if (existingFilial != null)        
            return (false, null, "Ya existe una filial con ese nombre.");

        var newFilial = new Filial
        {
            Name = m.Name,
            Address = m.Address,
            Lat = m.Lat,
            Lng = m.Lng,
            Active = m.Active,
        };

        await _filialRepository.AddAsync(newFilial);
        await _filialRepository.SaveChangesAsync();

        return (true, m, null);
    }

    public async Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Update(FilialRequestModel m)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == m.Id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        filial.Name = m.Name;
        filial.Address = m.Address;
        filial.Lat = m.Lat;
        filial.Lng = m.Lng;
        filial.Active = m.Active;

        await _filialRepository.UpdateAsync(filial);
        await _filialRepository.SaveChangesAsync();

        return (true, m, null);
    }

    public async Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Disable(int id)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        filial.Active = false;

        await _filialRepository.UpdateAsync(filial);
        await _filialRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool success, FilialRequestModel? filial, string? errorMsg)> Enable(int id)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        filial.Active = true;

        await _filialRepository.UpdateAsync(filial);
        await _filialRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool success, FilialRequestModel? filial, string? errorMsg)> GetByUserName(string userName)
    {
        var filial = await _filialRepository.GetAsync(x => x.Name == userName);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        var filialModel = new FilialRequestModel
        {
            Id = filial.Id,
            Name = filial.Name,
            Address = filial.Address,
            Lat = filial.Lat,
            Lng = filial.Lng,
            Active = filial.Active
        };

        return (true, filialModel, null);
    }

    public async Task<(bool success, List<FilialRequestModel>? filials, string? errorMsg)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
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

        if (page > 0 && pageSize > 0)
        {
            filials = filials.Skip(page).Take(pageSize);
        }

        var filialModels = filials.Select(x => new FilialRequestModel
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            Lat = x.Lat,
            Lng = x.Lng,
            Active = x.Active
        }).ToList();

        return (true, filialModels, null);
    }

    public async Task<(bool success, FilialRequestModel? filial, string? errorMsg)> GetById(int id)
    {
        var filial = await _filialRepository.GetAsync(x => x.Id == id);

        if (filial == null)
        {
            return (false, null, "Filial not found.");
        }

        var filialModel = new FilialRequestModel
        {
            Id = filial.Id,
            Name = filial.Name,
            Address = filial.Address,
            Lat = filial.Lat,
            Lng = filial.Lng,
            Active = filial.Active
        };

        return (true, filialModel, null);
    }
}