using CoreOSP;
using CoreOSP.Models;
using GrainInterfaces.Operators;
using Orleans;
using OSPJobManager;
using System;
using System.Threading.Tasks;

namespace GrainImplementations.Operators
{
    public abstract class Source<T> : Operator<T>, ISource
    {
        public override Task ProcessCheckpoint(Checkpoint cp, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override Task ProcessData(Data<T> data, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public abstract Task Start();

        public abstract T ProcessMessage(string message);
        public abstract object GetKey(T input);

        public DateTime PreviousTime;
        public DateTime LastIssueTime { get; set; } = DateTime.MinValue;
        protected TimePolicy Policy { get; set; }

        public async Task InitSource(TimePolicy policy)
        {
            Policy = policy;
            if ((NextStreamIds.Count == 0 || NextStreamGuid == null))
            {
                var result = await GrainFactory.GetGrain<IJob>(Oicfg.JobManagerGuid, Oicfg.JobManagerType.FullName).GetOutputStreams(this.GetPrimaryKey(), GetType());
                if (result.HasValue)
                {
                    NextStreamGuid = result.Value.Item1;
                    NextStreamIds = result.Value.Item2;
                    _partitioner.SetOutputStreams(NextStreamGuid, NextStreamIds);
                }
                else throw new ArgumentNullException("No next operator found, check topology");
            }
        }

        public async Task SendMessageToStream(Data<T> dt)
        {
            await SendToNextStreamData(dt.Key, dt, GetMetadata());

            switch (Policy)
            {
                case TimePolicy.EventTime:

                    if (ExtractEventTime(dt.Value).Subtract(LastIssueTime) >= WatermarkIssuePeriod())
                    {
                        await SendToNextStreamWatermark(GenerateWatermark(dt.Value), GetMetadata());
                        LastIssueTime = ExtractEventTime(dt.Value);
                    }
                    break;

                case TimePolicy.ProcessingTime:
                    if (dt.ProcessingTime.Subtract(LastIssueTime) >= WatermarkIssuePeriod())
                    {
                        await SendToNextStreamWatermark(new Watermark(DateTime.Now), GetMetadata());
                        LastIssueTime = dt.ProcessingTime;
                    }
                    break;
                default:
                    break;
            }

        }

        public abstract DateTime ExtractEventTime(T data);
        public abstract TimeSpan MaxOutOfOrder();
        public abstract TimeSpan WatermarkIssuePeriod();

        public virtual Watermark GenerateWatermark(T input) 
        {
            return new Watermark(ExtractEventTime(input).Subtract(MaxOutOfOrder()));
        }
    }
}
