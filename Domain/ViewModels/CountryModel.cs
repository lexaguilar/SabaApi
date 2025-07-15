using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class CountryRequestModel
{
    public int Id { get; set; } 
    public string Name { get; set; } = null!;
    public bool Active { get; set; } 
    [NotMapped]
    public int UserId { get; set; }
}

public partial class CountryResponseModel:CountryRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class CountryPageResponseModel
{
    public IEnumerable<CountryResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
