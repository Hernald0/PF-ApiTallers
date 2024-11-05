using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using UTNApiTalleres.Data;
using UTNApiTalleres.Data.Repositorio;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                           .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var connectionString = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL")
                                   ?? Configuration.GetConnectionString("PostgreSQLConnection");

            var postgresConfiguration = new PostgresqlConfiguration(connectionString);
            services.AddSingleton(postgresConfiguration);

            services.AddScoped<IAseguradoraDao, AseguradoraDao>();
            services.AddScoped<IClienteDao, ClienteDao>();
            services.AddScoped<IPersonaDao, PersonaDao>();
            services.AddScoped<ITallerDao, TallerDao>();
            services.AddScoped<IGeneroDao, GeneroDao>();
            services.AddScoped<IEstadoCivilDao, EstadoCivilDao>();
            services.AddScoped<ITipoidentificadorDao, TipoidentificadorDao>();
            services.AddScoped<IPaisDao, PaisDao>();
            services.AddScoped<IConfiguracionDao, ConfiguracionDao>();
            services.AddScoped<ITurnoDao, TurnoDao>();
            services.AddScoped<IServRepDao, ServRepDao>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Swagger Proyecto Final API",
                        Description = "API Proyecto Final - HDC",
                        Version = "v1"
                    });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UTNApiTalleres v1"));
            }

            app.UseCors();

            // Comment or remove the line below if HTTPS redirection is causing issues in production
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
