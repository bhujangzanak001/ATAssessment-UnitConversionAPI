using UnitConversionAPI;

// Entry point for the API. This bootstraps the HTTP pipeline,
// registers the conversion service, and loads the runtime configuration from disk.
var builder = WebApplication.CreateBuilder(args);

// Register the MVC pipeline so controllers can serve HTTP requests.
builder.Services.AddControllers();

// Expose OpenAPI metadata for local API documentation and tooling support.
builder.Services.AddOpenApi();

// Load the conversion catalog from the JSON file and inject it as a singleton.
// This keeps the configuration centralised and makes it easy to update units/categories at runtime.
builder.Services.AddSingleton(sp =>
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("ConversionConfigs.json", optional: false, reloadOnChange: true)
        .Build();

    return configuration.Get<UnitConversionConfiguration>() ?? new UnitConversionConfiguration();
});

// Register the service that owns conversion logic and config persistence.
builder.Services.AddSingleton<IUnitConversionService, UnitConversionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Serve the generated OpenAPI schema for local development tools.
    app.MapOpenApi();

    // Serve the Swagger UI page from wwwroot/swagger/index.html.
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

// Expose HTTP endpoints from the controller layer.
app.MapControllers();

// Run the application.
app.Run();
