using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class FilialUserRequestModel
{
    public int UserId { get; set; } 
    public int FilialId { get; set; } 
   
}

public partial class FilialUserResponseModel : FilialUserRequestModel
{
    
}

public partial class FilialUserPageResponseModel
{
    public IEnumerable<FilialUserResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
