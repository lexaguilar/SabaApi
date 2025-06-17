using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Saba.Application.Helpers;
using Saba.Domain.Models;
using Saba.Domain.ViewModels;
using Saba.Repository;

namespace Saba.Application.Services;

public interface IFilesServices
{
    Task<(bool success, bool fileUploaded, string? message)> Add(FileRequestModel m);
    Task<(bool success, bool fileDeleted, string? message)> Remove(int id);
    Task<(bool success, FileStreamResult file, string? message)> GetById(int id);
    Task<(bool success, FilePageResponseModel filesInfo, string? message)> GetAll(int surveyUserResponseId);
}

public class FilesServices : IFilesServices
{
    private readonly ISurveyUserResponseFilesServices _surveyUserResponseFilesServices;
    private readonly ISurveyUserResponsesServices _surveyUserResponsesServices;
    private IWebHostEnvironment _hostingEnvironment;

    public FilesServices(IWebHostEnvironment hostingEnvironment, ISurveyUserResponseFilesServices surveyUserResponseFilesServices, ISurveyUserResponsesServices surveyUserResponsesServices)
    {
        _hostingEnvironment = hostingEnvironment;
        _surveyUserResponseFilesServices = surveyUserResponseFilesServices;
        _surveyUserResponsesServices = surveyUserResponsesServices;
    }


    public async Task<(bool success, bool fileUploaded, string? message)> Add(FileRequestModel m)
    {
        if (m.File == null || m.File.Length == 0)
            return (false, false, "Archivo no proporcionado.");



        string uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads", m.Id.ToString());
        if (m.File.Length > 0)
        {
            string filePath = System.IO.Path.Combine(uploads, m.File.FileName);

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await m.File.CopyToAsync(fileStream);
            }
        }

        var surveyUserResponseFiles = new SurveyUserResponseFileRequestModel
        {
            Id = 0,
            FileNameUploaded = m.File.FileName,
            SurveyUserResponseId = m.Id,
            ContentType = m.File.ContentType
        };

        await _surveyUserResponseFilesServices.Add(surveyUserResponseFiles);
       
        return (true, true, null);
    }

    public async Task<(bool success, bool fileDeleted, string? message)> Remove(int id)
    {
        var response = await _surveyUserResponseFilesServices.GetById(id);
        if (!response.success)
            return (false, false, "No se encontró la respuesta del usuario.");

        if (response.surveyUserResponseFile.FileNameUploaded == null)
            return (false, false, "No se encontró el archivo.");

        string uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
        if (Directory.Exists(uploads))
        {
            var surveyUserResponseId = response.surveyUserResponseFile.SurveyUserResponseId;
            string filePath = System.IO.Path.Combine(uploads, surveyUserResponseId.ToString(), response.surveyUserResponseFile.FileNameUploaded);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _surveyUserResponseFilesServices.Remove(id);

        return (true, false, "Archivo eliminado correctamente.");
    }

    public async Task<(bool success, FileStreamResult file, string? message)> GetById(int id)
    {
        var response = await _surveyUserResponseFilesServices.GetById(id);
        if (!response.success)
            return (false, null, "No se encontró la respuesta del usuario.");

        if (response.surveyUserResponseFile.FileNameUploaded == null)
            return (false, null, "No se encontró el archivo.");

        var surveyUserResponseId = response.surveyUserResponseFile.SurveyUserResponseId;
        string uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
        string filePath = System.IO.Path.Combine(uploads, surveyUserResponseId.ToString(), response.surveyUserResponseFile.FileNameUploaded);

        if (!System.IO.File.Exists(filePath))
            return (false, null, "El archivo no existe.");

        var fileInfo = _hostingEnvironment.WebRootFileProvider.GetFileInfo(filePath);
        var fileResponse = new FileStreamResult(fileInfo.CreateReadStream(), response.surveyUserResponseFile.ContentType);

        return (true, fileResponse, null);
    }

    public async Task<(bool success, FilePageResponseModel filesInfo, string? message)> GetAll(int surveyUserResponseId)
    {
        var response = await _surveyUserResponseFilesServices.GetAll(0, 1000, new Dictionary<string, string> { { "surveyUserResponseId", surveyUserResponseId.ToString() } });
        if (!response.success)
            return (false, new FilePageResponseModel { Items = Array.Empty<FileResponseModel>(), TotalCount = 0 }, response.message);

        var fileResponses = response.surveyUserResponseFileResult.Items.Select(surveyUserResponse => new FileResponseModel
        {
            Id = surveyUserResponse.Id,
            FileName = surveyUserResponse.FileNameUploaded ?? string.Empty,
            FilePath = System.IO.Path.Combine("uploads", surveyUserResponseId.ToString(), surveyUserResponse.FileNameUploaded)
        });

        var surveyUserResponse = await _surveyUserResponsesServices.GetById(surveyUserResponseId);

        if (!surveyUserResponse.success)
            return (false, new FilePageResponseModel { Items = Array.Empty<FileResponseModel>(), TotalCount = 0 }, surveyUserResponse.message);

        if (surveyUserResponse.surveyUserResponse == null)
            return (false, new FilePageResponseModel { Items = Array.Empty<FileResponseModel>(), TotalCount = 0 }, "No se encontró la respuesta del usuario.");

        if (surveyUserResponse.surveyUserResponse.Question == null)
            return (false, new FilePageResponseModel { Items = Array.Empty<FileResponseModel>(), TotalCount = 0 }, "No se encontró la pregunta asociada a la respuesta del usuario.");

        if (surveyUserResponse.surveyUserResponse.Question.Name == null)
            return (false, new FilePageResponseModel { Items = Array.Empty<FileResponseModel>(), TotalCount = 0 }, "No se encontró el nombre de la pregunta asociada a la respuesta del usuario.");

        var questionName = surveyUserResponse.surveyUserResponse.Question.Name;

        return (true, new FilePageResponseModel { Items = fileResponses, TotalCount = response.surveyUserResponseFileResult.TotalCount, QuestionName = questionName }, null);
    }

}