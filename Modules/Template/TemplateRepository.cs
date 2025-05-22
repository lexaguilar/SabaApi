using System.Linq.Expressions;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ITemplateRepository
{
    Task<IQueryable<Template>> GetAllAsync(Expression<Func<Template, bool>>? predicate = null);

    Task<Template?> GetAsync(Expression<Func<Template, bool>> predicate);
    Task AddAsync(Template Template);

    Task UpdateAsync(Template Template);

    Task<int> SaveChangesAsync();
}

public class TemplateRepository : ITemplateRepository
{
    private readonly SabaContext _context;

    public TemplateRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Template>> GetAllAsync(Expression<Func<Template, bool>>? predicate = null)
    {
        if (predicate == null)
            return _context.Templates.AsQueryable();

        return _context.Templates.Where(predicate).AsQueryable();

    }


    public Task<Template> GetAsync(Expression<Func<Template, bool>> predicate)
    {
        var Template = _context.Templates.Where(predicate).FirstOrDefault();

        return Task.FromResult(Template);
    }

    public async Task AddAsync(Template Template)
    {
        await _context.Templates.AddAsync(Template);
    }

    public async Task UpdateAsync(Template Template)
    {
        _context.Templates.Update(Template);
    }

    public async Task<int> SaveChangesAsync()
    {
       return await _context.SaveChangesAsync();
    }
}