using Application.Extensions;
using Infraestructure.Extensions;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddInjectionInfraestructure();
        builder.Services.AddInjectionApplication();

        var app = builder.Build();

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
    }
}