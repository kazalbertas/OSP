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
            if ((NextIds.Count == 0 || NextType == null))
            {
                var result = await GrainFactory.GetGrain<IJob>(JobMgrId, JobMgrType.FullName).GetNext(this.GetPrimaryKey(), GetType());
                if (result.HasValue)
                {
                    NextIds = result.Value.Item1;
                    NextType = result.Value.Item2;
                    var op = new List<(Guid, Type)>();
                    NextIds.ForEach(x => op.Add((x, NextType)));
                    _delegator.SetNextOperators(op);
                }
                else throw new ArgumentNullException("No next operator found, check topology");
                // Need to keep null types in case of sink,
            }
        }
    }
}
