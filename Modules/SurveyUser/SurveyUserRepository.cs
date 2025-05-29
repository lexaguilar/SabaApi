using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ISurveyUserRepository
{
    Task<IQueryable<SurveyUser>> GetAllAsync(Expression<Func<SurveyUser, bool>>? predicate = null);
    Task<SurveyUser?> GetAsync(Expression<Func<SurveyUser, bool>> predicate);
    Task AddAsync(SurveyUser surveyUser);
    Task UpdateAsync(SurveyUser surveyUser);
    Task RemoveAsync(SurveyUser surveyUser);
    Task<int> SaveChangesAsync();
}

public class SurveyUserRepository : ISurveyUserRepository
{
    private readonly SabaContext _context;

    public SurveyUserRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<SurveyUser>> GetAllAsync(Expression<Func<SurveyUser, bool>>? predicate = null)
    {
        return predicate == null
            ? _context.SurveyUsers.AsQueryable()
            : _context.SurveyUsers
                .Include(x => x.Survey)
                .Include(x => x.User)
                .Include(x => x.Filial)
                .Include(x => x.SurveyUserState)
            .Where(predicate).AsQueryable();
    }

    public Task<SurveyUser?> GetAsync(Expression<Func<SurveyUser, bool>> predicate)
    {
        var item = _context.SurveyUsers.Where(predicate).FirstOrDefault();
        return Task.FromResult(item);
    }

    public async Task RemoveAsync(SurveyUser surveyUser)
    {
        _context.SurveyUsers.Remove(surveyUser);
    }

    public async Task AddAsync(SurveyUser surveyUser)
    {
        await _context.SurveyUsers.AddAsync(surveyUser);
    }

    public async Task UpdateAsync(SurveyUser surveyUser)
    {
        _context.SurveyUsers.Update(surveyUser);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
