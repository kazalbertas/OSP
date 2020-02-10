using Orleans;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSPJobManager
{
    public interface IJob : IGrainWithGuidKey
    {
        public Task StartJob(TopologyManager mgr);
        public Task<List<(Guid, List<int>)>> GetStreamsSubscribe(Guid guid, Type type);
        public Task<(Guid, List<int>)?> GetOutputStreams(Guid guid, Type type);
    }
}
