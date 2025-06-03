using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class SurveyUserRequestModel
{
    public int Id { get; set; } 
    public int SurveyId { get; set; } 
    public int UserId { get; set; } 
    public int FilialId { get; set; } 
    public int SurveyUserStateId { get; set; } 
    public string? Observation { get; set; } 
    
}

public partial class SurveyUserResponseModel : SurveyUserRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
    public string? SurveyName { get; set; }
    public string? FilialName { get; set; }
    public string? UserName { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalResponses { get; set; }
}

public partial class SurveyUserPageResponseModel
{
    public IEnumerable<SurveyUserResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
