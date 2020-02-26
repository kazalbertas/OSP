using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using Orleans.Streams;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Join
{
    public abstract class EventBasedJoin<T, K> : EventBasedJoin<T, K, (T, K)> 
    {
        public override (T, K) Map(T inputA, K inputB)
        {
            return (inputA, inputB);
        }
    }
    public abstract class EventBasedJoin<T, K, O> : Operator<T>, IEventBasedJoin
    {
        private List<Guid> SourceA;
        private List<Guid> SourceB;

        protected List<Data<T>> sourceAInput = new List<Data<T>>();
        protected List<Data<K>> sourceBInput = new List<Data<K>>();

        private List<TerminationEvent> TerminationEvent { get; } = new List<TerminationEvent>();

        internal DateTime WatermarkA = DateTime.MinValue;
        internal DateTime WatermarkB = DateTime.MinValue;

        public override async Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken)
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
            if (input is Data<TerminationEvent>) await ProcessTerminationEvent(input as Data<TerminationEvent>);
            else if (input is Watermark) await ProcessWatermark(input as Watermark, metadata);
            else if (input is Checkpoint) await ProcessCheckpoint(input as Checkpoint, metadata);
            else if (input is Data<T> && SourceA.Contains(metadata.SenderId)) await ProcessData((Data<T>)input, metadata);
            else if (input is Data<K> && SourceB.Contains(metadata.SenderId)) await ProcessData((Data<K>)input, metadata);
            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
        }

        public override async Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            if (SourceA.Contains(metadata.SenderId))
            {
                WatermarkA = wm.TimeStamp;
            }
            else if (SourceB.Contains(metadata.SenderId))
            {
                WatermarkB = wm.TimeStamp;
            }

            if (WatermarkA == new DateTime(2019, 10, 10, 10, 10, 13) && WatermarkB == new DateTime(2019, 10, 10, 10, 10, 13))
            {
                Console.WriteLine("aa");
            }

            foreach (var e in TerminationEvent.ToArray())
            {
                if (e.TimeStamp <= WatermarkA && e.TimeStamp <= WatermarkB)
                {
                    RemoveProcessed(e.Key, e.TimeStamp);
                    TerminationEvent.Remove(e);
                }
            }
            await SendToNextStreamWatermark(wm, GetMetadata());
        }

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            sourceAInput.Add(input);

            var sourceBWithSameKey = sourceBInput.Where(x => x.Key.Equals(input.Key)).ToList();
            foreach (var bIn in sourceBWithSameKey)
            {
                if (Filter(input.Value, bIn.Value))
                {
                    var dt = new Data<O>(input.Key, Map(input.Value, bIn.Value));
                    await SendToNextStreamData(input.Key, dt, GetMetadata());
                }
            }
        }

        public async Task ProcessData(Data<K> input, Metadata metadata)
        {
            sourceBInput.Add(input);
            var sourceAWithSameKey = sourceAInput.Where(x => x.Key.Equals(input.Key)).ToList();
            foreach (var aIn in sourceAWithSameKey)
            {
                if (Filter(aIn.Value, input.Value))
                {
                    var dt = new Data<O>(input.Key, Map(aIn.Value, input.Value));
                    await SendToNextStreamData(input.Key, dt, GetMetadata());
                }
            }
        }

        private void RemoveProcessed(object key, DateTime timestamp)
        {
            sourceAInput.RemoveAll(x => x.Key.Equals(key) && ExtractDateTime(x.Value) <= timestamp);
            sourceBInput.RemoveAll(x => x.Key.Equals(key) && ExtractDateTime(x.Value) <= timestamp);
        }

        public override Task ProcessTerminationEvent(Data<TerminationEvent> tevent)
        {
            TerminationEvent.Add(tevent.Value);
            foreach (var e in TerminationEvent.ToArray())
            {
                if (e.TimeStamp <= WatermarkA && e.TimeStamp <= WatermarkB)
                {
                    RemoveProcessed(e.Key, e.TimeStamp);
                    TerminationEvent.Remove(e);
                }
            }
            return Task.CompletedTask;
        }


        public virtual bool Filter(T inputA, K inputB)
        {
            return true;
        }

        public abstract O Map(T inputA, K inputB);

        public Task SetSources(List<Guid> sourceA, List<Guid> sourceB)
        {
            SourceA = sourceA;
            SourceB = sourceB;
            return Task.CompletedTask;
        }

        public abstract DateTime ExtractDateTime(T input);
        public abstract DateTime ExtractDateTime(K input);
    }
}
