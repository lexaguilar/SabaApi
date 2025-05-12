namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientsServices _clientsServices;

    public ClientsController(IClientsServices clientsServices)
    {
        _clientsServices = clientsServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, clients, errorMsg) = await _clientsServices.GetAll(skip, take, filters);
        if (!success)
            return Ok(Array.Empty<ClientResponseModel>());

        return Ok(new {
            items = clients,
            totalCount = clients?.Count() ?? 0
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, client, errorMsg) = await _clientsServices.GetById(id);
        if (!success)
            return NotFound(new { message = errorMsg });

        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClientRequestModel clientRequestModel)
    {
        var user = this.GetUser();

        clientRequestModel.UserId = user.Id;

        var (success, client, errorMsg) = await _clientsServices.Add(clientRequestModel);
        if (!success)
            return BadRequest(new { message = errorMsg });

        return Ok(client);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClientRequestModel clientRequestModel)
    {
        if (id != clientRequestModel.Id)
            return BadRequest(new { message = "Client ID mismatch." });

        var user = this.GetUser();
        
        clientRequestModel.UserId = user.Id;

        var (success, client, errorMsg) = await _clientsServices.Update(clientRequestModel);
        if (!success)
            return NotFound(new { message = errorMsg });

        return Ok(client);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {

        await _clientsServices.Disable(id);
        return Ok(new { message = "Client disabled." });

    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {

        await _clientsServices.Enable(id);
        return Ok(new { message = "Client enabled." });

    }
}