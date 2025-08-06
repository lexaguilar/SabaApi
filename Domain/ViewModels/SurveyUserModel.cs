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
    public int UserEditId { get; set; } 
    
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
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? FilialLatitude { get; set; }
    public decimal? FilialLongitude { get; set; }
    public int Evaluation { get; set; }
    public decimal? Distance { get; set; }
    public string? AdministratorNameFilial { get; set; }
    public string? OwnerFilial { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}

public partial class SurveyUserPageResponseModel
{
    public IEnumerable<SurveyUserResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}

public partial class FinishSurveyUserRequestModel
{
    public int Id { get; set; }
    public string AdministratorNameFilial { get; set; } = string.Empty;
    public string OwnerFilial { get; set; } = string.Empty;
}

public partial class SurveyUserIssuesResponseModel
{
    public int FilialId { get; set; }
    public string FilialName { get; set; }
    public int Issues { get; set; }

}

public class SurveyUserResponseIssuesResponseModel
{
    public int SurveyId { get; set; }
    public int SurveyUserId { get; set; }
    public int FilialId { get; set; }
    public string FilialName { get; set; }
    public int QuestionId { get; set; }
    public string Question { get; set; }
    public string Response { get; set; } 
    public string Comment { get; set; } 
    public string Username { get; set; } 
    public DateTime? CompletedAt { get; set; }
    
    
}