using CoreOSP.Delegators;
using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace GrainImplementations.Operators
{
    public abstract class Operator<T> : Grain, IOperator
    {
        protected List<Guid> NextIds = new List<Guid>();
        protected Type NextType;

        protected Guid JobMgrId;
        protected Type JobMgrType;
        private bool Last = false;

        protected IDelegator _delegator;

        public override Task OnActivateAsync()
        {
            if (GetType().GetInterfaces().Contains(typeof(ISink))) Last = true;

            return base.OnActivateAsync();
        }

        public async Task Process(object input, Metadata metadata)
        {
            if ((NextIds.Count == 0 || NextType == null) && !Last) 
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

            if (input is Watermark) ProcessWatermark(input as Watermark);
            else if (input is Checkpoint) ProcessCheckpoint(input as Checkpoint);
            else if (input is Data<T>) ProcessData((Data<T>)input, metadata);
            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
            //return Task.CompletedTask;
        }

        public Task Init(Guid jobMgrId, Type jobMgrType, Type delegator)
        {
            JobMgrType = jobMgrType;
            JobMgrId = jobMgrId;
            _delegator = (IDelegator) Activator.CreateInstance(delegator);
            return Task.CompletedTask;
        }

        public Metadata GetMetadata()
        {
            return new Metadata()
            {
                SenderId = this.GetPrimaryKey(),
                SenderType = this.GetType()
            };
        }

        public abstract void ProcessData(Data<T> input, Metadata metadata);

        public virtual void ProcessWatermark(Watermark wm) 
        {
            foreach (var i in NextIds)
            {
                GrainFactory.GetGrain<IOperator>(i, NextType.FullName).Process(wm, GetMetadata());
            }
        }

        public virtual void ProcessCheckpoint(Checkpoint cp) 
        {
            foreach (var i in NextIds) 
            {
                GrainFactory.GetGrain<IOperator>(i, NextType.FullName).Process(cp, GetMetadata());
            }
        }

        
    }
}
