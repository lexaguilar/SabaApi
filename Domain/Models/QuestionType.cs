using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class QuestionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TemplateQuestion> TemplateQuestions { get; set; } = new List<TemplateQuestion>();
}
