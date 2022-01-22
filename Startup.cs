using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTNApiTalleres.Data;
using UTNApiTalleres.Data.Repositorio;
using UTNApiTalleres.Data.Repositorio.Interfaz;
using WebApiTalleres.Models;

namespace UTNApiTalleres
{
    public class Startup
    {
        public Startup//(IWebHostEnvironment env)
                      (IConfiguration configuration)
        {

           /* var builder = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                           .AddEnvironmentVariables();
           */

            //Configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var cadena = Configuration.GetConnectionString("PostgreSQLConnection");

            Console.WriteLine(cadena);

            var postgresqSQLConnectionConfiguration = new PostgresqlConfiguration(Configuration.GetConnectionString("PostgreSQLConnection"));
            
            services.AddSingleton(postgresqSQLConnectionConfiguration);

            services.AddScoped<IAseguradoraDao, AseguradoraDao>();
            services.AddScoped<IClienteDao, ClienteDao>();
            services.AddScoped<IPersonaDao, PersonaDao>();
            services.AddScoped<ITallerDao, TallerDao>();



            //////////////////////////////////////
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

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HerokuApiDemo v1"));
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
