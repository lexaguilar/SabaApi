using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ISurveyRepository
{
    Task<IQueryable<Survey>> GetAllAsync(Expression<Func<Survey, bool>>? predicate = null);
    Task<Survey?> GetAsync(Expression<Func<Survey, bool>> predicate);
    Task AddAsync(Survey survey);
    Task UpdateAsync(Survey survey);
    Task<int> SaveChangesAsync();
}

public class SurveyRepository : ISurveyRepository
{
    private readonly SabaContext _context;

    public SurveyRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Survey>> GetAllAsync(Expression<Func<Survey, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.Surveys.AsQueryable()
            : _context.Surveys
                .Include(x => x.Template)
            .Where(predicate).AsQueryable();
    }

    public Task<Survey?> GetAsync(Expression<Func<Survey, bool>> predicate)
    {
        var item = _context.Surveys
        .Include(x => x.SurveyUsers)
        .Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(Survey survey)
    {
        await _context.Surveys.AddAsync(survey);
    }

    public async Task UpdateAsync(Survey survey)
    {
        _context.Surveys.Update(survey);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
