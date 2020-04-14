using GrainInterfaces.Operators;
using Orleans;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSPJobManager
{
    public class JobManager
    {
        public async Task StartJob(TopologyManager tpm, IClusterClient client) 
        {
            await client.GetGrain<IJob>(Guid.NewGuid(), typeof(Job).FullName).StartJob(tpm);
            foreach (var stream in tpm.Operators) 
            {
                if (stream.OperatorType.GetInterfaces().Contains(typeof(IWindowJoin)))
                {
                    foreach (var g in stream.OperatorGUIDs)
                    {
                        var window = client.GetGrain<IWindowJoin>(g, stream.OperatorType.FullName);
                        await window.SetSources(stream.Prev.SelectMany(x => x.OperatorGUIDs).ToList(), stream.SourceBPrev.SelectMany(x => x.OperatorGUIDs).ToList());
                    }
                }

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
            var tasks = new List<Task>();
            foreach (var stream in tpm.Operators)
            {
                
                if (stream.OperatorType.GetInterfaces().Contains(typeof(ISource)))
                {
                    var s = client.GetGrain<ISource>(stream.OperatorGUIDs.First(), stream.OperatorType.FullName);
                    //tasks.Add(s.Start());
                    s.Start();
                }
            }
           // await Task.WhenAll(tasks);
        }
    }
}
