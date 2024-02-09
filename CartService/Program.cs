
using CartBLL;
using CartBLL.Interfaces;
using CartDAL;
using CartDAL.Interfaces;
using JwtAuthenticationManager;
using MassTransit;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace CartService
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
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExitTest", Version = "v1" });
                //c.OperationFilter<FileUploadOperationFilter>();
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

            builder.Services.AddSingleton<ICartRepository, CartRepository>();
            builder.Services.AddSingleton<ICartBLLService, CartBLLService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddMassTransit(config =>
            {

                config.AddConsumer<ProductDetailDeleteConsumer>();
                config.AddConsumer<OrderPlacedConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri("rabbitmq://rabbitmq-server"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    //receive endpoint
                    cfg.ReceiveEndpoint("deleted-product-details", c =>
                    {
                        c.ConfigureConsumer<ProductDetailDeleteConsumer>(ctx);
                    });
                    cfg.ReceiveEndpoint("order-placed-event", c =>
                    {
                        c.ConfigureConsumer<OrderPlacedConsumer>(ctx);
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
