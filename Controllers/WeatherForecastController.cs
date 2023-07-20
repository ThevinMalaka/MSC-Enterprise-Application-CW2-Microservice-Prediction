using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace predictionService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ApplicationDbContext _context;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var rng = new Random();
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Id = Guid.NewGuid().ToString(),  // Generate a new GUID and convert to string
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();

        // Save forecasts to database
        _context.WeatherForecasts.AddRange(forecasts);
        _context.SaveChanges();

        return forecasts;
    }

    [HttpGet("AllData")]
    public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAllData()
    {
        var data = await _context.WeatherForecasts.Select(wf => wf.Summary).ToListAsync();
        return Ok(data);
    }
}

