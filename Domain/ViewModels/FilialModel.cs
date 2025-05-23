using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class FilialRequestModel
{
    public int Id { get; set; } 
    public string InternalCode { get; set; } =null!;
    public string Name { get; set; } =null!;
    public string? Address { get; set; } 
    public string? Lat { get; set; } 
    public string? Lng { get; set; } 
    public bool Active { get; set; } 
    [NotMapped]
    public int UserId { get; set; }
}

public partial class FilialResponseModel:FilialRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class FilialPageResponseModel
{
    public IEnumerable<FilialResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
