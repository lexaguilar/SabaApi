using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;

namespace Saba.Repository;

public interface IReportRepository
{
    Task<ReportResponseModel> GetAllAsync(int surveyUserId);
  
}

public class ReportRepository : IReportRepository
{
    private readonly SabaContext _context;

    public ReportRepository(SabaContext context)
    {
        _context = context;
    }

    public async Task<ReportResponseModel> GetAllAsync(int surveyUserId)
    {
        // var report =  from sur in _context.SurveyUserResponses
        //                 join su in _context.SurveyUsers on sur.SurveyUserId equals su.Id
        //                 join tq in _context.TemplateQuestions on sur.QuestionId equals tq.Id
        //                 join tq2 in _context.TemplateQuestions on tq.ParentId equals tq2.Id
        //                 join f in _context.Filials on su.FilialId equals f.Id
        //                 join u in _context.Users on su.UserId equals u.Id
        //              where su.Id == surveyUserId
        //              select new ReportResponseModel
        //              {
        //                  Id = sur.Id,
        //                  FilialName = f.Name,
        //                  Supervisor = u.Name + " " + u.LastName,
        //                  StartDate = su.StartDate ?? DateTime.MinValue,
        //                  EndDate = su.EndDate ?? DateTime.MinValue,
        //                  AdministratorNameFilial = su.AdministratorNameFilial ?? string.Empty,
        //                  OwnerFilial = su.OwnerFilial ?? string.Empty,
        //                  //  Pregunta = tq.Name,
        //                  //  PreguntaPadre = tq2.Name,
        //                  //  QuestionTypeId = tq.QuestionTypeId,
        //                  //  Response = sur.Response ?? string.Empty,
        //                  //  Comment = sur.Comment ?? string.Empty,
        //                  //  ParentId = tq.ParentId
        //                     Questions = _context.SurveyUserResponses
        //                         .Where(sur2 => sur2.SurveyUserId == su.Id && sur2.QuestionId == tq.Id)
        //                         .Select(sur2 => new ReportQuestionModel
        //                         {
        //                             Pregunta = tq.Name,
        //                             PreguntaPadre = tq2.Name,
        //                             QuestionTypeId = tq.QuestionTypeId,
        //                             Response = sur2.Response ?? string.Empty,
        //                             Comment = sur2.Comment ?? string.Empty,
        //                             ParentId = tq.ParentId,
        //                             Files = _context.SurveyUserResponseFiles
        //                                 .Where(surFile => surFile.SurveyUserResponseId == sur2.Id)
        //                                 .Select(surFile => new ReportQuestionFilesModel
        //                                 {
        //                                     FileNameUploaded = surFile.FileNameUploaded
        //                                 }).ToList()
        //                         }).ToList()
        //              };

        var report = await _context.SurveyUsers.
            Where(su => su.Id == surveyUserId)
            .Select(su => new ReportResponseModel
            {
                Id = su.Id,
                FilialName = su.Filial.Name,
                Supervisor = $"{su.User.Name} {su.User.LastName}",
                StartDate = su.StartDate ?? DateTime.MinValue,
                EndDate = su.EndDate ?? DateTime.MinValue,
                AdministratorNameFilial = su.AdministratorNameFilial ?? string.Empty,
                OwnerFilial = su.OwnerFilial ?? string.Empty,
                Questions = _context.SurveyUserResponses.Include(sur => sur.Question)
                .ThenInclude(q => q.Parent)
                    .Where(sur => sur.SurveyUserId == su.Id)
                    .Select(sur => new ReportQuestionModel
                    {
                        Pregunta = sur.Question.Name,
                        PreguntaPadre = sur.Question.ParentId != null ? sur.Question.Parent.Name : string.Empty,
                        QuestionTypeId = sur.Question.QuestionTypeId,
                        Response = sur.Response ?? string.Empty,
                        Comment = sur.Comment ?? string.Empty,
                        ParentId = sur.Question.ParentId,
                        Files = _context.SurveyUserResponseFiles
                            .Where(surFile => surFile.SurveyUserResponseId == sur.Id)
                            .Select(surFile => new ReportQuestionFilesModel
                            {
                                FileNameUploaded = surFile.FileNameUploaded,
                                SurveyUserResponseId = surFile.SurveyUserResponseId
                            }).ToList()
                    }).ToList()
            }).FirstOrDefaultAsync();

        return report;
    }
}
