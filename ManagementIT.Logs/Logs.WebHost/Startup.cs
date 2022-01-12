using AutoMapper;
using Logs.Core.Abstractions;
using Logs.DataAccess.Data;
using Logs.WebHost.AutoMapper;
using Logs.WebHost.LogConsumer;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logs.WebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Masstransit

            var massTransitSection = Configuration.GetSection("MassTransit");
            var url = massTransitSection.GetValue<string>("Url");
            var host = massTransitSection.GetValue<string>("Host");
            var userName = massTransitSection.GetValue<string>("UserName");
            var password = massTransitSection.GetValue<string>("Password");

            services.AddMassTransit(x =>
            {
                x.AddBus(busFactory =>
                {
                    var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        cfg.Host($"rabbitmq://{url}{host}", configurator =>
                        {
                            configurator.Username(userName);
                            configurator.Password(password);
                        });
                        cfg.ConfigureEndpoints(busFactory, KebabCaseEndpointNameFormatter.Instance);
                        cfg.UseJsonSerializer();
                        cfg.UseHealthCheck(busFactory);
                    });
                    return bus;
                });

                x.AddConsumer<CreateLogConsumer>(typeof(CreateLogConsumerDefinition));
                x.AddConsumer<AllLogsConsumer>(typeof(AllLogsConsumerDefinition));
                x.AddConsumer<DeleteLogByIdConsumer>(typeof(DeleteLogByIdConsumerDefinition));
                x.AddConsumer<DeleteRangeLogsConsumer>(typeof(DeleteRangeLogsConsumerDefinition));
                x.AddConsumer<DeleteSelectedLogsConsumer>();
            });

            services.AddMassTransitHostedService();

            #endregion

            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddTransient<IMongoService, LogService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Logs.WebHost", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Logs.WebHost v1"));
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
