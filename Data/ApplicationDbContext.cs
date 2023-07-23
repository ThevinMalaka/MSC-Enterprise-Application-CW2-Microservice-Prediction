using predictionService;
using Microsoft.EntityFrameworkCore;
using predictionService.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
    public DbSet<PredictionModel> Predictions { get; set; }
}


