using System.Data;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ISurveyUserResponseRepository
{
    Task<IQueryable<SurveyUserResponse>> GetAllAsync(Expression<Func<SurveyUserResponse, bool>>? predicate = null);
    Task<DataTable> GetAllPivotAsync(int SurveyId);
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
            ? _context.SurveyUserResponses
            .Include(x => x.SurveyUserResponseFiles)
            .Include(x => x.SurveyUser)
            .Include(x => x.Question)
            .ThenInclude(x => x.CatalogName)
            .AsQueryable()
            : _context.SurveyUserResponses
            .Include(x => x.SurveyUserResponseFiles)
            .Include(x => x.SurveyUser)
            .Include(x => x.Question)
            .ThenInclude(x => x.CatalogName)
            .Where(predicate).AsQueryable();
    }

    public async Task<DataTable> GetAllPivotAsync(int SurveyId)
    {
        var datatable = new DataTable();
        var connection = _context.Database.GetDbConnection() as SqlConnection;
        {
            using (var cmd = new SqlCommand("ups_GetResponsesBySurveyId", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@SurveyId", SurveyId));

                connection.Open();
                var reader = cmd.ExecuteReader();
                datatable.Load(reader);

                connection.Close();
            }
        }

        return datatable;
    }



    public Task<SurveyUserResponse?> GetAsync(Expression<Func<SurveyUserResponse, bool>> predicate)
    {
        var item = _context.SurveyUserResponses
        .Include(x => x.SurveyUserResponseFiles)
        .Include(x => x.SurveyUser)
        .Include(x => x.Question)
        .ThenInclude(x => x.CatalogName).Where(predicate).FirstOrDefault();
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
