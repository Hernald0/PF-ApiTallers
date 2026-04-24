using UTNApiTalleres.Application.Interfaces;
using UTNApiTalleres.Application.Services;
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
using UTNApiTalleres.Infrastructure.Repositories.Interface;
using UTNApiTalleres.Infrastructure.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace UTNApiTalleres
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {

            Console.WriteLine($"Environment: {env.EnvironmentName}");

            // Primero, intenta obtener el valor de `RAILWAY_ENVIRONMENT_NAME`
            var railwayEnvironment = Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT_NAME");

            // Si `RAILWAY_ENVIRONMENT_NAME` tiene un valor, úsalo. Si no, usa `env.EnvironmentName`.
            var environmentName = !string.IsNullOrEmpty(railwayEnvironment) ? railwayEnvironment : env.EnvironmentName;


            var builder = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                           .AddEnvironmentVariables();

       

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.LoginPath = "/api/auth/login";
                            options.AccessDeniedPath = "/api/auth/denied";
                            options.ExpireTimeSpan = TimeSpan.FromHours(8);
                            options.Cookie.Name = ".AspNetCore.Cookies";
                            options.Cookie.SameSite = SameSiteMode.Lax;// SameSiteMode.None;                           
                            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; //CookieSecurePolicy.Always; 
                            options.Cookie.HttpOnly = true;

                        });

            services.AddAuthorization();

            services.AddControllers();

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var connectionString = Configuration.GetConnectionString("PostgreSQLConnection");
            /*Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL")
                               ?? Configuration.GetConnectionString("PostgreSQLConnection");*/

         

            var postgresConfiguration = new PostgresqlConfiguration(connectionString);
            services.AddSingleton(postgresConfiguration);

            
            services.AddHttpContextAccessor();
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
            //services.AddScoped<IServRepDao, ServRepDao>();
            services.AddScoped<IVentaDao, VentaDao>();
            services.AddScoped<IOrdenDao, OrdenDao>();
            services.AddScoped<IUsuarioDao, UsuarioDao>();
            services.AddScoped<IRolDao, RolDao>();
            services.AddScoped<IAccesoDao, AccesoDao>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IRolAccesoRepository, RolAccesoRepository>();
            services.AddScoped<IRolAccesoPermisoRepository, RolAccesoPermisoRepository>();
            services.AddScoped<IServRepService, ServRepService>();
            services.AddScoped<IServRepRepository, ServRepRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            

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
                /*options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });*/
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200") // tu frontend
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors("CorsPolicy");
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UTNApiTalleres v1"));
            }

       

            // Comment or remove the line below if HTTPS redirection is causing issues in production
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
