using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IFilialUserRepository
{
    Task<IQueryable<FilialUser>> GetAllAsync(Expression<Func<FilialUser, bool>>? predicate = null);
    Task<FilialUser?> GetAsync(Expression<Func<FilialUser, bool>> predicate);
}

public class FilialUserRepository : IFilialUserRepository
{
    private readonly SabaContext _context;

    public FilialUserRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<FilialUser>> GetAllAsync(Expression<Func<FilialUser, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.FilialUsers.AsQueryable()
            : _context.FilialUsers
            .Where(predicate).AsQueryable();
    }

    public Task<FilialUser?> GetAsync(Expression<Func<FilialUser, bool>> predicate)
    {
        var item = _context.FilialUsers.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

}
