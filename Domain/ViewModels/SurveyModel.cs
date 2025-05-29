using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class SurveyRequestModel
{
    public int Id { get; set; } 
    public string Name { get; set; } = null!;
    public int TemplateId { get; set; } 
    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; } 
    public bool ApplyAllUser { get; set; } 
    public int? MinGoal { get; set; } 
    public int? ExpectedGoal { get; set; } 
    public bool Active { get; set; } 
    
    [NotMapped]
    public int UserId { get; set; }
}

public partial class SurveyResponseModel:SurveyRequestModel
{
    public int SurveyStateId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class SurveyPageResponseModel
{
    public IEnumerable<SurveyResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
