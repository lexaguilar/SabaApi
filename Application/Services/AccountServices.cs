using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Saba.Application.Extensions;
using Saba.Application.Helpers;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface IAccountService
{
    Task<(bool success, LoginModelReponse? User, string? message)> Login(LoginModel m);

    Task<(bool success, string message)> ResetPassword(string email);

    Task<(bool success, string message)> ChangePassword(ChangePasswordModel m);
    Task<(bool success, string message)> ChangePassword(ChangePasswordManualModel m, string userName);
}

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly AppSettings _appSettings;
    private readonly IMessageService _messageService;


    public AccountService(IUserRepository userRepository, IOptions<AppSettings> appSettings, IMessageService messageService)
    {
        _messageService = messageService;
        _appSettings = appSettings.Value;
        _userRepository = userRepository;
    }

    public async Task<(bool success, string message)> ChangePassword(ChangePasswordModel m)
    {
        var user = await _userRepository.GetAsync(x => x.UserName == m.UserName);
        if (user == null)
            return (false, "User not found.");

        if (!CryptoHelper.ComparePassword(m.OldPassword, user.Password, user.PasswordSalt))
            return (false, "La contraseña actual no es correcta.");
        if (m.NewPassword != m.ConfirmPassword)
            return (false, "Las contraseñas no coinciden.");

        var (passwordHash, salt) = CryptoHelper.ComputePassword(m.NewPassword);
        user.Password = passwordHash;
        user.PasswordSalt = salt;
        user.LastPasswordChangedDate = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string message)> ChangePassword(ChangePasswordManualModel m, string userName)
    {
        var user = await _userRepository.GetAsync(x => x.UserName == m.UserName);
        if (user == null)
            return (false, "User not found.");

        var (passwordHash, salt) = CryptoHelper.ComputePassword(m.NewPassword);
        user.Password = passwordHash;
        user.PasswordSalt = salt;
        user.LastPasswordChangedDate = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        if (m.SendEmail)
        {
            var message = new MimeMessage
            {
                To = { new MailboxAddress(user.Name + ' ' + user.LastName, user.Email) }
            };

            var currentUser = await _userRepository.GetAsync(x => x.UserName == userName);
            if (currentUser != null)
                message.Cc.Add(new MailboxAddress(currentUser.Name + ' ' + currentUser.LastName, currentUser.Email));

            message.Subject = "Saba - Cambio de contraseña";
            message.Body = new TextPart("html")
            {
                Text = $"<h1>Cambio de contraseña</h1><p>Su contraseña ha sido cambiada exitosamente.</p><p>Su nueva contraseña es: {m.NewPassword}</p>"
            };

            // Send email with the token
            await _messageService.SendAsync(message);
        }

        return (true, null);
    }

    public async Task<(bool success, string message)> ResetPassword(string email)
    {
        var user = await _userRepository.GetAsync(x => x.Email == email);
        if (user == null)
            user = await _userRepository.GetAsync(x => x.UserName == email);
        if (user == null)
            return (false, "No se encontró el usuario.");

        var password = PasswordHelper.GeneratePassword(8, 2, 2, 2, 2);
        var (passwordHash, salt) = CryptoHelper.ComputePassword(password);

        user.LastPasswordChangedDate = DateTime.UtcNow;
        user.Password = passwordHash;
        user.PasswordSalt = salt;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        var message = new MimeMessage
        {
            To = { new MailboxAddress(user.Name, user.Email) }
        };

        message.Subject = "Saba - Restablecimiento de contraseña";
        message.Body = new TextPart("html")
        {
            Text = $"<h1>Restablecimiento de contraseña</h1><p>Su nueva contraseña es: {password}</p>"
        };

        // Send email with the token
        await _messageService.SendAsync(message);

        return (true, null);
    }


    public async Task<(bool success, LoginModelReponse? User, string message)> Login(LoginModel m)
    {
        var user = await _userRepository.GetAsync(x => x.UserName == m.UserName);

        if (user == null || !CryptoHelper.ComparePassword(m.Password, user.Password, user.PasswordSalt))
        {
            return (false, null, "Invalid username or password.");
        }

        user.LastLoginDate = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        var userModel = new UserModel
        {
            Id = user.Id,
            IsAdmin = user.IsAdmin ?? false,
            DisplayName = user.UserName,
            Role = user.Role.Name,
            RoleId = user.RoleId,
            UserName = user.UserName,
            Email = user.Email,
            CountryId = user.CountryId,
            CountryName = user.Country.Name
        };

        var userReponse = new LoginModelReponse
        {
            User = userModel,
            AccessResources = user.Role.RoleResources != null ? string.Join(",", user.Role.RoleResources.Where(x => x.Action == 1).Select(x => x.ResourceKey)) : string.Empty
        };

        var token = AuthenticateUser(userReponse);
        userReponse.Token = token.Token;

        return (true, userReponse, null);
    }

    private (string Token, DateTime Expires) AuthenticateUser(LoginModelReponse m)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretToken));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        DateTime expires = DateTime.UtcNow.AddHours(10);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = m.GetClaimsIdentity(),
            Expires = expires,
            SigningCredentials = credentials
        };

        var securityToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(securityToken);
        return (jwt, expires);
    }
}
