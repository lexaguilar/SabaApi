using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class FilialUserRequestModel
{
    public int UserId { get; set; } 
    public int FilialId { get; set; } 
    [NotMapped]
    public int UserId { get; set; }
}

public partial class FilialUserResponseModel:FilialUserRequestModel
{
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime? EditedAt { get; set; }
    public int? EditedByUserId { get; set; }
}

public partial class FilialUserPageResponseModel
{
    public IEnumerable<FilialUserResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
