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
    Task<(bool success, FilialPageResponseModel filialResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
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
            Active = filial.Active,
            CreatedAt = filial.CreatedAt,
            CreatedByUserId = filial.CreatedByUserId,
            EditedAt = filial.EditedAt,
            EditedByUserId = filial.EditedByUserId,
        };
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Add(FilialRequestModel m)
    {
        var existing = await _filialRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null) return (false, null, "Ya existe un filial con ese nombre.");

        var newFilial = new Filial
        {
            Id = m.Id,
            InternalCode = m.InternalCode,
            Name = m.Name,
            Address = m.Address,
            Lat = m.Lat,
            Lng = m.Lng,
            Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId
        };

        await _filialRepository.AddAsync(newFilial);
        await _filialRepository.SaveChangesAsync();

        return (true, MapToFilialResponseModel(newFilial), null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Update(FilialRequestModel m)
    {
        var item = await _filialRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _filialRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        if (existing != null) return (false, null, "Ya existe un filial con ese nombre.");

        item.InternalCode = m.InternalCode;
        item.Name = m.Name;
        item.Address = m.Address;
        item.Lat = m.Lat;
        item.Lng = m.Lng;
        item.Active = m.Active;
        item.EditedAt = DateTime.UtcNow;
        item.EditedByUserId = m.UserId;

        await _filialRepository.UpdateAsync(item);
        await _filialRepository.SaveChangesAsync();

        return (true, MapToFilialResponseModel(item), null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Disable(int id)
    {
        var item = await _filialRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _filialRepository.UpdateAsync(item);
        await _filialRepository.SaveChangesAsync();
        return (true, MapToFilialResponseModel(item), null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> Enable(int id)
    {
        var item = await _filialRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _filialRepository.UpdateAsync(item);
        await _filialRepository.SaveChangesAsync();
        return (true, MapToFilialResponseModel(item), null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> GetByUserName(string userName)
    {
        var item = await _filialRepository.GetAsync(x => x.Name == userName);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToFilialResponseModel(item), null);
    }

    public async Task<(bool success, FilialResponseModel? filial, string? message)> GetById(int id)
    {
        var item = await _filialRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToFilialResponseModel(item), null);
    }

    public async Task<(bool success, FilialPageResponseModel filialResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _filialRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "internalCode")
                    items = items.Where(x => x.InternalCode.Contains(filter.Value));
                if (filter.Key == "name")
                    items = items.Where(x => x.Name.Contains(filter.Value));
                if (filter.Key == "active" && bool.TryParse(filter.Value, out bool isActive))
                    items = items.Where(x => x.Active == isActive);
            }
        }

        var totalCount = items.Count();
        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToFilialResponseModel(x));

        return (true, new FilialPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
