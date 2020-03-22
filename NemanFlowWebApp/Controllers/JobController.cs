using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using OSPJobManager;
using OSPTopologyManager;

namespace NemanFlowWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {

        IClusterClient _client;

        public JobController(IClusterClient client)
        {
            _client = client;
        }


        public class JobParameters 
        {
            public int OutputStreamCount { get; set; }
            public int Paralellism { get; set; }

        }
        [HttpPost,Route("/startjob")]
        public async void StartJob([FromBody] JobParameters parameters)
        {
            TopologyConfiguration conf = new TopologyConfiguration();
            conf.ProcessingType = CoreOSP.ProcessingType.SynchronizeEach;
            conf.TimeCharacteristic = CoreOSP.TimePolicy.EventTime;
            TopologyManager tpm = new TopologyManager(conf);

            var a = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.SourceAllocation), parameters.OutputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var tsource = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.TerminationSource), parameters.OutputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var i = tpm.AddSource(typeof(UserGrainImplementations.TestKafka2.SourceIntake), parameters.OutputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            var joined = i.WindowJoin(typeof(UserGrainImplementations.TestKafka2.AIJoin), a, parameters.Paralellism, parameters.OutputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);
            joined.AddInput(tsource);
            joined.Sink(typeof(UserGrainImplementations.TestKafka2.AISinkTest5), parameters.Paralellism, parameters.OutputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(tpm, _client);
        }
    }
}
