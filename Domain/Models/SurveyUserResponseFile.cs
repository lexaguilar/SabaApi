using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class SurveyUserResponseFile
{
    public int Id { get; set; }

    public int SurveyUserResponseId { get; set; }

    public string FileNameUploaded { get; set; } = null!;

    public virtual SurveyUserResponse SurveyUserResponse { get; set; } = null!;
}
