using System.ComponentModel.DataAnnotations.Schema;
using Saba.Domain.Models;

namespace Saba.Domain.ViewModels;

public partial class GenericCatalogRequestModel
{
    public int Id { get; set; } 
    public int CatalogNameId { get; set; } 
    public string CatalogValue { get; set; } = null!;
    public bool Active { get; set; } 
    [NotMapped]
    public int UserId { get; set; }
}

public partial class GenericCatalogResponseModel:GenericCatalogRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class GenericCatalogPageResponseModel
{
    public IEnumerable<GenericCatalogResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
