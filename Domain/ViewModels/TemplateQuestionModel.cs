using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class TemplateQuestionRequestModel
{
    public int Id { get; set; } 
    public int TemplateId { get; set; } 
    public int QuestionTypeId { get; set; } 
    public string Name { get; set; } = null!;
    public bool? IsRequired { get; set; } 
    public int? ParentId { get; set; } 
    public int? CatalogNameId { get; set; } 
    public bool Active { get; set; } 
}

public partial class TemplateQuestionResponseModel : TemplateQuestionRequestModel
{
    public int Files { get; set; }
}

public partial class TemplateQuestionPageResponseModel
{
    public IEnumerable<TemplateQuestionResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
