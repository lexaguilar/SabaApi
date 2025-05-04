using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saba.Application.Extensions;
using Saba.Application.Helpers;
using Saba.Domain.ViewModels;

namespace Saba.Repository;

public interface IAccountService
{
    Task<(bool Success, LoginModelReponse? User, string? ErrorMsg)> Login(LoginModel m);

    string ForgotPassword(string email);

    (bool Success, string Email) ForgotPasswordConfirmation(ForgotPasswordConfirmationModel m);

    (bool Success, string ErrorMsg) ChangePassword(ChangePasswordModel m);
}

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly AppSettings _appSettings;

    public AccountService(IUserRepository userRepository, IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _userRepository = userRepository;
    }

    public (bool Success, string ErrorMsg) ChangePassword(ChangePasswordModel m)
    {
        throw new NotImplementedException();
    }

    public string ForgotPassword(string email)
    {
        throw new NotImplementedException();
    }

    public (bool Success, string Email) ForgotPasswordConfirmation(ForgotPasswordConfirmationModel m)
    {
        throw new NotImplementedException();
    }

    public async Task<(bool Success, LoginModelReponse? User, string ErrorMsg)> Login(LoginModel m)
    {
        var user = await _userRepository.Get(x => x.UserName == m.UserName);

        if (user == null || !CryptoHelper.ComparePassword(m.Password, user.Password, user.PasswordSalt))
        {
                return (false, null,  "Invalid username or password.");
        }

        var userModel = new UserModel
        {
            DisplayName = user.UserName,
            Role = user.Role.Name,
            RoleId = user.RoleId,
            UserName = user.UserName,
            Email = user.Email,
        };

        var userReponse = new LoginModelReponse
        {
            User = userModel,
        };

        var token = AuthenticateUser(userReponse);
        userReponse.Token = token.Token;
        //userReponse.Expires = token.Expires;

        return (true, userReponse, null);
    }

    private (string Token, DateTime Expires) AuthenticateUser(LoginModelReponse m)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.SecretToken));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        DateTime expires = DateTime.UtcNow.AddMinutes(10);

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
