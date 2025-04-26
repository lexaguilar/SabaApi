using Saba.Domain.Models;

namespace Saba.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}