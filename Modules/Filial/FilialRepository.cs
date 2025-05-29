using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IFilialRepository
{
    Task<IQueryable<Filial>> GetAllAsync(Expression<Func<Filial, bool>>? predicate = null);

    Task<Filial?> GetAsync(Expression<Func<Filial, bool>> predicate);
    Task<IQueryable<Filial>> GetFilialByUserIdAsync(int userId);

    Task AddAsync(Filial filial);

    Task UpdateAsync(Filial filial);

    Task<int> SaveChangesAsync();
}

public class FilialRepository : IFilialRepository
{
    private readonly SabaContext _context;

    public FilialRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Filial>> GetAllAsync(Expression<Func<Filial, bool>>? predicate = null)
    {
        if (predicate == null)
            return _context.Filials.AsQueryable();

        return _context.Filials.Where(predicate).AsQueryable();

    }

    public Task<IQueryable<Filial>> GetFilialByUserIdAsync(int userId)
    {
        IQueryable<Filial> filial = _context.Users.Include(x => x.FilialUsers)
            .Where(x => x.Id == userId)
            .SelectMany(x => x.FilialUsers.Select(fu => fu.Filial));

        return Task.FromResult(filial);
    }

    public Task<Filial> GetAsync(Expression<Func<Filial, bool>> predicate)
    {
        var filial = _context.Filials.Where(predicate).FirstOrDefault();

        return Task.FromResult(filial);
    }

    public async Task AddAsync(Filial filial)
    {
        await _context.Filials.AddAsync(filial);
    }

    public async Task UpdateAsync(Filial filial)
    {
        _context.Filials.Update(filial);
    }

    public async Task<int> SaveChangesAsync()
    {
       return await _context.SaveChangesAsync();
    }
}