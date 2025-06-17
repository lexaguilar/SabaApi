using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;

public partial class SurveyUserResponseFileRequestModel
{
    public int Id { get; set; } 
    public int SurveyUserResponseId { get; set; } 
    public string FileNameUploaded { get; set; } = null!;
    public string ContentType { get; set; } = null!;
   
}

public partial class SurveyUserResponseFileResponseModel:SurveyUserResponseFileRequestModel
{
   
}

public partial class SurveyUserResponseFilePageResponseModel
{
    public IEnumerable<SurveyUserResponseFileResponseModel> Items { get; set; }
    public int TotalCount { get; set; }
}
