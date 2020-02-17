using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CoreOSP.Partitioner;
using Orleans.Streams;

namespace GrainImplementations.Operators
{
    public abstract class Operator<T> : Grain, IOperator
    {

        protected Guid NextStreamGuid { get; set; }
        protected List<int> NextStreamIds = new List<int>();

        protected Guid JobMgrId;
        protected Type JobMgrType;
        protected bool Last = false;

        protected IPartitioner _partitioner;

        public override Task OnActivateAsync()
        {
            if (GetType().GetInterfaces().Contains(typeof(ISink))) Last = true;

            return base.OnActivateAsync();
        }

        public async virtual Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken)
        {
            (object input, Metadata metadata) = packedInput;
            if ((NextStreamIds.Count == 0 || NextStreamGuid == null) && !Last) 
            {
                var result = await GrainFactory.GetGrain<IJob>(JobMgrId, JobMgrType.FullName).GetOutputStreams(this.GetPrimaryKey(), GetType());
                if (result.HasValue)
                {
                    NextStreamGuid = result.Value.Item1;
                    NextStreamIds = result.Value.Item2;
                    _partitioner.SetOutputStreams(NextStreamGuid,NextStreamIds);
                }
                else throw new ArgumentNullException("No next operator found, check topology");
                // Need to keep null types in case of sink,
            }
            if (input is Data<TerminationEvent>) ProcessTerminationEvent(input as TerminationEvent);
            else if (input is Watermark) ProcessWatermark(input as Watermark, metadata);
            else if (input is Checkpoint) ProcessCheckpoint(input as Checkpoint, metadata);
            else if (input is Data<T>) ProcessData((Data<T>)input, metadata);
            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
            //return Task.CompletedTask;
        }

        public Task Init(Guid jobMgrId, Type jobMgrType, Type delegator)
        {
            JobMgrType = jobMgrType;
            JobMgrId = jobMgrId;
            _partitioner = (IPartitioner) Activator.CreateInstance(delegator);
            return Task.CompletedTask;
        }

        public async Task GetSubscribedStreams() 
        {
            var result = await GrainFactory.GetGrain<IJob>(JobMgrId, JobMgrType.FullName).GetStreamsSubscribe(this.GetPrimaryKey(), GetType());
            var provider = GetStreamProvider("SMSProvider");

            foreach (var r in result) 
            {
                (Guid guid, List<int> id) = r;
                foreach (var i in id)
                {
                    var s = provider.GetStream<(object, Metadata)>(guid, i.ToString());
                    await s.SubscribeAsync(Process);
                }
            }
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

        public virtual void ProcessWatermark(Watermark wm, Metadata metadata) 
        {
           // foreach (var i in NextIds)
           // {
                //GrainFactory.GetGrain<IOperator>(i, NextType.FullName).Process(wm, GetMetadata());
            //}
        }

        public virtual void ProcessCheckpoint(Checkpoint cp, Metadata metadata) 
        {
            //foreach (var i in NextIds) 
           // {
                //GrainFactory.GetGrain<IOperator>(i, NextType.FullName).Process(cp, GetMetadata());
            //}
        }

        public virtual void ProcessTerminationEvent(TerminationEvent tevent) 
        {
            return;
        }

        public async Task SendToNextStreamData(object key, object obj, Metadata md) 
        {
            var next = _partitioner.GetNextStream(key);
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<(object,Metadata)>(next.Item1, next.Item2.ToString());
            await stream.OnNextAsync((obj, md));
        }

        public async Task SendToNextStreamWatermark(Watermark wm, Metadata md) 
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            foreach (var id in NextStreamIds) 
            {
                var stream = streamProvider.GetStream<(object, Metadata)>(NextStreamGuid, id.ToString());
                await stream.OnNextAsync((wm, md));
            }
        }

        
    }
}
