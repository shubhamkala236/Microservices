
using MassTransit;
using ProductDetailBLL;
using ProductDetailBLL.Interfaces;
using ProductDetailDAL;
using ProductDetailDAL.Interfaces;
using JwtAuthenticationManager;
using Steeltoe.Discovery.Client;
using Microsoft.OpenApi.Models;
using Steeltoe.Discovery.Eureka;


namespace ProductDetailService
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
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
            builder.Services.AddScoped<IProductDetailBLLService, ProductDetailBLLService>();
            builder.Services.AddSingleton<IProductDetailRepository, ProductDetailRepository>();


            builder.Services.AddMassTransit(config =>
            {

                config.AddConsumer<DeleteProductConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    //cfg.Host("amqp://guest:guest@localhost:5672");
                    cfg.Host(new Uri("rabbitmq://rabbitmq-server"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    //receive endpoint
                    cfg.ReceiveEndpoint("deleted-product", c =>
                    {
                        c.ConfigureConsumer<DeleteProductConsumer>(ctx);
                    });
                });
            });

            builder.Services.AddMassTransitHostedService();

            //builder.Services.AddDiscoveryClient();
            builder.Services.AddServiceDiscovery(o => o.UseEureka());
            builder.Services.AddHealthChecks();

            builder.Services.AddCustomJwtAuthentication();

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
