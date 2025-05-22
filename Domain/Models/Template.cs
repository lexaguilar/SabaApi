using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Template
{
    public int Id { get; set; }

    public string? TemplateCode { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();

    public virtual ICollection<TemplateQuestion> TemplateQuestions { get; set; } = new List<TemplateQuestion>();
}
