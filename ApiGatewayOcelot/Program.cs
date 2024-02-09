
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using JwtAuthenticationManager;
using Ocelot.Provider.Eureka;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;


namespace ApiGatewayOcelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging();

            builder.Services.AddOcelot(builder.Configuration).AddEureka();
            builder.Services.AddServiceDiscovery(o => o.UseEureka());

            builder.Services.AddCustomJwtAuthentication();

            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();

            app.MapControllers();


            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseHealthChecks("/health");

            app.UseOcelot().Wait();

            app.Run();
        }
    }
}
