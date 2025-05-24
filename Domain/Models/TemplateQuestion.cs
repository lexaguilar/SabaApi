using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class TemplateQuestion
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int TemplateId { get; set; }

    public int QuestionTypeId { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsRequired { get; set; }

    public string? Uuid { get; set; }

    public string? ParentUuid { get; set; }
    
    public int? CatalogNameId { get; set; }

    public virtual QuestionType QuestionType { get; set; } = null!;

    public virtual Template Template { get; set; } = null!;
}
