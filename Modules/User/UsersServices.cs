using Microsoft.Extensions.Options;
using Saba.Application.Extensions;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface IUsersServices
{
    Task<(bool success, UserResponseModel? user, string? message)> Add(UserRequestModel m);

    Task<(bool success, UserResponseModel? user, string? message)> Update(UserRequestModel m);

    Task<(bool success, UserResponseModel? user, string? message)> Disable(int id);

    Task<(bool success, UserResponseModel? user, string? message)> Enable(int id);

    Task<(bool success, UserResponseModel? user, string? message)> GetById(int id);

    Task<(bool success, UserResponseModel? user, string? message)> GetByUserName(string userName);

    Task<(bool success, IEnumerable<UserResponseModel>? users, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
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

    private UserResponseModel MapToUserResponseModel(User user)
    {
        return new UserResponseModel
        {
            Id = user.Id,
            UserName = user.UserName,
            RoleId = user.RoleId,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            CreateDate = user.CreateDate,
            LastLoginDate = user.LastLoginDate,
        };
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Add(UserRequestModel m)
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
        await _userRepository.SaveChangesAsync();

        return (true, null, null);

    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Disable(int id)
    {
        var user = await _userRepository.Get(x => x.Id == id);
        if (user == null)
            return (false, null, "User not found");

        user.IsActive = false;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Enable(int id)    
    {
        var user = await _userRepository.Get(x => x.Id == id);
        if (user == null)
            return (false, null, "User not found");

        user.IsActive = true;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool success, IEnumerable<UserResponseModel>? users, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
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
                    users = users.Where(x => x.Name != null && x.Name.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

                if (filter.Key == "lastName")
                    users = users.Where(x => x.LastName != null && x.LastName.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

                if (filter.Key == "roleId")
                    if (int.TryParse(value, out var roleId))
                        users = users.Where(x => x.RoleId == roleId);                

                if (filter.Key == "isActive")                
                    if (bool.TryParse(value, out var isActive))
                        users = users.Where(x => x.IsActive == isActive);
                

            }
        }

        if (page > 0 && pageSize > 0)
            users = users.Skip(page).Take(pageSize);

        var userModels = users.ToArray()
        .Select(x => MapToUserResponseModel(x)).ToArray();

        return (true, userModels, null);
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> GetByUserName(string userName)
    {
        var user = await _userRepository.Get(x => x.UserName == userName);
        if (user == null)
            return (false, null, "User not found");

        var userModel = MapToUserResponseModel(user);
        
        return (true, userModel, null);

    }

    public async Task<(bool success, UserResponseModel? user, string? message)> GetById(int id)
    {
        var user = await _userRepository.Get(x => x.Id == id);
        if (user == null)
            return (false, null, "User not found");

        var userModel = MapToUserResponseModel(user);

        return (true, userModel, null);
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Update(UserRequestModel m)
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
        await _userRepository.SaveChangesAsync();

        var userModel = MapToUserResponseModel(user);

        return (true, userModel, null);
    }
}