using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Saba.Domain.ViewModels;

namespace Saba.Application.Extensions;

public static class ClaimPrincipalExtension
{

    public static ClaimsIdentity GetClaimsIdentity(this LoginModelReponse m, string authenticationScheme = JwtBearerDefaults.AuthenticationScheme)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, m.User.UserName),
                new Claim(ClaimTypes.Email, m.User.Email),
                new Claim(ClaimTypes.Role, m.User.RoleId.ToString())
             };

        var claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);

        return claimsIdentity;
    }

    internal static UserTeminal GetUser(this ClaimsPrincipal principal)
    {
        UserTeminal usr = new UserTeminal();
        foreach (var claim in principal.Claims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.NameIdentifier:
                    usr.UserName = claim.Value; break;
                case ClaimTypes.Email:
                    usr.Email = claim.Value; break;
                case ClaimTypes.Role:
                    usr.Role = claim.Value; break;

            }
        }
        return usr;
    }

    internal class UserTeminal
    {
        public int Id { get; set; }

        public string? Role { get; set; }

        public int RoleId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
    }
}