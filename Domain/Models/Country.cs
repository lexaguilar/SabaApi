using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }

    public virtual ICollection<CatalogName> CatalogNames { get; set; } = new List<CatalogName>();

    public virtual ICollection<Filial> Filials { get; set; } = new List<Filial>();

    public virtual ICollection<GenericCatalog> GenericCatalogs { get; set; } = new List<GenericCatalog>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();

    public virtual ICollection<Template> Templates { get; set; } = new List<Template>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
