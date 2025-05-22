using Saba.Domain.Models;
using Saba.Application.Services;

namespace Saba.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUsersServices, UsersServices>();
        services.AddScoped<IFilialRepository, FilialRepository>();
        services.AddScoped<IFilialsServices, FilialsServices>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ICatalogsServices, CatalogsServices>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IClientsServices, ClientsServices>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRolesServices, RolesServices>();
        services.AddTransient<IMessageService, MessageService>();       
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<ITemplatesServices, TemplatesServices>();
        services.AddScoped<ICatalogGenericRepository, CatalogGenericRepository>();
        services.AddScoped<ICatalogGenericsServices, CatalogGenericsServices>();
        return services;
    }
}