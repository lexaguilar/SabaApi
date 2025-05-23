using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class TemplateRequestModel
{
    public int Id { get; set; } 
    public string? TemplateCode { get; set; } 
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 
    public bool Active { get; set; } 
    [NotMapped]
    public int UserId { get; set; }
}

public partial class TemplateResponseModel:TemplateRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class TemplatePageResponseModel
{
    public IEnumerable<TemplateResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
