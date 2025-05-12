namespace Saba.Domain.ViewModels;

public class RoleRequestModel
{
    public int Id { get; set; }
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
}
