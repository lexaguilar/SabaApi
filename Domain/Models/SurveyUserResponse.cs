using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class SurveyUserResponse
{
    public int Id { get; set; }

    public int SurveyUserId { get; set; }

    public int QuestionId { get; set; }

    public string? Response { get; set; }

    public DateTime? CompletedAt { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? FileNameUploaded { get; set; }

    public virtual TemplateQuestion Question { get; set; } = null!;

    public virtual SurveyUser SurveyUser { get; set; } = null!;

    public virtual ICollection<SurveyUserResponseFile> SurveyUserResponseFiles { get; set; } = new List<SurveyUserResponseFile>();
}
