using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IResourceRepository
{
    Task<IQueryable<Resource>> GetAllAsync(Expression<Func<Resource, bool>>? predicate = null);
    Task<Resource?> GetAsync(Expression<Func<Resource, bool>> predicate);
    Task AddAsync(Resource resource);
    Task UpdateAsync(Resource resource);
    Task<int> SaveChangesAsync();
}

public class ResourceRepository : IResourceRepository
{
    private readonly SabaContext _context;

    public ResourceRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Resource>> GetAllAsync(Expression<Func<Resource, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.Resources.AsQueryable()
            : _context.Resources
            .Where(predicate).AsQueryable();
    }

    public Task<Resource?> GetAsync(Expression<Func<Resource, bool>> predicate)
    {
        var item = _context.Resources.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(Resource resource)
    {
        await _context.Resources.AddAsync(resource);
    }

    public async Task UpdateAsync(Resource resource)
    {
        _context.Resources.Update(resource);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
