using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class SurveyState
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public bool Active { get; set; }

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();
}
