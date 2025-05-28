using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ITemplateQuestionRepository
{
    Task<IQueryable<TemplateQuestion>> GetAllAsync(Expression<Func<TemplateQuestion, bool>>? predicate = null);
    Task<TemplateQuestion?> GetAsync(Expression<Func<TemplateQuestion, bool>> predicate);
    Task AddAsync(TemplateQuestion templateQuestion);
    Task UpdateAsync(TemplateQuestion templateQuestion);
    Task RemoveAsync(TemplateQuestion templateQuestion);
    Task<int> SaveChangesAsync();
}

public class TemplateQuestionRepository : ITemplateQuestionRepository
{
    private readonly SabaContext _context;

    public TemplateQuestionRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<TemplateQuestion>> GetAllAsync(Expression<Func<TemplateQuestion, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.TemplateQuestions.AsQueryable()
            : _context.TemplateQuestions
            .Where(predicate).AsQueryable();
    }

    public Task<TemplateQuestion?> GetAsync(Expression<Func<TemplateQuestion, bool>> predicate)
    {
        var item = _context.TemplateQuestions.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(TemplateQuestion templateQuestion)
    {
        await _context.TemplateQuestions.AddAsync(templateQuestion);
    }

    public async Task UpdateAsync(TemplateQuestion templateQuestion)
    {
        _context.TemplateQuestions.Update(templateQuestion);
    }

    public async Task RemoveAsync(TemplateQuestion templateQuestion)
    {
        _context.TemplateQuestions.Remove(templateQuestion);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
