using Haengma.Backend.Imperative.Exceptions;
using Haengma.Backend.Imperative.Persistance;
using Haengma.Backend.Imperative.Services;
using Haengma.GS.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System;
using System.Net;
using System.Text.Json.Serialization;

namespace Haengma.GS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddDbContext<HaengmaContext>(options => {
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    o => o.MigrationsAssembly("Haengma.GS")
                );
            });

            services.AddScoped<ITransactionFactory>(provider => {
                return new TransactionFactory<HaengmaContext>(() => provider.GetService<HaengmaContext>()!);
            });

            services.AddScoped(provider => {
                var transactions = provider.GetService<ITransactionFactory>();
                return new ServiceContext(transactions!);
            });
            services.AddScoped(provider => {
                var services = provider.GetService<ServiceContext>();
                return new ActionContext(services!);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Haengma.GS", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Haengma.GS v1"));
            }

            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandler.Error;

                    var (statusCode, message) = exception switch
                    {
                        ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                        ResourceNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                        _ => (HttpStatusCode.InternalServerError, "Oops")
                    };

                    context.Response.StatusCode = (int)statusCode;
                    await context.Response.WriteAsJsonAsync(new { error = message });
                });
            });

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
