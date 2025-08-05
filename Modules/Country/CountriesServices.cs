using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;
using Saba.Application.Extensions;

namespace Saba.Application.Services;

public interface ICountriesServices
{
    Task<(bool success, CountryResponseModel? country, string? message)> Add(CountryRequestModel m);
    Task<(bool success, CountryResponseModel? country, string? message)> Update(CountryRequestModel m);
    Task<(bool success, CountryResponseModel? country, string? message)> Disable(int id);
    Task<(bool success, CountryResponseModel? country, string? message)> Enable(int id);
    Task<(bool success, CountryResponseModel? country, string? message)> GetById(int id);
    Task<(bool success, CountryPageResponseModel countryResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class CountriesServices : ICountriesServices
{
    private readonly ICountryRepository _countryRepository;
    private readonly AppSettings _appSettings;

    public CountriesServices(ICountryRepository countryRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _countryRepository = countryRepository;
    }

    private CountryResponseModel MapToCountryResponseModel(Country country)
    {
        return new CountryResponseModel
        {
            Id = country.Id,
            Name = country.Name,
            Active = country.Active,
            CreatedAt = country.CreatedAt,
            CreatedByUserId = country.CreatedByUserId,
        };
    }

    public async Task<(bool success, CountryResponseModel? country, string? message)> Add(CountryRequestModel m)
    {
        var existing = await _countryRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null) return (false, null, "Ya existe un country con ese nombre.");

        var newCountry = new Country
        {
           Id = m.Id,
           Name = m.Name,
           Active = m.Active,
           CreatedAt = DatetimeHelper.getDateTimeZoneInfo(),
           CreatedByUserId = m.UserId
        };

        await _countryRepository.AddAsync(newCountry);
        await _countryRepository.SaveChangesAsync();

        return (true, MapToCountryResponseModel(newCountry), null);
    }

    public async Task<(bool success, CountryResponseModel? country, string? message)> Update(CountryRequestModel m)
    {
        var item = await _countryRepository.GetAsync(x => x.Id == m.Id);
        if (item == null) return (false, null, "No encontrado.");

        var existing = await _countryRepository.GetAsync(x => x.Name == m.Name && x.Id != m.Id);
        if (existing != null) return (false, null, "Ya existe un country con ese nombre.");

        item.Name = m.Name;
        item.Active = m.Active;
        item.EditedAt = DatetimeHelper.getDateTimeZoneInfo();
        item.EditedByUserId = m.UserId;

        await _countryRepository.UpdateAsync(item);
        await _countryRepository.SaveChangesAsync();

        return (true, MapToCountryResponseModel(item), null);
    }

    public async Task<(bool success, CountryResponseModel? country, string? message)> Disable(int id)
    {
        var item = await _countryRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = false;
        await _countryRepository.UpdateAsync(item);
        await _countryRepository.SaveChangesAsync();
        return (true, MapToCountryResponseModel(item), null);
    }

    public async Task<(bool success, CountryResponseModel? country, string? message)> Enable(int id)
    {
        var item = await _countryRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        item.Active = true;
        await _countryRepository.UpdateAsync(item);
        await _countryRepository.SaveChangesAsync();
        return (true, MapToCountryResponseModel(item), null);
    }

    public async Task<(bool success, CountryResponseModel ? country, string? message)> GetById(int id)
    {
        var item = await _countryRepository.GetAsync(x => x.Id == id);
        if (item == null) return (false, null, "No encontrado.");
        return (true, MapToCountryResponseModel (item), null);
    }

    public async Task<(bool success, CountryPageResponseModel countryResult, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var items = await _countryRepository.GetAllAsync();
        items = items.OrderByDescending(x => x.Id);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "name")
                    items = items.Where(x => x.Name.Contains(filter.Value));
            }
        }

        var totalCount = items.Count();

        if(filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        items = items.Skip(page).Take(pageSize);
        var list = items.ToList().Select(x => MapToCountryResponseModel(x));

        return (true, new CountryPageResponseModel { Items = list, TotalCount = totalCount }, null);
    }
}
