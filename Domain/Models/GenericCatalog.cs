using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class GenericCatalog
{
    public int Id { get; set; }

    public int CatalogNameId { get; set; }

    public string CatalogValue { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }

    public bool Active { get; set; }

    public virtual CatalogName CatalogName { get; set; } = null!;

}
