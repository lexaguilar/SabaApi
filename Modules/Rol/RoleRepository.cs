using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IRoleRepository
{
    Task<IQueryable<Role>> GetAllAsync(Expression<Func<Role, bool>>? predicate = null);
    Task<Role?> GetAsync(Expression<Func<Role, bool>> predicate);
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task RemoveAsync(int roleId);
    Task UpdateResourcesAsync(int roleId, RoleResource[] roleResources);
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
        return Task.FromResult(_context.Roles
        .Include(x => x.RoleResources)
        .ThenInclude(x => x.ResourceKeyNavigation)
        .FirstOrDefault(predicate));
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

    public Task UpdateResourcesAsync(int roleId, RoleResource[] roleResources)
    {
       
        _context.RoleResources.AddRange(roleResources);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public Task RemoveAsync(int roleId)
    {
        _context.RoleResources.RemoveRange(_context.RoleResources.Where(rr => rr.RoleId == roleId));
        return Task.CompletedTask;
    }
}