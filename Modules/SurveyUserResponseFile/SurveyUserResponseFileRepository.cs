using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ISurveyUserResponseFileRepository
{
    Task<IQueryable<SurveyUserResponseFile>> GetAllAsync(Expression<Func<SurveyUserResponseFile, bool>>? predicate = null);
    Task<SurveyUserResponseFile?> GetAsync(Expression<Func<SurveyUserResponseFile, bool>> predicate);
    Task AddAsync(SurveyUserResponseFile surveyUserResponseFile);
    Task RemoveAsync(SurveyUserResponseFile surveyUserResponseFile);
    Task<int> SaveChangesAsync();
}

public class SurveyUserResponseFileRepository : ISurveyUserResponseFileRepository
{
    private readonly SabaContext _context;

    public SurveyUserResponseFileRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<SurveyUserResponseFile>> GetAllAsync(Expression<Func<SurveyUserResponseFile, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.SurveyUserResponseFiles.Include(x => x.SurveyUserResponse).AsQueryable()
            : _context.SurveyUserResponseFiles.Include(x => x.SurveyUserResponse)
            .Where(predicate).AsQueryable();
    }

    public Task<SurveyUserResponseFile?> GetAsync(Expression<Func<SurveyUserResponseFile, bool>> predicate)
    {
        var item = _context.SurveyUserResponseFiles.Include(x => x.SurveyUserResponse).Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task AddAsync(SurveyUserResponseFile surveyUserResponseFile)
    {
        await _context.SurveyUserResponseFiles.AddAsync(surveyUserResponseFile);
    }
    public async Task RemoveAsync(SurveyUserResponseFile surveyUserResponseFile)
    {
        _context.SurveyUserResponseFiles.Remove(surveyUserResponseFile);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
