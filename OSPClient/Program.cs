using CoreOSP.Delegators;
using GrainImplementations.Operators;
using GrainInterfaces.Operators;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OSPJobManager;
using OSPTopologyManager;
using System;
using System.Threading.Tasks;
using UserGrainImplementations;

namespace OSPClient
{
    class Program
    {
        static int Main(string[] args)
        {

            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await Test1(client);
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
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

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task Test1(IClusterClient client)
        {
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var guid = new Guid();
            var photoStream = streamProvider.GetStream<string>(guid, "Photo");
            var tagStream = streamProvider.GetStream<string>(guid, "Tag");
            var gpsStream = streamProvider.GetStream<string>(guid, "GPS");

            TopologyConfiguration conf = new TopologyConfiguration();
            conf.Delegator = typeof(RoundRobinDelegator);
            TopologyManager tpm = new TopologyManager(conf);

            var a = tpm.AddSource(typeof(TestSourceA));
            a.Sink(typeof(GenericPrintSink));

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(tpm, client);
            await DataDriver.Run(photoStream, tagStream, gpsStream, 1600, 0);
        }
    }
}
