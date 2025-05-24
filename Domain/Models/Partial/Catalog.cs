using Saba.Repository;

namespace Saba.Domain.Models;

interface ICatalogo
{
}

public partial class Role : ICatalogo { }
public partial class Filial : ICatalogo { }
public partial class CatalogName : ICatalogo { }
