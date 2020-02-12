using CoreOSP;
using GrainInterfaces.Operators;
using Orleans;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopologyManagerOSP.Operators;

namespace OSPJobManager
{

    public class Job : Grain, IJob
    {

        protected TopologyManager tpm { get; set; }

        public async Task StartJob(TopologyManager mgr) 
        {
            tpm = mgr;
            foreach (var s in mgr.Operators) 
            {
                var curr = s;
                foreach (var guid in curr.OperatorGUIDs)
                {
                    await GrainFactory.GetGrain<IOperator>(guid, curr.OperatorType.FullName).Init(this.GetPrimaryKey(), GetType(), s.Partitioner);
                }
            }
        }

        public Task<List<(Guid,List<int>)>> GetStreamsSubscribe(Guid guid, Type type)
        {
            List<(Guid, List<int>)> result = new List<(Guid, List<int>)>();
            foreach (var ds in tpm.Operators) 
            {

                if (ds.OperatorGUIDs.Contains(guid) && ds.OperatorType == type)
                {
                    foreach (var prev in ds.Prev) 
                    {
                        var chunkSize = (int)Math.Ceiling(prev.OutputStreamCount / (double)ds.Parallelism);
                        var index = ds.OperatorGUIDs.IndexOf(guid);
                        result.Add((prev.StreamGUID, Enumerable.Range(0, prev.OutputStreamCount).ToList().ChunkBy(chunkSize)[index]));
                    }
                }
            }
            if (type.GetInterfaces().Contains(typeof(IWindowJoin))) 
            {
                foreach (var ds in tpm.Operators)
                {
                    if (ds.OperatorGUIDs.Contains(guid) && ds.OperatorType == type)
                    {
                        foreach (var prev in ds.SourceBPrev)
                        {
                            var chunkSize = (int)Math.Ceiling(prev.OutputStreamCount / (double)ds.Parallelism);
                            var index = ds.OperatorGUIDs.IndexOf(guid);
                            result.Add((prev.StreamGUID, Enumerable.Range(0, prev.OutputStreamCount).ToList().ChunkBy(chunkSize)[index]));
                        }
                    }
                }
            }
            return Task.FromResult(result);
        }

        public Task<(Guid, List<int>)?> GetOutputStreams(Guid guid, Type type) 
        {
            (Guid, List<int>)? result = null;
            foreach (var ds in tpm.Operators)
            {

                if (ds.OperatorGUIDs.Contains(guid) && ds.OperatorType == type)
                {
                    result = (ds.StreamGUID, Enumerable.Range(0, ds.OutputStreamCount).ToList());
                    return Task.FromResult(result);
                }
            }
            return Task.FromResult(result);
        }
    }
}
