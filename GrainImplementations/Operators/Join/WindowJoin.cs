﻿using CoreOSP;
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

        protected Dictionary<object,List<Data<T>>> sourceAInput = new Dictionary<object, List<Data<T>>>();
        protected Dictionary<object, List<Data<K>>> sourceBInput = new Dictionary<object, List<Data<K>>>();

        internal DateTime WindowStart = DateTime.MinValue;
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

            if (input is Watermark) await ProcessWatermark(input as Watermark, metadata);
            else if (input is Checkpoint) await ProcessCheckpoint(input as Checkpoint, metadata);
            else if (input is Data<T> && SourceA.Contains(metadata.SenderId)) await ProcessData((Data<T>)input, metadata);
            else if (input is Data<K> && SourceB.Contains(metadata.SenderId)) await ProcessData((Data<K>)input, metadata);
            else throw new ArgumentException("Argument is not of type " + typeof(T).FullName);
        }

        public abstract Task ProcessData(Data<K> input, Metadata metadata);

        public override Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            if (SourceA.Contains(metadata.SenderId)) 
            {
                WatermarkA = wm.TimeStamp;
            } else if (SourceB.Contains(metadata.SenderId))
            {
                WatermarkB = wm.TimeStamp;
            }

            if (WindowStart != DateTime.MinValue) ProcessWindow();
            return Task.CompletedTask;
        }

        internal void RemoveOutsideWindowData(DateTime newWindowStart)
        {
            foreach (var key in sourceAInput.Keys)
            {
                sourceAInput[key].RemoveAll(x => ExtractDateTime(x) >= newWindowStart);
            
            }

            foreach (var key in sourceBInput.Keys)
            {
                sourceBInput[key].RemoveAll(x => ExtractDateTime(x) >= newWindowStart);

            }
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

        protected DateTime ExtractDateTime(Data<T> input)
        {
            var dt = Oicfg.TimeCharacteristic switch
            {
                (TimePolicy.EventTime) => ExtractEventTime(input.Value),
                (TimePolicy.ProcessingTime) => input.ProcessingTime,
                (TimePolicy.None) => throw new MissingMemberException("Join requires not none time characteristic"),
                _ => throw new NotSupportedException("Time characteristic not supported"),
            };
            return dt;
        }

        protected DateTime ExtractDateTime(Data<K> input)
        {
            var dt = Oicfg.TimeCharacteristic switch
            {
                (TimePolicy.EventTime) => ExtractEventTime(input.Value),
                (TimePolicy.ProcessingTime) => input.ProcessingTime,
                (TimePolicy.None) => throw new MissingMemberException("Join requires not none time characteristic"),
                _ => throw new NotSupportedException("Time characteristic not supported"),
            };
            return dt;
        }

        public abstract DateTime ExtractEventTime(T input);
        public abstract DateTime ExtractEventTime(K input);
    }
}
