using UnitConversionAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton(sp =>
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("ConversionConfigs.json", optional: false, reloadOnChange: true)
        .Build();

    return configuration.Get<UnitConversionConfiguration>() ?? new UnitConversionConfiguration();
});

builder.Services.AddSingleton<IUnitConversionService, UnitConversionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.MapControllers();

app.Run();
