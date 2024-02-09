
using MassTransit;
using Microsoft.OpenApi.Models;
using OrderBLL;
using OrderBLL.Interfaces;
using OrderDAL;
using OrderDAL.Interfaces;
using JwtAuthenticationManager;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;


namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //Dependency injection
            builder.Services.AddSingleton<IOrderRepository,OrderRepository>();
            builder.Services.AddScoped<IOrderBLLService,OrderBLLService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExitTest", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token here."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            builder.Services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx,cfg) =>
                {
                    cfg.Host(new Uri("rabbitmq://rabbitmq-server"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });

            builder.Services.AddMassTransitHostedService();

            builder.Services.AddCustomJwtAuthentication();

            //Eureka Discovery config
            //builder.Services.AddDiscoveryClient();
            builder.Services.AddServiceDiscovery(o => o.UseEureka());
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/health");


            app.MapControllers();

            app.Run();
        }
    }
}
