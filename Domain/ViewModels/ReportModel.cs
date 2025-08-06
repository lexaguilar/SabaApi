using System.ComponentModel.DataAnnotations.Schema;

namespace Saba.Domain.ViewModels;
// [
//   {
//     "FilialName": "Farmacia Saba Plaza Real (C.Masaya)",
//     "Supervisor": "Alejandro Aguilar",
//     "StartDate": "2025-06-20T00:49:39.680",
//     "EndDate": "2025-06-20T01:03:13.417",
//     "AdministratorNameFilial": "Juan Perez",
//     "OwnerFilial": "Maria Lopez",
//     "Pregunta": "Limpieza Externa Local",
//     "QuestionTypeId": 4,
//     "Response": "",
//     "Comment":"",
//     "PreguntaId":0,
//     "ParentId":0
//   }
// ]
public partial class ReportResponseModel
{
    public int Id { get; set; }
    public string FilialName { get; set; } = null!;
    public string Supervisor { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal FilialLatitude { get; set; }
    public decimal FilialLongitude { get; set; }
    public decimal Distance { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string AdministratorNameFilial { get; set; } = null!;
    public string OwnerFilial { get; set; } = null!;

    public IEnumerable<ReportQuestionModel> Questions { get; set; }
}

public partial class ReportQuestionModel
{

    public string Pregunta { get; set; } = null!;
    public string PreguntaPadre { get; set; } = null!;
    public int QuestionTypeId { get; set; }
    public string Response { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public int? ParentId { get; set; }
    public IEnumerable<ReportQuestionFilesModel> Files { get; set; }
}


public partial class ReportQuestionFilesModel
{

    public string FileNameUploaded { get; set; } = null!;
    public int   SurveyUserResponseId { get; set; }
}

