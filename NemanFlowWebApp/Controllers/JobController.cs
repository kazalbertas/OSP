using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces.Operators;
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

        public static Dictionary<Guid,Type> storageOperatorGuids = new Dictionary<Guid,Type>();

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
            var storage = joined.Storage(typeof(UserGrainImplementations.TestKafka2.AIStorage),parameters.OutputStreamCount,CoreOSP.Partitioner.PartitionPolicy.Key);
            storageOperatorGuids.Add(storage.OperatorGUIDs.First(),storage.OperatorType);
            joined.Sink(typeof(UserGrainImplementations.TestKafka2.AISinkTest5), parameters.Paralellism, parameters.OutputStreamCount, CoreOSP.Partitioner.PartitionPolicy.Key);

            JobManager jmgr = new JobManager();
            await jmgr.StartJob(tpm, _client);
        }

        [HttpGet,Route("/getStorages")]
        public ActionResult GetStorages() 
        {
            return Ok(storageOperatorGuids.Keys);
        }

        [HttpPost, Route("/getFunctions")]
        public async Task<ActionResult> GetFunctions([FromBody] Guid storageId) 
        {
            var result = await _client.GetGrain<IStorage>(storageId, storageOperatorGuids[storageId].FullName).GetFunctions();
            return Ok(result);
        }
        
        public class RunParameters 
        {
            public Guid StorageId { get; set; }
            public string FunctionName { get; set; }
            public object FunctionParameters { get; set; }
        }
        [HttpPost,Route("/runFunc")]
        public async Task<ActionResult> RunFunctionAsync([FromBody] RunParameters parameters) 
        {
            var result = await _client.GetGrain<IStorage>(parameters.StorageId, storageOperatorGuids[parameters.StorageId].FullName).Select(parameters.FunctionName, parameters.FunctionParameters);
            return Ok(result);
        }
    }
}
