using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class CatalogName
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }

    public bool Active { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; }

    public virtual ICollection<GenericCatalog> GenericCatalogs { get; set; } = new List<GenericCatalog>();

    public virtual ICollection<TemplateQuestion> TemplateQuestions { get; set; } = new List<TemplateQuestion>();
}
