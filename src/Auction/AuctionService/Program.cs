using Application.Extensions;
using AuctionService.Filters;
using AuctionService.Middlewares;
using Infraestructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

builder.AddInjectionInfraestructure();
builder.Services.AddInjectionApplication(configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateActor = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    DbInitializer.InitDb(app.Services);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

app.Run();