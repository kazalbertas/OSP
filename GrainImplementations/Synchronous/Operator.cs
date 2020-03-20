using CoreOSP;
using CoreOSP.Models;
using CoreOSP.Partitioner;
using GrainInterfaces.Operators;
using Orleans;
using Orleans.Streams;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Synchronous
{
    public abstract class Operator : Grain, IOperator
    {
        protected Guid NextStreamGuid { get; set; }
        protected List<int> NextStreamIds = new List<int>();

        protected OperatorInitConfig Config { get; set; }
        protected bool Last = false;

        protected IPartitioner _partitioner;


        public override Task OnActivateAsync()
        {
            if (GetType().GetInterfaces().Contains(typeof(ISink))) Last = true;
            return base.OnActivateAsync();
        }

        public Task Init(OperatorInitConfig operatorInitConfig)
        {
            Config = operatorInitConfig;
            _partitioner = (IPartitioner)Activator.CreateInstance(operatorInitConfig.Partitioner);
            return Task.CompletedTask;
        }

        internal async Task SetOutputStreamsAsync()
        {
            if ((NextStreamIds.Count == 0 || NextStreamGuid == null) && !Last)
            {
                var result = await GrainFactory.GetGrain<IJob>(Config.JobManagerGuid, Config.JobManagerType.FullName).GetOutputStreams(this.GetPrimaryKey(), GetType());
                if (result.HasValue)
                {
                    NextStreamGuid = result.Value.Item1;
                    NextStreamIds = result.Value.Item2;
                    _partitioner.SetOutputStreams(NextStreamGuid, NextStreamIds);
                }
                else throw new ArgumentNullException("No next operator found, check topology");
            }
        }

        public async Task GetSubscribedStreams()
        {
            var result = await GrainFactory.GetGrain<IJob>(Config.JobManagerGuid, Config.JobManagerType.FullName).GetStreamsSubscribe(this.GetPrimaryKey(), GetType());
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

        public async Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken) 
        {
            (object input, Metadata metadata) = packedInput;
            await SetOutputStreamsAsync();
            if (input is Data<TerminationEvent>) await ProcessTerminationEvent(input as Data<TerminationEvent>);
            else if (input is Watermark) await ProcessWatermark(input as Watermark, metadata);
            else if (input is Checkpoint) await ProcessCheckpoint(input as Checkpoint, metadata);
            else await ProcessInput(input, metadata); 
        }

        public abstract Task ProcessInput(object input , Metadata metadata);
        public virtual async Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            await SendToNextStreamWatermark(wm, GetMetadata());
        }

        public virtual Task ProcessCheckpoint(Checkpoint cp, Metadata metadata)
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessTerminationEvent(Data<TerminationEvent> tevent)
        {
            return Task.CompletedTask;
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

        public abstract Task SendToNextStreamData(object key, object obj, Metadata md);
    }

}
