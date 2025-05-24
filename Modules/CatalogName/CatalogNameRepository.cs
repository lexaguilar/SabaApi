using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ICatalogNameRepository
{
    Task<IQueryable<CatalogName>> GetAllAsync(Expression<Func<CatalogName, bool>>? predicate = null);
    Task<CatalogName?> GetAsync(Expression<Func<CatalogName, bool>> predicate);
    Task AddAsync(CatalogName catalogName);
    Task UpdateAsync(CatalogName catalogName);
    Task<int> SaveChangesAsync();
}

public class CatalogNameRepository : ICatalogNameRepository
{
    private readonly SabaContext _context;

    public CatalogNameRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<CatalogName>> GetAllAsync(Expression<Func<CatalogName, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.CatalogNames.AsQueryable()
            : _context.CatalogNames
            .Where(predicate).AsQueryable();
    }

    public Task<CatalogName?> GetAsync(Expression<Func<CatalogName, bool>> predicate)
    {
        var item = _context.CatalogNames.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(CatalogName catalogName)
    {
        await _context.CatalogNames.AddAsync(catalogName);
    }

    public async Task UpdateAsync(CatalogName catalogName)
    {
        _context.CatalogNames.Update(catalogName);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
