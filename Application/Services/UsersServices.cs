using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saba.Application.Extensions;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;

namespace Saba.Repository;

public interface IUsersServices
{
    Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Add(UserRequestModel m);

    Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Update(UserRequestModel m);

    Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Disable(string userName);

    Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Enable(string userName);

    Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> GetByUserName(string userName);

    Task<(bool Success, List<UserRequestModel>? Users, string? ErrorMsg)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class UsersServices : IUsersServices
{
    private readonly IUserRepository _userRepository;
    private readonly AppSettings _appSettings;

    public UsersServices(IUserRepository userRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _userRepository = userRepository;
    }

    public async Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Add(UserRequestModel m)
    {
        var password = PasswordHelper.GeneratePassword(8, 2, 2, 2, 2);
        var pwdResult = CryptoHelper.ComputePassword(password);
        var newUser = new User{
            UserName = m.UserName,
            RoleId = m.RoleId,
            Name = m.Name,
            LastName = m.LastName,
            Email = m.Email,
            IsActive = m.IsActive,
            Password = pwdResult.PasswordHash,
            PasswordSalt = pwdResult.Salt,
            CreateDate = DateTime.UtcNow,
            LastLoginDate = DateTime.UtcNow,
            LastPasswordChangedDate = DateTime.UtcNow,
            LastLockoutDate = DateTime.UtcNow,
            FailedPasswordAttemptCount = 0,
            FailedPasswordAttemptWindowStart = DateTime.UtcNow,
            TempToken = null,
            TempTokenExpiration = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(newUser);
        _userRepository.SaveChangesAsync();

        return (true, m, null);

    }

    public async Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Disable(string userName)
    {
        var user = await _userRepository.Get(x => x.UserName == userName);
        if (user == null)
            return (false, null, "User not found");

        user.IsActive = false;

        _userRepository.Update(user);
        _userRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Enable(string userName)
    {
        var user = await _userRepository.Get(x => x.UserName == userName);
        if (user == null)
            return (false, null, "User not found");

        user.IsActive = true;

        _userRepository.Update(user);
        _userRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool Success, List<UserRequestModel>? Users, string? ErrorMsg)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
    {
        var users = await _userRepository.GetAllAsync(x => x.IsActive == true);

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                var value = filter.Value.ToLower();
                if(filter.Key == "userName")
                    users = users.Where(x => x.UserName.ToLower().StartsWith(value));

                if (filter.Key == "email")
                    users = users.Where(x => x.Email.ToLower().StartsWith(value));

                if (filter.Key == "name")
                    users = users.Where(x => x.Name.ToLower().StartsWith(value));

                if (filter.Key == "lastName")
                    users = users.Where(x => x.LastName.ToLower().StartsWith(value));

                if (filter.Key == "roleId"){
                    if (int.TryParse(value, out var roleId))
                        users = users.Where(x => x.RoleId == roleId);
                }

                if (filter.Key == "isActive")
                {
                    if (bool.TryParse(value, out var isActive))
                        users = users.Where(x => x.IsActive == isActive);
                }

            }
        }

        if (page > 0 && pageSize > 0)
        {
            users = users.Skip(page).Take(pageSize);
        }

        var userModels = users.Select(x => new UserRequestModel
        {
            UserName = x.UserName,
            RoleId = x.RoleId,
            Name = x.Name,
            LastName = x.LastName,
            Email = x.Email,
            IsActive = x.IsActive
        }).ToList();

        return (true, userModels, null);
    }

    public async Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> GetByUserName(string userName)
    {
        var user = await _userRepository.Get(x => x.UserName == userName);
        if (user == null)
            return (false, null, "User not found");

        var userModel = new UserRequestModel
        {
            UserName = user.UserName,
            RoleId = user.RoleId,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive
        };

        return (true, userModel, null);
    }

    public async Task<(bool Success, UserRequestModel? User, string? ErrorMsg)> Update(UserRequestModel m)
    {
        var user = await _userRepository.Get(x => x.UserName == m.UserName);
        if (user == null)
            return (false, null, "User not found");

        user.RoleId = m.RoleId;
        user.Name = m.Name;
        user.LastName = m.LastName;
        user.Email = m.Email;
        user.IsActive = m.IsActive;

        _userRepository.Update(user);
        _userRepository.SaveChangesAsync();

        return (true, m, null);
    }
}