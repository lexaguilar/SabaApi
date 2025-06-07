// using Microsoft.Extensions.Options;
// using Saba.Application.Helpers;
// using Saba.Domain.Models;
// using Saba.Domain.ViewModels;
// using Saba.Repository;

// namespace Saba.Application.Services;

// public interface IFilesServices
// {
//     Task<(bool success, bool fileUploaded, string? message)> Add(FileRequestModel m);
//     Task<(bool success, bool fileDeleted, string? message)> Remove(int id);
//     Task<(bool success, FileResponseModel? role, string? message)> GetById(int id);
//     Task<(bool success, FilePageResponseModel roles, string? message)> GetAll(int page, int pageSize, Dictionary<string, string>? filters = null);
// }

// public class FilesServices : IFilesServices
// {
//     private readonly ISurveyUserResponsesServices _surveyUserResponsesServices
//     private IWebHostEnvironment _hostingEnvironment;

//     public FilesServices(IWebHostEnvironment hostingEnvironment, ISurveyUserResponsesServices surveyUserResponsesServices)
//     {
//         _hostingEnvironment = hostingEnvironment;
//         _surveyUserResponsesServices = surveyUserResponsesServices;
//     }


//     public async Task<(bool success, bool fileUploaded, string? message)> Add(FileRequestModel m)
//     {
//         if (m.File == null || m.File.Length == 0)
//             return (false, false, "Archivo no proporcionado.");

//         string uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
//         if (m.File.Length > 0)
//         {
//             string filePath = System.IO.Path.Combine(uploads, m.Id.ToString() + m.File.FileName);

//             if (!Directory.Exists(uploads))
//                 Directory.CreateDirectory(uploads);

//             if (System.IO.File.Exists(filePath))
//                 System.IO.File.Delete(filePath);

//             using (Stream fileStream = new FileStream(filePath, FileMode.Create))
//             {
//                 await m.File.CopyToAsync(fileStream);
//             }
//         }

//         var surveyUserResponse = await _surveyUserResponsesServices.GetById(m.Id);
//         if (!surveyUserResponse.success)
//             return (false, false, "No se encontró la respuesta del usuario.");

//         surveyUserResponse.surveyUserResponse.FileNameUploaded = m.File.FileName;
//         var updateResponse = await _surveyUserResponsesServices.Update(surveyUserResponse.surveyUserResponse);

//         return (true, true, null);
//     }

//     public async Task<(bool success, bool fileDeleted, string? message)> Remove(int id)
//     {
//         var response = await _surveyUserResponsesServices.GetById(id);
//         if (!response.success)
//             return (false, false, "No se encontró la respuesta del usuario.");

//         if (response.surveyUserResponse.FileNameUploaded == null)
//             return (false, false, "No se encontró el archivo.");

//         string uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
//         if (Directory.Exists(uploads))
//         {
//             string filePath = System.IO.Path.Combine(uploads, id.ToString() + response.surveyUserResponse.FileNameUploaded);
//             if (System.IO.File.Exists(filePath))
//             {
//                 System.IO.File.Delete(filePath);
//                 return (true, true, null);
//             }
//             return (true, true, null);
//         }
//         return (false, false, "El archivo no existe.");
//     }

//     public async Task<(bool success, FileResponseModel? role, string? message)> GetById(int id)
//     {
//         var response = await _surveyUserResponsesServices.GetById(id);
//         if (!response.success)
//             return (false, null, "No se encontró la respuesta del usuario.");

//         if (response.surveyUserResponse.FileNameUploaded == null)
//             return (false, null, "No se encontró el archivo.");

//         string uploads = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
//         string filePath = System.IO.Path.Combine(uploads, id.ToString() + response.surveyUserResponse.FileNameUploaded);

//         if (!System.IO.File.Exists(filePath))
//             return (false, null, "El archivo no existe.");

//         var fileResponse = new FileResponseModel
//         {
//             Id = id,
//             FileName = response.surveyUserResponse.FileNameUploaded,
//             FilePath = filePath
//         };

//         return (true, fileResponse, null);
//     }

//     public async Task<(bool success, FilePageResponseModel roles, string? message)> GetAll(int page, int pageSize, Dictionary<string, string>? filters = null)
//     {
//         var response = await _surveyUserResponsesServices.GetAll(page, pageSize, filters);
//         if (!response.success)
//             return (false, new FilePageResponseModel { Items = Array.Empty<FileResponseModel>(), TotalCount = 0 }, response.message);

//         var fileResponses = response.roles.Items.Select(surveyUserResponse => new FileResponseModel
//         {
//             Id = surveyUserResponse.Id,
//             FileName = surveyUserResponse.FileNameUploaded ?? string.Empty,
//             FilePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "uploads", surveyUserResponse.Id.ToString() + surveyUserResponse.FileNameUploaded)
//         });

//         return (true, new FilePageResponseModel { Items = fileResponses, TotalCount = response.roles.TotalCount }, null);
//     }
// }