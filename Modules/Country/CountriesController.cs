namespace Saba.Infrastructure.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saba.Application.Extensions;
using Saba.Application.Services;
using Saba.Domain.ViewModels;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ICountriesServices _countryServices;
    public CountriesController(ICountriesServices countryServices)
    {
        _countryServices = countryServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        var (success, countrys, message) = await _countryServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<CountryResponseModel>());
        return Ok(countrys);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll(int skip, int take, [FromQuery]Dictionary<string, string> filters)
    {
        filters ??= new Dictionary<string, string>();
        filters.TryAdd("all-items", "true");

        var (success, countries, message) = await _countryServices.GetAll(skip, take, filters);
        if (!success) return Ok(Array.Empty<CountryResponseModel>());
        return Ok(countries.Items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (success, country, message) = await _countryServices.GetById(id);
        if (!success) return NotFound(new { message });
        return Ok(country);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CountryRequestModel model)
    {

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, country, message) = await _countryServices.Add(model);
        if (!success) return BadRequest(new { message });
        return Ok(country);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CountryRequestModel model)
    {
        if (id != model.Id) return BadRequest(new { message = "Id no coincide" });

        var user = this.GetUser();
        model.UserId = user.Id;

        var (success, country, message) = await _countryServices.Update(model);
        if (!success) return BadRequest(new { message });
        return Ok(country);
    }

    [HttpGet("{id}/disable")]
    public async Task<IActionResult> Disable(int id)
    {
        var (success, country, message) = await _countryServices.Disable(id);
        if (!success) return BadRequest(new { message });
        return Ok(country);
    }

    [HttpGet("{id}/enable")]
    public async Task<IActionResult> Enable(int id)
    {
        var (success, country, message) = await _countryServices.Enable(id);
        if (!success) return BadRequest(new { message });
        return Ok(country);
    }
}
