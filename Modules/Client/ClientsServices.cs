using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface IClientsServices
{
    Task<(bool success, ClientResponseModel? client, string? errorMsg)> Add(ClientRequestModel m);

    Task<(bool success, ClientResponseModel? client, string? errorMsg)> Update(ClientRequestModel m);

    Task<(bool success, ClientResponseModel? client, string? errorMsg)> Disable(int id);

    Task<(bool success, ClientResponseModel? client, string? errorMsg)> Enable(int id);

    Task<(bool success, ClientResponseModel? client, string? errorMsg)> GetById(int id);

    Task<(bool success, IEnumerable<ClientResponseModel>? clients, string? errorMsg)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class ClientsServices : IClientsServices
{
    private readonly IClientRepository _clientRepository;
    private readonly AppSettings _appSettings;

    public ClientsServices(IClientRepository clientRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _clientRepository = clientRepository;
    }

    private ClientResponseModel MapToClientResponseModel(Client client)
    {
        return new ClientResponseModel
        {
            Id = client.Id,
            Name = client.Name,
            Address = client.Address,
            Active = client.Active,
            TypeId = client.TypeId,
            DateAdded = client.DateAdded,
            Amount = client.Amount,
            CreatedAt = client.CreatedAt,
            CreatedByUserId = client.CreatedByUserId,
            EditedAt = client.EditedAt,
            EditedByUserId = client.EditedByUserId
        };
    }

    public async Task<(bool success, ClientResponseModel? client, string? errorMsg)> Add(ClientRequestModel m)
    {
        var existingClient = await _clientRepository.GetAsync(x => x.Name == m.Name);
        if (existingClient != null)        
            return (false, null, "Ya existe una cliente con ese nombre.");

        var newClient = new Client
        {
            Name = m.Name,
            Address = m.Address,
            Active = m.Active,
            TypeId = m.TypeId,
            DateAdded = m.DateAdded,
            Amount = m.Amount,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId           
        };

        await _clientRepository.AddAsync(newClient);
        await _clientRepository.SaveChangesAsync();

        var clientModel = MapToClientResponseModel(newClient);
        return (true, clientModel, null);

    }

    public async Task<(bool success, ClientResponseModel? client, string? errorMsg)> Update(ClientRequestModel m)
    {
        var client = await _clientRepository.GetAsync(x => x.Id == m.Id);

        if (client == null)        
            return (false, null, "Client not found.");
        
        client.Name = m.Name;
        client.Address = m.Address;
        client.Active = m.Active;
        client.TypeId = m.TypeId;
        client.DateAdded = m.DateAdded;
        client.Amount = m.Amount;

        client.EditedAt = DateTime.UtcNow;
        client.EditedByUserId = m.UserId;        

        await _clientRepository.UpdateAsync(client);
        await _clientRepository.SaveChangesAsync();

        var clientModel = MapToClientResponseModel(client);

        return (true, clientModel, null);

    }

    public async Task<(bool success, ClientResponseModel? client, string? errorMsg)> Disable(int id)
    {
        var client = await _clientRepository.GetAsync(x => x.Id == id);

        if (client == null)
            return (false, null, "Client not found.");

        client.Active = false;

        await _clientRepository.UpdateAsync(client);
        await _clientRepository.SaveChangesAsync();

        var clientModel = MapToClientResponseModel(client);
        
        return (true, clientModel, null);
    }

    public async Task<(bool success, ClientResponseModel? client, string? errorMsg)> Enable(int id)
    {
        var client = await _clientRepository.GetAsync(x => x.Id == id);

        if (client == null)
            return (false, null, "Client not found.");

        client.Active = true;

        await _clientRepository.UpdateAsync(client);
        await _clientRepository.SaveChangesAsync();

        var clientModel = MapToClientResponseModel(client);
        
        return (true, clientModel, null);
    }

    public async Task<(bool success, IEnumerable<ClientResponseModel>? clients, string? errorMsg)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {

        var clients = await _clientRepository.GetAllAsync();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {

                if (filter.Key == "name")
                {
                    clients = clients.Where(x => x.Name.Contains(filter.Value));
                }
                else if (filter.Key == "typeId")
                {
                    clients = clients.Where(x => x.TypeId == int.Parse(filter.Value));
                }
                else if (filter.Key == "active")
                {
                    if (bool.TryParse(filter.Value, out bool isActive))
                    {
                        clients = clients.Where(x => x.Active == isActive);
                    }
                }

            }
        }

        if (page > 0 && pageSize > 0)
        {
            clients = clients.Skip(page).Take(pageSize);
        }

        var clientModels = clients.ToArray().Select(x => MapToClientResponseModel(x)).ToArray();

        return (true, clientModels, null);
    }

    public async Task<(bool success, ClientResponseModel? client, string? errorMsg)> GetById(int id)
    {
        var client = await _clientRepository.GetAsync(x => x.Id == id);

        if (client == null)
        {
            return (false, null, "Client not found.");
        }

        var clientModel = MapToClientResponseModel(client);

        return (true, clientModel, null);
    }
}