using CoreOSP;
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
        public override void ProcessCheckpoint(Checkpoint cp, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void ProcessData(Data<T> data, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void ProcessWatermark(Watermark wm, Metadata metadata)
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

        public void SendMessageToStream(Data<T> dt)
        {

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SendToNextStreamData(dt.Key, dt, GetMetadata());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            switch (Policy)
            {
                case TimePolicy.EventTime:

                    if (ExtractTimestamp(dt.Value).Subtract(LastIssueTime) > WatermarkIssuePeriod())
                    {

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        SendToNextStreamWatermark(GenerateWatermark(dt.Value), GetMetadata());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        
                        LastIssueTime = ExtractTimestamp(dt.Value);
                    }
                    break;

                case TimePolicy.ProcessingTime:
                    if (dt.TimeStamp.Subtract(LastIssueTime) > WatermarkIssuePeriod())
                    {

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        SendToNextStreamWatermark(new Watermark(DateTime.Now), GetMetadata());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                       
                        LastIssueTime = dt.TimeStamp;
                    }
                    break;
                default:
                    break;
            }

        }

        public abstract DateTime ExtractTimestamp(T data);
        public abstract TimeSpan MaxOutOfOrder();
        public abstract TimeSpan WatermarkIssuePeriod();

        public virtual Watermark GenerateWatermark(T input) 
        {
            return new Watermark(ExtractTimestamp(input).Subtract(MaxOutOfOrder()));
        }
    }
}
