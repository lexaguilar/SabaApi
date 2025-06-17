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
        services.AddScoped<IGenericCatalogRepository, GenericCatalogRepository>();
        services.AddScoped<IGenericCatalogsServices, GenericCatalogsServices>();
        services.AddScoped<ICatalogNameRepository, CatalogNameRepository>();
        services.AddScoped<ICatalogNamesServices, CatalogNamesServices>();
        services.AddScoped<ITemplateQuestionRepository, TemplateQuestionRepository>();
        services.AddScoped<ITemplateQuestionsServices, TemplateQuestionsServices>();
        services.AddScoped<ISurveyRepository, SurveyRepository>(); 
        services.AddScoped<ISurveysServices, SurveysServices>();
        services.AddScoped<IFilialUserRepository, FilialUserRepository>();
        services.AddScoped<IFilialUsersServices, FilialUsersServices>();
        services.AddScoped<ISurveyUserRepository, SurveyUserRepository>();
        services.AddScoped<ISurveyUsersServices, SurveyUsersServices>();
        services.AddScoped<ISurveyUserResponseRepository, SurveyUserResponseRepository>();
        services.AddScoped<ISurveyUserResponsesServices, SurveyUserResponsesServices>();
        services.AddScoped<ISurveyUserResponseFileRepository, SurveyUserResponseFileRepository>();
        services.AddScoped<ISurveyUserResponseFilesServices, SurveyUserResponseFilesServices>();
        services.AddScoped<IFilesServices, FilesServices>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IResourcesServices, ResourcesServices>();
        return services;
    }
}