using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saba.Application.Extensions;
using Saba.Application.Helpers;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface ICatalogsServices
{
    IEnumerable<object> GetAll(string name);
}

public class CatalogsServices : ICatalogsServices
{
    private readonly ICatalogRepository _catalogRepository;
    
    public CatalogsServices(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public IEnumerable<object> GetAll(string name)
    {
        return _catalogRepository.GetAll(name);
    }
}
