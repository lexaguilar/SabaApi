using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ITemplateRepository
{
    Task<IQueryable<Template>> GetAllAsync(Expression<Func<Template, bool>>? predicate = null);
    Task<Template?> GetAsync(Expression<Func<Template, bool>> predicate);
    Task AddAsync(Template template);
    Task UpdateAsync(Template template);
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
        return predicate == null
            ? _context.Templates.AsQueryable()
            : _context.Templates.Where(predicate).AsQueryable();
    }

    public Task<Template?> GetAsync(Expression<Func<Template, bool>> predicate)
    {
        var item = _context.Templates.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(Template template)
    {
        await _context.Templates.AddAsync(template);
    }

    public async Task UpdateAsync(Template template)
    {
        _context.Templates.Update(template);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
