using System.Linq.Expressions;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IRoleRepository
{
    Task<IQueryable<Role>> GetAllAsync(Expression<Func<Role, bool>>? predicate = null);
    Task<Role?> GetAsync(Expression<Func<Role, bool>> predicate);
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task<int> SaveChangesAsync();
}

public class RoleRepository : IRoleRepository
{
    private readonly SabaContext _context;

    public RoleRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Role>> GetAllAsync(Expression<Func<Role, bool>>? expression)
    {
        return expression == null ? _context.Roles.AsQueryable() : _context.Roles.Where(expression).AsQueryable();
    }

    public Task<Role?> GetAsync(Expression<Func<Role, bool>> predicate)
    {
        return Task.FromResult(_context.Roles.FirstOrDefault(predicate));
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}