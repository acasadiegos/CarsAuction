using Infraestructure.Extensions;
using Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInjectionInfraestructure(configuration);
builder.Services.AddInjectionApplication(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try{
    DbInitializer.InitDb(app.Services);
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}


app.Run();
