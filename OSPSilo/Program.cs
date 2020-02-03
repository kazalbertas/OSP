using GrainImplementations.Operators;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserGrainImplementations;
using OSPTopologyManager;
using OSPJobManager;
using GrainInterfaces.Operators;

namespace OSPSilo
{
    class Program
    {
        public static int Main(string[] args)
        {

            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "GrainStreamProcessing";
                })
                .ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(TestSourceA).Assembly)
                .AddApplicationPart(typeof(Source<object>).Assembly)
                .AddApplicationPart(typeof(Job).Assembly)
                .AddApplicationPart(typeof(ISource).Assembly)
                .WithReferences()
                )
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("SMSProvider")
                .AddMemoryGrainStorage("PubSubStore");

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }


    }
}
