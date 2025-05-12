using System.Linq.Expressions;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface IClientRepository
{
    Task<IQueryable<Client>> GetAllAsync(Expression<Func<Client, bool>>? predicate = null);

    Task<Client?> GetAsync(Expression<Func<Client, bool>> predicate);

    Task AddAsync(Client client);

    Task UpdateAsync(Client client);

    Task<int> SaveChangesAsync();
}

public class ClientRepository : IClientRepository
{
    private readonly SabaContext _context;

    public ClientRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Client>> GetAllAsync(Expression<Func<Client, bool>>? expression)
    {
        if (expression == null)
            return _context.Clients.AsQueryable();

        return _context.Clients.Where(expression).AsQueryable();
    }

    public Task<Client> GetAsync(Expression<Func<Client, bool>> predicate)
    {
        var client = _context.Clients.Where(predicate).FirstOrDefault();

        return Task.FromResult(client);
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
    }

    public Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
       return await _context.SaveChangesAsync();
    }
}