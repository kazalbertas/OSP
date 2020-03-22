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
using UserGrainImplementations.TestKafka;

namespace OSPClient
{
    class Program
    {
        static int Main(string[] args)
        {
            int paralelism = int.Parse(args[0]);
            int outputStreamCount = int.Parse(args[1]);
            int program = int.Parse(args[2]);
            //int paralelism = 1;
            //int outputStreamCount = 1;
            //int program = 4;
            return RunMainAsync(paralelism, outputStreamCount,program).Result;
        }

        private static async Task<int> RunMainAsync(int paralelism, int outputStreamCount,int program)
        {
            try
            {

                using (var client = await ConnectClient())
                {
                    switch (program)
                    {
                        case 0:
                            await Test1(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka.AISinkTest1));
                            break;
                        case 1:
                            await Test1(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka.AISinkTest2));
                            break;
                        case 2:
                            await Test1(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka.AISinkTest3));
                            break;
                        case 3:
                            await Test1(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka.AISinkTest4));
                            break;
                        case 4:
                            await Test2(client,paralelism,outputStreamCount,typeof(UserGrainImplementations.TestKafka2.AISinkTest5));
                            break;
                        case 5:
                            await Test2(client, paralelism, outputStreamCount*2, typeof(UserGrainImplementations.TestKafka2.AISinkTest6));
                            break;
                        case 6:
                            await Test2(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka2.AISinkTest7));
                            break;
                        case 7:
                            await Test2(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka2.AISinkTest8));
                            break;
                        case 8:
                            await Test3(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka3.AISinkTest9));
                            break;
                        case 9:
                            await Test3(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka3.AISinkTest10));
                            break;
                        case 10:
                            await Test3(client, paralelism, outputStreamCount, typeof(UserGrainImplementations.TestKafka3.AISinkTest11));
                            break;
                        default:
                            break;
                    }
                    
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

        private static async Task Test1(IClusterClient client, int paralelism, int outputStreamCount, Type sinkType)
        {

            TopologyConfiguration conf = new TopologyConfiguration();
            conf.ProcessingType = CoreOSP.ProcessingType.SynchronizeEach;
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            TopologyManager tpm = new TopologyManager(conf);

            var a = tpm.AddSource(typeof(SourceAllocation), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            var tsource = tpm.AddSource(typeof(TerminationSource), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            var amapped = a.Map(typeof(AllocationMap), paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var i = tpm.AddSource(typeof(SourceIntake), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var joined = i.WindowJoin(typeof(AIJoin), amapped, paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            joined.AddInput(tsource);
            var filtered = joined.Filter(typeof(AIFilter), paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            filtered.Sink(sinkType, paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(tpm, client);
            Console.ReadLine();
        }

        private static async Task Test2(IClusterClient client, int paralelism, int outputStreamCount, Type sinkType)
        {

            TopologyConfiguration conf = new TopologyConfiguration();
            conf.ProcessingType = CoreOSP.ProcessingType.SynchronizeEach;
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            TopologyManager tpm = new TopologyManager(conf);

            var a = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.SourceAllocation), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var tsource = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.TerminationSource), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var i = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.SourceIntake), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var joined = i.WindowJoin(typeof(UserGrainImplementations.TestKafka2.AIJoin), a, paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            joined.AddInput(tsource);
            joined.Sink(sinkType, paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(tpm, client);
            Console.ReadLine();
        }

        private static async Task Test3(IClusterClient client, int paralelism, int outputStreamCount, Type sinkType)
        {

            TopologyConfiguration conf = new TopologyConfiguration();
            conf.ProcessingType = CoreOSP.ProcessingType.SynchronizeEach;
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            TopologyManager tpm = new TopologyManager(conf);

            var a = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.SourceAllocation), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var i = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.SourceIntake), outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var joined = i.WindowJoin(typeof(UserGrainImplementations.TestKafka2.AIJoin), a, paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            joined.Sink(sinkType, paralelism, outputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(tpm, client);
            Console.ReadLine();
        }
    }
}
