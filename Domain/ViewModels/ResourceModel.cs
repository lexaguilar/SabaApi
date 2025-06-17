using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class ResourceRequestModel
{
    public string ResourceKey { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 
    public string? ParentResourceKey { get; set; } 
    [NotMapped]
    public int UserId { get; set; }
}

public partial class ResourceResponseModel:ResourceRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class ResourcePageResponseModel
{
    public IEnumerable<ResourceResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
