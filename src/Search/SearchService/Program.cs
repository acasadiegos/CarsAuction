using Application.Extensions;
using Infraestructure.Extensions;
using SearchService.Filters;
using SearchService.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelFormatExceptionFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Deshabilitar la respuesta automática de validación
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInjectionInfraestructure(builder.Configuration);
builder.Services.AddInjectionApplication();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        using var scope = app.Services.CreateScope();
        await DbInitializer.InitDb(configuration, scope);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

//Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
