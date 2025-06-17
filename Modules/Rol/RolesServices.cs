using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface IRolesServices
{
    Task<(bool success, RoleResponseModel? role, string? message)> Add(RoleRequestModel m);
    Task<(bool success, RoleResponseModel? role, string? message)> Update(RoleRequestModel m);
    Task<(bool success, RoleResponseModel? role, string? message)> Disable(int id);
    Task<(bool success, RoleResponseModel? role, string? message)> Enable(int id);
    Task<(bool success, RoleResponseModel? role, string? message)> GetById(int id);
    Task<(bool success, RoleResponseModel? role, string? message)> UpdateResources(int id, ResourcesRequestModel model);
    Task<(bool success, RolePageResponseModel roles, string? message)> GetAll(int page, int pageSize, Dictionary<string, string>? filters = null);
}

public class RolesServices : IRolesServices
{
    private readonly IRoleRepository _roleRepository;

    public RolesServices(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    private RoleResponseModel MapToRoleResponseModel(Role role)
    {
        return new RoleResponseModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Active = role.Active,
            CreatedAt = role.CreatedAt,
            CreatedByUserId = role.CreatedByUserId,
            EditedAt = role.EditedAt,
            EditedByUserId = role.EditedByUserId,
            RoleResources = role.RoleResources.Select(rr => new RoleResourceResponseModel
            {
                ResourceKey = rr.ResourceKey,
                Name = rr.ResourceKeyNavigation?.Name ?? string.Empty,
                Description = rr.ResourceKeyNavigation?.Description ?? string.Empty,
                ParentResourceKey = rr.ResourceKeyNavigation?.ParentResourceKey ?? string.Empty
            }).ToList()
        };
    }

    public async Task<(bool success, RoleResponseModel? role, string? message)> Add(RoleRequestModel m)
    {
        var existing = await _roleRepository.GetAsync(x => x.Name == m.Name);
        if (existing != null)
            return (false, null, "Ya existe un rol con ese nombre.");

        var newRole = new Role
        {
            Name = m.Name,
            Description = m.Description,
            Active = m.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = m.UserId
        };

        await _roleRepository.AddAsync(newRole);
        await _roleRepository.SaveChangesAsync();
        return (true, MapToRoleResponseModel(newRole), null);
    }

    public async Task<(bool success, RoleResponseModel? role, string? message)> Update(RoleRequestModel m)
    {
        var role = await _roleRepository.GetAsync(x => x.Id == m.Id);
        if (role == null)
            return (false, null, "Rol no encontrado.");

        role.Name = m.Name;
        role.Description = m.Description;
        role.Active = m.Active;
        role.EditedAt = DateTime.UtcNow;
        role.EditedByUserId = m.UserId;

        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
        return (true, MapToRoleResponseModel(role), null);
    }

    public async Task<(bool success, RoleResponseModel? role, string? message)> Disable(int id)
    {
        var role = await _roleRepository.GetAsync(x => x.Id == id);
        if (role == null)
            return (false, null, "Rol no encontrado.");
        role.Active = false;
        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
        return (true, MapToRoleResponseModel(role), null);
    }

    public async Task<(bool success, RoleResponseModel? role, string? message)> Enable(int id)
    {
        var role = await _roleRepository.GetAsync(x => x.Id == id);
        if (role == null)
            return (false, null, "Rol no encontrado.");
        role.Active = true;
        await _roleRepository.UpdateAsync(role);
        await _roleRepository.SaveChangesAsync();
        return (true, MapToRoleResponseModel(role), null);
    }

    public async Task<(bool success, RolePageResponseModel roles, string? message)> GetAll(int page, int pageSize, Dictionary<string, string>? filters = null)
    {
        var roles = await _roleRepository.GetAllAsync();

        if (filters != null)
        {
            foreach (var filter in filters)
            {
                if (filter.Key == "name")
                {
                    roles = roles.Where(x => x.Name.Contains(filter.Value));
                }
                else if (filter.Key == "active" && bool.TryParse(filter.Value, out bool act))
                {
                    roles = roles.Where(x => x.Active == act);
                }
            }
        }

        var totalCount = roles.Count();
        roles = roles.Skip(page).Take(pageSize);

        var rolePageResponseModel = new RolePageResponseModel
        {
            TotalCount = totalCount,
            Items = roles.ToArray().Select(MapToRoleResponseModel)
        };
        return (true, rolePageResponseModel, null);

    }

    public async Task<(bool success, RoleResponseModel? role, string? message)> GetById(int id)
    {
        var role = await _roleRepository.GetAsync(x => x.Id == id);
        if (role == null)
            return (false, null, "Rol no encontrado.");
        return (true, MapToRoleResponseModel(role), null);
    }

    public async Task<(bool success, RoleResponseModel? role, string? message)> UpdateResources(int id, ResourcesRequestModel model)
    {
        var role = await _roleRepository.GetAsync(x => x.Id == id);
        if (role == null)
            return (false, null, "Rol no encontrado.");
        //remove all existing resources
      
        foreach (var resourceKey in model.ResourcesKey)
        {
            role.RoleResources.Add(new RoleResource
            {
                ResourceKey = resourceKey,
                Action = 1, // Assuming action is always 1 for this case
                RoleId = role.Id
            });
        }

        await _roleRepository.UpdateResourcesAsync(role.Id, role.RoleResources.ToArray());
        await _roleRepository.SaveChangesAsync();
        return (true, MapToRoleResponseModel(role), null);
    }
}