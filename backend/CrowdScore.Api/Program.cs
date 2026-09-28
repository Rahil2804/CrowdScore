using System.Text.Json.Serialization;
using CrowdScore.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string frontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];
var connectionString = builder.Configuration.GetConnectionString("CrowdScore")
    ?? throw new InvalidOperationException(
        "The connection string 'ConnectionStrings:CrowdScore' is not configured.");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<CrowdScoreDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .WithMethods("GET")
            .WithHeaders("Accept");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() && !EF.IsDesignTime)
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CrowdScoreDbContext>();
    await DevelopmentDataSeeder.SeedAsync(dbContext);
}

app.UseRouting();
app.UseCors(frontendCorsPolicy);
app.MapControllers();

app.Run();
