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
using CoreOSP;

namespace GrainImplementations.Operators
{
    public abstract class Operator<T> : Grain, IOperator
    {

        protected Guid NextStreamGuid { get; set; }
        protected List<int> NextStreamIds = new List<int>();

        protected OperatorInitConfig Oicfg { get; set; }
        protected bool Last = false;

        protected IPartitioner _partitioner;

        public override Task OnActivateAsync()
        {
            if (GetType().GetInterfaces().Contains(typeof(ISink))) Last = true;

            return base.OnActivateAsync();
        }

        public async virtual Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken)
        {
            try
            {
                (object input, Metadata metadata) = packedInput;
                if ((NextStreamIds.Count == 0 || NextStreamGuid == null) && !Last)
                {
                    var result = await GrainFactory.GetGrain<IJob>(Oicfg.JobManagerGuid, Oicfg.JobManagerType.FullName).GetOutputStreams(this.GetPrimaryKey(), GetType());
                    if (result.HasValue)
                    {
                        NextStreamGuid = result.Value.Item1;
                        NextStreamIds = result.Value.Item2;
                        _partitioner.SetOutputStreams(NextStreamGuid, NextStreamIds);
                    }
                    else throw new ArgumentNullException("No next operator found, check topology");
                    // Need to keep null types in case of sink,
                }
                switch (Oicfg.ProcessingType)
                {
                    case ProcessingType.SynchronizeEach:
                        {
                            if (input is Data<TerminationEvent>) await ProcessTerminationEvent(input as Data<TerminationEvent>);
                            else if (input is Watermark) await ProcessWatermark(input as Watermark, metadata);
                            else if (input is Checkpoint) await ProcessCheckpoint(input as Checkpoint, metadata);
                            else if (input is Data<T>) await ProcessData((Data<T>)input, metadata);
                            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
                            break;
                        }
                    case ProcessingType.Asynchronous:
                        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            if (input is Data<TerminationEvent>) ProcessTerminationEvent(input as Data<TerminationEvent>);
                            else if (input is Watermark) ProcessWatermark(input as Watermark, metadata);
                            else if (input is Checkpoint) ProcessCheckpoint(input as Checkpoint, metadata);
                            else if (input is Data<T>) ProcessData((Data<T>)input, metadata);
                            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
                            break;
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        }

                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            //return Task.CompletedTask;
        }

        public Task Init(OperatorInitConfig operatorInitConfig)
        {
            Oicfg = operatorInitConfig;
            _partitioner = (IPartitioner)Activator.CreateInstance(operatorInitConfig.Partitioner);
            return Task.CompletedTask;
        }

        public async Task GetSubscribedStreams() 
        {
            var result = await GrainFactory.GetGrain<IJob>(Oicfg.JobManagerGuid, Oicfg.JobManagerType.FullName).GetStreamsSubscribe(this.GetPrimaryKey(), GetType());
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

        public abstract Task ProcessData(Data<T> input, Metadata metadata);

        public virtual async Task ProcessWatermark(Watermark wm, Metadata metadata) 
        {
            await SendToNextStreamWatermark(wm, GetMetadata());
        }

        public virtual Task ProcessCheckpoint(Checkpoint cp, Metadata metadata) 
        {
            //foreach (var i in NextIds) 
            // {
            //GrainFactory.GetGrain<IOperator>(i, NextType.FullName).Process(cp, GetMetadata());
            //}
            return Task.CompletedTask;
        }

        public virtual Task ProcessTerminationEvent(Data<TerminationEvent> tevent) 
        {
            return Task.CompletedTask;
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
