using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;

namespace Saba.Repository;

public interface ISurveyUserRepository
{
    Task<IQueryable<SurveyUser>> GetAllAsync(Expression<Func<SurveyUser, bool>>? predicate = null);
    Task<SurveyUser?> GetAsync(Expression<Func<SurveyUser, bool>> predicate);
    Task<SurveyUserIssuesResponseModel[]> GetIssuesFoundAsync(int countryId, DateTime date);
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
            ? _context.SurveyUsers
                .Include(x => x.User)
                .Include(x => x.Filial)
                // .Include(x => x.SurveyUserState)
                .Include(x => x.Survey)
                .Include(x => x.SurveyUserResponses).ThenInclude(x => x.Question)
            .AsQueryable()
            : _context.SurveyUsers
                .Include(x => x.User)
                .Include(x => x.Filial)
                // .Include(x => x.SurveyUserState)
                .Include(x => x.Survey)
                .Include(x => x.SurveyUserResponses).ThenInclude(x => x.Question)
            .Where(predicate).AsQueryable();
    }

    public Task<SurveyUser?> GetAsync(Expression<Func<SurveyUser, bool>> predicate)
    {
        var item = _context.SurveyUsers
            .Include(x => x.User)
            .Include(x => x.Filial)
            .Include(x => x.SurveyUserState)
            .Include(x => x.Survey)
            .Include(x => x.SurveyUserResponses).ThenInclude(x => x.Question)
        .Where(predicate).FirstOrDefault();
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

    public Task<SurveyUserIssuesResponseModel[]> GetIssuesFoundAsync(int countryId, DateTime date)
    {
       
        var result = _context.SurveyUserResponses
        .Where(sur => sur.Response == "No" &&
                    sur.SurveyUser.CreatedAt.Year == date.Year &&
                    sur.SurveyUser.CreatedAt.Month == date.Month &&
                    sur.SurveyUser.Survey.CountryId == countryId)
        .GroupBy(sur => new
        {
            sur.SurveyUser.FilialId,
            FilialName = sur.SurveyUser.Filial.Name
        })
        .Select(g => new SurveyUserIssuesResponseModel
        {
            FilialId = g.Key.FilialId,
            FilialName = g.Key.FilialName,
            Issues = g.Count()
        })
        .ToList();

        return Task.FromResult(result.OrderByDescending(x => x.Issues).ToArray());
    }
}
