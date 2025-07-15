using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ICountryRepository
{
    Task<IQueryable<Country>> GetAllAsync(Expression<Func<Country, bool>>? predicate = null);
    Task<Country?> GetAsync(Expression<Func<Country, bool>> predicate);
    Task AddAsync(Country country);
    Task UpdateAsync(Country country);
    Task<int> SaveChangesAsync();
}

public class CountryRepository : ICountryRepository
{
    private readonly SabaContext _context;

    public CountryRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Country>> GetAllAsync(Expression<Func<Country, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.Countries.AsQueryable()
            : _context.Countries
            .Where(predicate).AsQueryable();
    }

    public Task<Country?> GetAsync(Expression<Func<Country, bool>> predicate)
    {
        var item = _context.Countries.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(Country country)
    {
        await _context.Countries.AddAsync(country);
    }

    public async Task UpdateAsync(Country country)
    {
        _context.Countries.Update(country);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
