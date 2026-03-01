using ManagementPort;
using Microsoft.AspNetCore.Mvc;

public class Program
{
  private static string CurrentApiKey { get; set; } = "";
  private static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    var app = builder.Build();

    var summaries = new[]
    {
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    app.Use(async (context, next) =>
    {
      if (string.IsNullOrEmpty(CurrentApiKey) || context.Request.Headers["X-API-KEY"] != CurrentApiKey)
      {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
      }

      else
        await next.Invoke(context);
    });

    app.MapGet("/weatherforecast", (HttpContext context) =>
    {
      var forecast = Enumerable.Range(1, 5).Select(index =>
          new WeatherForecast
          (
              DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
              Random.Shared.Next(-20, 55),
              summaries[Random.Shared.Next(summaries.Length)]
          ))
          .ToArray();
      return forecast;
    })
    .WithName("GetWeatherForecast");

    var managementPort = ManagementPortBuilder.CreateManagementWebApplication(new ManagementPortOptions
    {
      SocketPath = builder.Configuration.GetSection("ManagementPort").GetSection("SocketPath").Value!,
      UnixFileMode = UnixFileMode.UserRead | UnixFileMode.UserWrite,
    });

    managementPort.MapPost("/management", ([FromBody] string apiKey) =>
    {
      CurrentApiKey = apiKey;
      return "OK";
    })
    .WithName("SetApiKey");

    managementPort.Use(async (context, next) =>
    {
      await next.Invoke(context);
    });

    await app.RunWithManagementPortAsync(managementPort);
  }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
