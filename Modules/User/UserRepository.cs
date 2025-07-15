using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IUserRepository
{
    Task<IQueryable<User>> GetAllAsync(Expression<Func<User, bool>>? predicate = null);
    Task<User?> GetAsync(Expression<Func<User, bool>> predicate);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<int> MarkFilialsAsDeleted(User user);
    Task<int> SaveChangesAsync();
}

public class UserRepository : IUserRepository
{
    private readonly SabaContext _context;

    public UserRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<User>> GetAllAsync(Expression<Func<User, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.Users.Include(x => x.Country).Include(x => x.Role).Include(x => x.FilialUsers).AsQueryable()
            : _context.Users.Include(x => x.Country).Include(x => x.Role).Include(x => x.FilialUsers).Where(predicate).AsQueryable();
    }

    public Task<User?> GetAsync(Expression<Func<User, bool>> predicate)
    {
        var item = _context.Users
        .Include(x => x.FilialUsers)
        .Include(x => x.Country)
        .Include(x => x.Role)
        .ThenInclude(x => x.RoleResources)
        .Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task<int> MarkFilialsAsDeleted(User user)
    {
        var oldRelations = user.FilialUsers.ToList();
        foreach (var rel in oldRelations)
        {
            _context.Entry(rel).State = EntityState.Deleted;
        }
        _context.Users.Update(user);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
