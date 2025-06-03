using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ISurveyUserResponseRepository
{
    Task<IQueryable<SurveyUserResponse>> GetAllAsync(Expression<Func<SurveyUserResponse, bool>>? predicate = null);
    Task<SurveyUserResponse?> GetAsync(Expression<Func<SurveyUserResponse, bool>> predicate);
    Task AddAsync(SurveyUserResponse surveyUserResponse);
    Task UpdateAsync(SurveyUserResponse surveyUserResponse);
    Task<int> SaveChangesAsync();
}

public class SurveyUserResponseRepository : ISurveyUserResponseRepository
{
    private readonly SabaContext _context;

    public SurveyUserResponseRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<SurveyUserResponse>> GetAllAsync(Expression<Func<SurveyUserResponse, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.SurveyUserResponses.Include(x => x.Question)
            .ThenInclude(x => x.CatalogName)
            .AsQueryable()
            : _context.SurveyUserResponses
            .Where(predicate).Include(x => x.Question).ThenInclude(x => x.CatalogName).AsQueryable();
    }

    public Task<SurveyUserResponse?> GetAsync(Expression<Func<SurveyUserResponse, bool>> predicate)
    {
        var item = _context.SurveyUserResponses.Include(x => x.Question).ThenInclude(x => x.CatalogName).Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(SurveyUserResponse surveyUserResponse)
    {
        await _context.SurveyUserResponses.AddAsync(surveyUserResponse);
    }

    public async Task UpdateAsync(SurveyUserResponse surveyUserResponse)
    {
        _context.SurveyUserResponses.Update(surveyUserResponse);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
