using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class GenericCatalog
{
    public int Id { get; set; }

    public string CatalogName { get; set; } = null!;

    public string CatalogValue { get; set; } = null!;
}
