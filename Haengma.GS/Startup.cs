using Haengma.Core;
using Haengma.Core.Logics;
using Haengma.Core.Logics.Games;
using Haengma.Core.Logics.Lobby;
using Haengma.Core.Models;
using Haengma.Core.Persistence;
using Haengma.Core.Services;
using Haengma.GS.Actions;
using Haengma.GS.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using shortid.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

            services.AddSignalR();

            services.AddDbContext<HaengmaContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    o => o.MigrationsAssembly("Haengma.GS")
                );
            });

            services.AddScoped<IIdGenerator<string>>(provider => new ShortIdGenerator(new GenerationOptions()));
            services.AddScoped<ITransactionFactory>(provider => new TransactionFactory(() =>
            {
                return provider.GetRequiredService<HaengmaContext>();
            }));

            services.AddScoped(provider =>
            {
                var idGenerator = provider.GetRequiredService<IIdGenerator<string>>();
                var transactions = provider.GetRequiredService<ITransactionFactory>();
                var logger = provider.GetRequiredService<ILogger<ServiceContext>>();
                var logics = provider.GetRequiredService<LogicContext>();
                
                return new ServiceContext(
                    transactions,
                    logics,
                    logger,
                    idGenerator
                );
            });

            services.AddSingleton(provider =>
            {
                var lobbyState = new LobbyState(
                    new HashSet<UserId>(),
                    new ConcurrentDictionary<UserId, OpenGameState>(),
                    new ConcurrentDictionary<GameId, Game>()
                );

                var idGenerator = provider.GetRequiredService<IIdGenerator<string>>();
                var hub = provider.GetRequiredService<IHubContext<GameHub, IGameClient>>();
                var gameNotifier = new GameNotifier(hub);
                var lobbyNotifier = new LobbyNotifier(hub);
                
                return new LogicContext(
                    lobbyState,
                    new ConcurrentDictionary<GameId, GameState>(),
                    lobbyNotifier,
                    gameNotifier,
                    idGenerator,
                    new Random()
                );
            });

            services.AddScoped(provider =>
            {
                var services = provider.GetRequiredService<ServiceContext>();
                return new ActionContext(services);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Haengma.GS", Version = "v1" });
            });

            services.AddCors(o => o.AddPolicy("HaengmaPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));
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
                        NoSuchEntityException => (HttpStatusCode.NotFound, exception.Message),
                        _ => (HttpStatusCode.InternalServerError, "Oops")
                    };

                    context.Response.StatusCode = (int)statusCode;
                    await context.Response.WriteAsJsonAsync(new { error = message });
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("HaengmaPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/hub/game");
            });
        }
    }
}
