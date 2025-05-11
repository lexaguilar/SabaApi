using System.Reflection;
using Saba.Domain.Models;

namespace Saba.Repository;

public interface ICatalogRepository
{
    IEnumerable<object> GetAll(string name);

}


public class CatalogRepository : ICatalogRepository
    {

        private readonly SabaContext _context;
        readonly string CatalogoInterfaceTypeName = typeof(ICatalogo).FullName;
        public CatalogRepository(SabaContext context)
        {
            _context = context;
          
        }

        public IEnumerable<object> GetAll(string name)
        {
              Type type = typeof(SabaContext);
            IEnumerable<object> result = null;
            //var context = Activator.CreateInstance(type);
            TypeFilter catalogFilter = new TypeFilter(CatalogoInterfaceFilter);

            var property = type.GetProperties().FirstOrDefault(x => x.GetMethod.IsVirtual
                && x.PropertyType.IsGenericType && (x.PropertyType.GenericTypeArguments[0].FindInterfaces(catalogFilter, CatalogoInterfaceTypeName).Length != 0
                && x.PropertyType.GenericTypeArguments[0].Name == name)
            );

            if (property != null)
                result = property.GetMethod.Invoke(_context, null) as IEnumerable<object>;

            return result;
        }
     

        private static bool CatalogoInterfaceFilter(Type m, object filterCriteria)
        {
            return m.ToString() == filterCriteria.ToString();
        }
    }