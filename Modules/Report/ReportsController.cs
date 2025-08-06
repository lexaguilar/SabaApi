namespace Saba.Infrastructure.Controllers;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportsServices _surveyServices;
    private readonly IHttpClientFactory _httpClientFactory;

    public ReportsController(IReportsServices surveyServices, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _surveyServices = surveyServices;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var surveyUser = await _surveyServices.GetById(id);

        var html = GenerarHtmlEncuesta(surveyUser);

        return Content(html, "text/html");

        // // Construye la URL del endpoint
        // string baseUrl = "https://design.quickreport.net"; // Cambia esto por la URL base de tu aplicación
        // string url = $"{baseUrl}/Report/GeneratePDFFromJson";

        // var reportName = "UserSurvey";       

        // var requestBody = new
        // {
        //     ApiKey = "qX6+YYcK/yFrq30TES8pA2Oz7/L7Lhmv4chKkOOxz+Y=",
        //     ReportName = reportName,
        //     Data = JsonSerializer.Serialize(surveyUserResponses),
        //     Parameters = new {  } // Reemplaza con los parámetros reales
        // };

        // // Serializa el cuerpo a JSON
        // var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // var client = _httpClientFactory.CreateClient();

        // var response = await client.PostAsync(url, jsonContent);


        // if (response.IsSuccessStatusCode)
        // {

        //     var stream  = await response.Content.ReadAsStringAsync();
        //     // Lee la respuesta como JSON
        //     var result  = JsonSerializer.Deserialize<UrlResponse>(stream);
        //     var data = await client.GetByteArrayAsync(result.Url);
        //     return new FileStreamResult(new MemoryStream(data), "application/pdf");

        // }
        // else
        // {
        //     return BadRequest($"Error al obtener el reporte: {response.StatusCode} - {response.ReasonPhrase}");
        // }



    }
    
    private string GenerarHtmlEncuesta(ReportResponseModel survey)
{
    var sb = new StringBuilder();

    sb.AppendLine("<!DOCTYPE html>");
    sb.AppendLine("<html lang='es'><head>");
    sb.AppendLine("<meta charset='UTF-8'>");
    sb.AppendLine("<title>Resultado de Encuesta</title>");
    sb.AppendLine("<style>");
    sb.AppendLine(@"body { font-family: Arial; padding: 5px; }
        .pregunta { margin-bottom: 40px; page-break-inside: avoid; }
        img { max-width: 200px; max-height: 200px; margin-right: 10px; }
        hr { border-top: 1px dashed #aaa; }
        @media print {
            @page { size: A4; margin: 1cm; }
        }");
    sb.AppendLine("</style></head><body>");
    sb.AppendLine($@"<div>
        <div style='display: flex; justify-content: space-between;'>
            <img src='/img/logo.png' alt='Logo Saba' style='width: 150px; height: auto;' />
            <h4 style='text-align: center;font-size: 15px;'>FORMATO DE SUPERVISION DE FARMACIAS</h4>
            <img src='/img/logo.png' alt='Logo Saba' style='width: 150px; height: auto;' />
        </div>   
        <br />     
        <div style='display: flex; justify-content: space-between;'>
            <div style='width: 100%; margin-bottom: 0px; background-color: #a4a4a4'>
                <h4 style='text-align: center; background-color: #a4a4a4;font-size: 13px;margin-bottom: 5px;margin-top: 5px;'>DATOS GENERALES</h4>
            </div>
        </div>
        <br />     
        <div style='display: flex; justify-content: space-around;'>
            <div>
                <div style='color: gray; font-size: 13px;'>Nombre de la Farmacia</div> <div style='font-size: 14px;'>{survey.FilialName}</div><br />
                <div style='color: gray; font-size: 13px;'>Nombre del Administrador:</div> <div style='font-size: 14px;'>{survey.AdministratorNameFilial}</div><br />
                <div style='color: gray; font-size: 13px;'>Fecha/Hora Inicio:</div> <div style='font-size: 14px;'>{survey.StartDate:dd/MM/yyyy HH:mm:ss tt}</div><br />
            </div>
            <div>
                <div style='color: gray; font-size: 13px;'>Nombre del Supervisor:</div> <div style='font-size: 14px;'>{survey.Supervisor}</div><br />
                <div style='color: gray; font-size: 13px;'>Nombre del Encargado:</div> <div style='font-size: 14px;'>{survey.OwnerFilial}</div><br />
                <div style='color: gray; font-size: 13px;'>Fecha/Hora Finalización:</div> <div style='font-size: 14px;'>{survey.EndDate:dd/MM/yyyy HH:mm:ss tt}</div><br />
            </div>
        </div>
    </div>
    <div style='border: 1px solid #aaa; '></div>
        
    ");
    
    IEnumerable<ReportQuestionModel> preguntas = survey.Questions;

    var preguntasAgrupadas = preguntas
    .GroupBy(q => q.PreguntaPadre)
    .Select(g => new
    {
        PreguntaPadre = g.Key,
        PreguntasHijas = g.ToList()
    })
    .ToList();

        foreach (var grupo in preguntasAgrupadas.Where(g => !string.IsNullOrEmpty(g.PreguntaPadre)))
        {
            sb.AppendLine($"<table style='width: 100%;'><tr><th style='width: 400px;'><h3 style='text-align: start; margin-bottom:5px;margin-top:5px;'>{grupo.PreguntaPadre}</h3></th><td>SI</td><td>NO</td><td>Comentario</td></tr>");

            foreach (var p in grupo.PreguntasHijas)
            {
                sb.AppendLine($"<tr><td style='padding-left: 5px;'>{p.Pregunta}</td><td>{(p.Response.ToUpper() == "SI" ? "✅" : "")}</td><td>{(p.Response.ToUpper() == "NO" ? "❌" : "")}</td><td>{p.Comment}</td></tr>");
                sb.AppendLine("<tr><td colspan='4'><div style='display: flex; justify-content: space-around; flex-wrap: wrap;'>");
                foreach (var img in p.Files.Take(3))
                {
                    sb.AppendLine($"<img src='/uploads/{img.SurveyUserResponseId}/{img.FileNameUploaded}' alt='Imagen' />");
                }
                sb.AppendLine("</div></td></tr>");

            }
            sb.AppendLine("</table>");
        

    }


    sb.AppendLine("<div style='border: 1px solid #aaa; '></div>");
    sb.AppendLine("<div style='text-align: center; margin-top: 20px;'>Coordenadas de la Filial: " + survey.FilialLatitude + ", " + survey.FilialLongitude + "</div>");
    sb.AppendLine("<div style='text-align: center; margin-top: 20px;'>Coordenadas de la encuesta: " + survey.Latitude + ", " + survey.Longitude + "</div>");
    sb.AppendLine("<div style='text-align: center; margin-top: 20px;'>Distancia aproximada: " + survey.Distance + " km</div>");
    sb.AppendLine("<script>window.onload = () => window.print();</script>");
    sb.AppendLine("</body></html>");

    return sb.ToString();
}


}

internal class UrlResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}