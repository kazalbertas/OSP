using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class Source<T> : Operator<T>, ISource
    {
        public override void ProcessCheckpoint(Checkpoint cp)
        {
            throw new NotImplementedException();
        }

        public override void ProcessData(Data<T> data, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void ProcessWatermark(Watermark wm)
        {
            throw new NotImplementedException();
        }

        public abstract Task Start();

        public abstract T ProcessMessage(string message);
        public abstract object GetKey(T input);

        public async Task InitSource()
        {
            if ((NextStreamIds.Count == 0 || NextStreamGuid == null))
            {
                var result = await GrainFactory.GetGrain<IJob>(JobMgrId, JobMgrType.FullName).GetOutputStreams(this.GetPrimaryKey(), GetType());
                if (result.HasValue)
                {
                    NextStreamGuid = result.Value.Item1;
                    NextStreamIds = result.Value.Item2;
                    _partitioner.SetOutputStreams(NextStreamGuid, NextStreamIds);
                }
                else throw new ArgumentNullException("No next operator found, check topology");
                // Need to keep null types in case of sink,
            }
        }
    }
}
