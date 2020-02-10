using GrainInterfaces.Operators;
using Orleans;
using OSPTopologyManager;
using System;
using System.Linq;

namespace OSPJobManager
{
    public class JobManager
    {
        public async System.Threading.Tasks.Task StartJob(TopologyManager tpm, IClusterClient client) 
        {
            await client.GetGrain<IJob>(Guid.NewGuid(), typeof(Job).FullName).StartJob(tpm);
            foreach (var stream in tpm.Operators) 
            {
                if (stream.OperatorType.GetInterfaces().Contains(typeof(ISource)))
                {
                    var s = client.GetGrain<ISource>(stream.OperatorGUIDs.First(), stream.OperatorType.FullName);
                    await s.InitSource(tpm.Conf.TimeCharacteristic);
                }
                else 
                {
                    foreach (var g in stream.OperatorGUIDs)
                    {
                        var s = client.GetGrain<IOperator>(g, stream.OperatorType.FullName);
                        await s.GetSubscribedStreams();
                    }
                }
            }

            foreach (var stream in tpm.Operators)
            {
                if (stream.OperatorType.GetInterfaces().Contains(typeof(ISource)))
                {
                    var s = client.GetGrain<ISource>(stream.OperatorGUIDs.First(), stream.OperatorType.FullName);
                    await s.Start();
                }
            }
            
        }
    }
}
