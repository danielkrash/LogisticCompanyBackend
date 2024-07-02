using Carter;
using LogisticCompany.models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using LogisticCompany.config;
using LogisticCompany.data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    $" Host={Environment.GetEnvironmentVariable("HOST")};" +
    $" Database={Environment.GetEnvironmentVariable("DATABASE")};" +
    $" Username={Environment.GetEnvironmentVariable("USER")};" +
    $" Password={Environment.GetEnvironmentVariable("PASSWORD")};" +
    $" SearchPath={Environment.GetEnvironmentVariable("SEARCHPATH")};";

builder.Services.AddDbContext<LogisticCompanyContext>(options => { options.UseNpgsql(connectionString); }
);

builder.Services.Configure<IdentityOptions>(options => { options.User.RequireUniqueEmail = true; });

// Configure App Settings
AppConfig.ConfigureWebHost(builder.WebHost);
AppConfig.ConfigureServices(builder.Services);

var app = builder.Build();
app.UseCors("AllowNextJS");
app.MapGroup("auth").MapAllIdentityApi<ApplicationUser>().WithTags("auth");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapGet("/weatherforecast", Results<Ok<WeatherForecast[]>, UnauthorizedHttpResult> (int num) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
        .ToArray();
    return num == 1 ? TypedResults.Unauthorized() : TypedResults.Ok(forecast);
}).WithName("GetWeatherForecast").WithOpenApi().RequireAuthorization("admin_only");
app.MapCarter();
// Seed the role to database
await AppConfig.SeedDatabase(app);
app.Run();