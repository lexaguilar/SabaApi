using Microsoft.Extensions.Options;
using MimeKit;
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

    Task<(bool success, UserPageResponseModel users, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null);
}

public class UsersServices : IUsersServices
{
    private readonly IUserRepository _userRepository;
    private readonly IFilialRepository _filialRepository;
    private readonly IMessageService _messageService;
    private readonly AppSettings _appSettings;

    public UsersServices(IUserRepository userRepository, IOptions<AppSettings> appSettings, IMessageService messageService, IFilialRepository filialRepository)
    {
        _filialRepository = filialRepository;
        _messageService = messageService;
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
            FilialIds = user.FilialUsers.Select(x => x.FilialId).ToArray()
        };
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Add(UserRequestModel m)
    {

        var existing = await _userRepository.GetAsync(x => x.UserName == m.UserName);
        if (existing != null)
            return (false, null, "Ya existe un usuario con ese nombre.");

        var password = PasswordHelper.GeneratePassword(8, 2, 2, 2, 2);
        var (pass, salt) = CryptoHelper.ComputePassword(password);
        var newUser = new User{
            UserName = m.UserName,
            RoleId = m.RoleId,
            Name = m.Name,
            LastName = m.LastName,
            Email = m.Email,
            IsActive = m.IsActive,
            Password = pass,
            PasswordSalt = salt,
            CreateDate = DateTime.UtcNow,
            LastLoginDate = DateTime.UtcNow,
            LastPasswordChangedDate = DateTime.UtcNow,
            LastLockoutDate = DateTime.UtcNow,
            FailedPasswordAttemptCount = 0,
            FailedPasswordAttemptWindowStart = DateTime.UtcNow,
            TempToken = null,
            TempTokenExpiration = DateTime.UtcNow,
        };

        if (m.FilialIds != null && m.FilialIds.Length > 0)
        {
            var filials = await _filialRepository.GetAllAsync(x => m.FilialIds.Contains(x.Id));
            foreach (var filial in filials)
            {
                newUser.FilialUsers.Add(new FilialUser
                {
                    UserId = newUser.Id,
                    FilialId = filial.Id
                });
            }
        }

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();
        
        var message = new MimeMessage
        {               
            To = { new MailboxAddress(m.Name, m.Email) }
        };

        message.Subject = "Bienvenido a Saba";
        message.Body = new TextPart("html")
        {
            Text = $"<h1>Bienvenido a Saba</h1><p>Su usuario es: {m.UserName}</p><p>Su contraseña es: {password}</p>"
        };

        await _messageService.SendAsync(message);

        var userModel = MapToUserResponseModel(newUser);
        return (true, userModel, null);

    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Disable(int id)
    {
        var user = await _userRepository.GetAsync(x => x.Id == id);
        if (user == null)
            return (false, null, "User not found");

        user.IsActive = false;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Enable(int id)    
    {
        var user = await _userRepository.GetAsync(x => x.Id == id);
        if (user == null)
            return (false, null, "User not found");

        user.IsActive = true;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null, null);
    }

    public async Task<(bool success, UserPageResponseModel users, string? message)> GetAll(int page, int pageSize, Dictionary<string, string> filters = null)
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

        var totalCount = users.Count();       

         if (filters.Any(x => x.Key == "all-items" && x.Value == "true"))
        {
            page = 0;
            pageSize = totalCount;
        }

        users = users.Skip(page).Take(pageSize);

        var userModels = users.ToArray()
        .Select(x => MapToUserResponseModel(x));

        var userPageResponseModel = new UserPageResponseModel
        {
            TotalCount = totalCount,
            Items = userModels
        };

        return (true, userPageResponseModel, null);
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> GetByUserName(string userName)
    {
        var user = await _userRepository.GetAsync(x => x.UserName == userName);
        if (user == null)
            return (false, null, "User not found");

        var userModel = MapToUserResponseModel(user);
        
        return (true, userModel, null);

    }

    public async Task<(bool success, UserResponseModel? user, string? message)> GetById(int id)
    {
        var user = await _userRepository.GetAsync(x => x.Id == id);
        if (user == null)
            return (false, null, "User not found");

        var userModel = MapToUserResponseModel(user);

        return (true, userModel, null);
    }

    public async Task<(bool success, UserResponseModel? user, string? message)> Update(UserRequestModel m)
    {
        var user = await _userRepository.GetAsync(x => x.Id == m.Id);
        if (user == null)
            return (false, null, "User not found");

        user.UserName = m.UserName;
        user.RoleId = m.RoleId;
        user.Name = m.Name;
        user.LastName = m.LastName;
        user.Email = m.Email;
        user.IsActive = m.IsActive;

        if (m.FilialIds != null && m.FilialIds.Length > 0)
        {
            var filials = await _filialRepository.GetAllAsync(x => m.FilialIds.Contains(x.Id));
            user.FilialUsers.Clear();
            foreach (var filial in filials)
            {
                user.FilialUsers.Add(new FilialUser
                {
                    UserId = user.Id,
                    FilialId = filial.Id
                });
            }
        }

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        var userModel = MapToUserResponseModel(user);

        return (true, userModel, null);
    }
}