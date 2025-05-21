using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

/// <summary>
/// Interface for user repository operations.
/// </summary>
namespace Saba.Repository;

public interface IUserRepository 
{
    Task<User> GetAsync(Expression<Func<User, bool>> predicate);
    Task<IQueryable<User>> GetAllAsync(Expression<Func<User, bool>>? predicate = null);
    Task AddAsync(User user);
    User Update(User user);
    Task<int> SaveChangesAsync();
}
public class UserRepository : IUserRepository
{
    private readonly SabaContext _context;

    public UserRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<IQueryable<User>> GetAllAsync(Expression<Func<User, bool>>? predicate = null)
    {
        if (predicate == null)
            return _context.Users.Include(x => x.Role).AsQueryable();

        return _context.Users.Include(x => x.Role).Where(predicate).AsQueryable();

    }

    public Task<User> GetAsync(Expression<Func<User, bool>> predicate)
    {
        var user = _context.Users.Include(x => x.Role).Where(predicate).FirstOrDefault();

        return Task.FromResult(user);
    }

    public User Update(User user)
    {
        _context.Users.Update(user);

        return user;

    }

    public async Task<int> SaveChangesAsync()
    {
       return await _context.SaveChangesAsync();
    }
}