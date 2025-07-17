using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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
                new Claim(ClaimTypes.Role, m.User.RoleId.ToString()),
                new Claim(ClaimTypes.Name, m.User.UserName),
                new Claim(ClaimTypes.Actor, m.User.Id.ToString()),
                new Claim(AppClaimTypes.Resources, m.AccessResources),
                new Claim(AppClaimTypes.CountryId, m.User.CountryId.ToString()),
                new Claim(AppClaimTypes.CountryName, m.User.CountryName),
                new Claim(AppClaimTypes.IsAdmin, m.User.IsAdmin.ToString())
             };

        var claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);

        return claimsIdentity;
    }

    internal static UserTeminal GetUser(this ControllerBase controller)
    {
        UserTeminal usr = new UserTeminal();
        var identity = controller.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            foreach (var claim in identity.Claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.NameIdentifier:
                        usr.UserName = claim.Value; break;
                    case ClaimTypes.Email:
                        usr.Email = claim.Value; break;
                    case ClaimTypes.Role:
                        usr.RoleId = int.Parse(claim.Value); break;
                    case ClaimTypes.Actor:
                        usr.Id = int.Parse(claim.Value); break;
                    case ClaimTypes.Name:
                        usr.UserName = claim.Value; break;
                    case AppClaimTypes.Resources:
                        usr.Resources = claim.Value.Split(',').Select(x => x.Trim()).ToArray();
                        break;
                    case AppClaimTypes.CountryId:
                        usr.CountryId = int.Parse(claim.Value);
                        break;
                    case AppClaimTypes.CountryName:
                        usr.CountryName = claim.Value;  break;
                    case AppClaimTypes.IsAdmin:
                        usr.IsAdmin = bool.Parse(claim.Value); break;
                }
            }
        }
        return usr;
    }

    internal class UserTeminal
    {
        public int Id { get; set; }
        public string? Role { get; set; }
        public int RoleId { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Resources { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class AppClaimTypes
    {
        internal const string Resources = "Resources";
        internal const string CountryId = "CountryId";
        internal const string CountryName = "CountryName";
        internal const string IsAdmin = "IsAdmin";
    }
}