using GrainInterfaces.Operators;
using Orleans;
using OSPTopologyManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSPJobManager
{
    public class Job : Grain, IJob
    {

        protected TopologyManager tpm { get; set; }

        public async Task StartJob(TopologyManager mgr) 
        {
            tpm = mgr;
            foreach (var s in mgr.Streams) 
            {
                var curr = s;
                do
                {
                    foreach (var guid in curr.OperatorId)
                    {
                        await GrainFactory.GetGrain<IOperator>(guid, curr.OperatorType.FullName).Init(this.GetPrimaryKey(), GetType(), mgr.Conf.Delegator);
                    }
                    curr = curr.Next;
                } while (curr != null);
                
            }
        }

        public Task<(List<Guid>,Type)?> GetNext(Guid guid, Type type)
        {
            (List<Guid>, Type)? result = null;
            foreach (var ds in tpm.Streams) 
            {
                var curr = ds;
                do
                {
                    if (curr.OperatorId.Contains(guid) && curr.OperatorType == type)
                    {
                        result = (curr.Next.OperatorId, curr.Next.OperatorType);
                        return Task.FromResult(result);
                    }
                    curr = curr.Next;
                } while (curr.Next != null);
            }
            return Task.FromResult(result);
        }
    }
}
