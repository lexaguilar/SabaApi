using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Survey
{
    public int Id { get; set; }

    public int TemplateId { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool ApplyAllUser { get; set; }

    public int? MinGoal { get; set; }

    public int? ExpectedGoal { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<SurveyUser> SurveyUsers { get; set; } = new List<SurveyUser>();

    public virtual Template Template { get; set; } = null!;
}
