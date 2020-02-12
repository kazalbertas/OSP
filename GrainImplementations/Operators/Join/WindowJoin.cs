using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using Orleans.Streams;
using OSPJobManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Join
{
    public abstract class WindowJoin<T,K> : Operator<T>, IWindowJoin
    {
        private List<Guid> SourceA;
        private List<Guid> SourceB;

        protected List<Data<T>> sourceAInput = new List<Data<T>>();
        protected List<Data<K>> sourceBInput = new List<Data<K>>();

        internal DateTime WindowStart = DateTime.MinValue;
        internal DateTime WatermarkA = DateTime.MinValue;
        internal DateTime WatermarkB = DateTime.MinValue;

        public override async Task Process((object, Metadata) packedInput, StreamSequenceToken sequenceToken)
        {
            (object input, Metadata metadata) = packedInput;
            if ((NextStreamIds.Count == 0 || NextStreamGuid == null) && !Last)
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

            if (input is Watermark) ProcessWatermark(input as Watermark, metadata);
            else if (input is Checkpoint) ProcessCheckpoint(input as Checkpoint, metadata);
            else if (input is Data<T> && SourceA.Contains(metadata.SenderId)) ProcessData((Data<T>)input, metadata);
            else if (input is Data<K> && SourceB.Contains(metadata.SenderId)) ProcessData((Data<K>)input, metadata);
            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
        }

        public abstract void ProcessData(Data<K> input, Metadata metadata);

        public override void ProcessWatermark(Watermark wm, Metadata metadata)
        {
            if (SourceA.Contains(metadata.SenderId)) 
            {
                WatermarkA = wm.TimeStamp;
            } else if (SourceB.Contains(metadata.SenderId))
            {
                WatermarkB = wm.TimeStamp;
            }

            ProcessWindow();

        }

        internal void RemoveOutsideWindowData(DateTime newWindowStart)
        {
            sourceAInput = sourceAInput.Where(x => ExtractTimestamp(x) >= newWindowStart).ToList();
            sourceBInput = sourceBInput.Where(x => ExtractTimestamp(x) >= newWindowStart).ToList();
        }

        public Task SetSources(List<Guid> sourceA, List<Guid> sourceB)
        {
            SourceA = sourceA;
            SourceB = sourceB;
            return Task.CompletedTask;
        }
        
        

        public abstract void ProcessWindow();

        public virtual TimeSpan AllowedLateness()
        {
            return TimeSpan.Zero;
        }

        public abstract DateTime ExtractTimestamp(Data<T> input);
        public abstract DateTime ExtractTimestamp(Data<K> input);
    }
}
