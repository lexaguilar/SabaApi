using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ICatalogGenericRepository
{
    Task<IQueryable<GenericCatalog>> GetAllAsync(Expression<Func<GenericCatalog, bool>>? predicate = null);

    Task<GenericCatalog?> GetAsync(Expression<Func<GenericCatalog, bool>> predicate);

    Task AddAsync(GenericCatalog genericCatalog);

    Task UpdateAsync(GenericCatalog genericCatalog);

    Task<int> SaveChangesAsync();
}

public class CatalogGenericRepository : ICatalogGenericRepository
{
    private readonly SabaContext _context;

    public CatalogGenericRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<GenericCatalog>> GetAllAsync(Expression<Func<GenericCatalog, bool>>? predicate = null)
    {
        if (predicate == null)
            return _context.GenericCatalogs.AsQueryable();

        return _context.GenericCatalogs.Where(predicate).AsQueryable();

    }

    public Task<GenericCatalog> GetAsync(Expression<Func<GenericCatalog, bool>> predicate)
    {
        var genericCatalog = _context.GenericCatalogs.Where(predicate).FirstOrDefault();

        return Task.FromResult(genericCatalog);
    }

    public async Task AddAsync(GenericCatalog genericCatalog)
    {
        await _context.GenericCatalogs.AddAsync(genericCatalog);
    }

    public async Task UpdateAsync(GenericCatalog genericCatalog)
    {
        _context.GenericCatalogs.Update(genericCatalog);
    }

    public async Task<int> SaveChangesAsync()
    {
       return await _context.SaveChangesAsync();
    }
}