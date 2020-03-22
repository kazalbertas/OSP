using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainImplementations.Operators;
using GrainInterfaces.Operators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OSPJobManager;
using UserGrainImplementations;

namespace NemanFlowWebApp
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
            services.AddControllers();
            services.AddSingleton(ConnectClient);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }

        private IClusterClient ConnectClient(IServiceProvider serviceProvider)
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "GrainStreamProcessing";
                })
                .ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(IJob).Assembly)
                .AddApplicationPart(typeof(TestSourceA).Assembly)
                .AddApplicationPart(typeof(Source<object>).Assembly)
                .AddApplicationPart(typeof(Job).Assembly)
                .AddApplicationPart(typeof(ISource).Assembly)
                .WithReferences()
                )
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("SMSProvider")
                .Build();

            client.Connect().Wait();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

    }
}
