using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class SurveyUserResponseRequestModel
{
    public int Id { get; set; }
    public int SurveyUserId { get; set; }
    public int QuestionId { get; set; }
    public string? Response { get; set; }
    public DateTime? CompletedAt { get; set; } 
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? FileNameUploaded { get; set; }

}

public partial class SurveyUserResponseResponseModel : SurveyUserResponseRequestModel
{
    public TemplateQuestionResponseModel Question { get; set; } = null!;
    public string? CatalogName { get; set; } // Assuming this is a property in TemplateQuestion
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
    
}

public partial class SurveyUserResponsePageResponseModel
{
    public IEnumerable<SurveyUserResponseResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}

public partial class SurveyUserResponsePivotResponseModel
{
    public int SurveyUserId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public int FilialId { get; set; }
    public string FilialName { get; set; }
    public int QuestionId { get; set; }
    public string QuestionName { get; set; }
    public string Response { get; set; }
}