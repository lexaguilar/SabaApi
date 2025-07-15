namespace Saba.Domain.ViewModels;

public class RoleRequestModel
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Active { get; set; }
    public int UserId { get; set; }
}

public class RoleResponseModel : RoleRequestModel
{
    public DateTime? CreatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
    public IEnumerable<RoleResourceResponseModel> RoleResources { get; set; } = new List<RoleResourceResponseModel>();
}

public class RolePageResponseModel
{
    public int TotalCount { get; set; }
    public IEnumerable<RoleResponseModel> Items { get; set; }
}

public class RoleResourceResponseModel
{
    public string ResourceKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ParentResourceKey { get; set; } = string.Empty;
}

public class ResourcesRequestModel
{
    public string[] ResourcesKey { get; set; } = Array.Empty<string>();
}