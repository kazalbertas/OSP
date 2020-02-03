using GrainInterfaces.Operators;
using Orleans;
using OSPTopologyManager;
using System;
using System.Linq;

namespace OSPJobManager
{
    public class JobManager
    {
        public async System.Threading.Tasks.Task StartTestJobAsync(TopologyManager tpm, IClusterClient client) 
        {
            await client.GetGrain<IJob>(Guid.NewGuid(), typeof(Job).FullName).StartJob(tpm);
            foreach (var stream in tpm.Streams) 
            {
                var s = client.GetGrain<ISource>(stream.OperatorId.First(), stream.OperatorType.FullName);
                await s.InitSource();
                
            }
            foreach (var stream in tpm.Streams)
            {
                var s = client.GetGrain<ISource>(stream.OperatorId.First(), stream.OperatorType.FullName);
                await s.Start();

            }
            
        }
    }
}
