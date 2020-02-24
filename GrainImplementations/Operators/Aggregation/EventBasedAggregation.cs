using CoreOSP.Models;
using GrainInterfaces.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrainImplementations.Operators.Aggregation
{
    //Continuous event based aggregation
    public abstract class EventBasedAggregation<T, K> : Operator<T>, IEventBasedAggregation
    {

        private List<Data<T>> data = new List<Data<T>>();

        private DateTime Watermark { get; set; }
        private List<TerminationEvent> TerminationEvent { get;} = new List<TerminationEvent>();

        //returns true if update
        public abstract bool CheckUpdateEvent(T input, T aggEvent);

        public override async Task ProcessWatermark(Watermark wm, Metadata metadata)
        {
            Watermark = wm.TimeStamp;
            foreach (var e in TerminationEvent.ToArray())
            {
                if (e.TimeStamp <= Watermark) 
                {
                    RemoveProcessed(e.Key, e.TimeStamp);
                    TerminationEvent.Remove(e);
                }
            }
            await SendToNextStreamWatermark(wm, GetMetadata());
        }

        public override async Task ProcessData(Data<T> input, Metadata metadata)
        {
            data.RemoveAll(x => x.Key.Equals(input.Key) && CheckUpdateEvent(input.Value, x.Value));
            data.Add(input);
            var filteredData = data.FindAll(x => x.Key.Equals(input.Key));
            var result = AggregateResults(filteredData.Select(x=>x.Value).ToList());
            if (Filter(result)) 
            {
                await SendToNextStreamData(input.Key, new Data<K>(input.Key, result), GetMetadata());
            }
        }

        private void RemoveProcessed(object key, DateTime timestamp) 
        {
            data.RemoveAll(x => x.Key.Equals(key) && ExtractDateTime(x.Value) <= timestamp);
        }

        public override Task ProcessTerminationEvent(Data<TerminationEvent> tevent)
        {
            TerminationEvent.Add(tevent.Value);
            foreach (var e in TerminationEvent.ToArray())
            {
                if (e.TimeStamp <= Watermark)
                {
                    RemoveProcessed(e.Key, e.TimeStamp);
                    TerminationEvent.Remove(e);
                }
            }
            return Task.CompletedTask;
        }

        public abstract K AggregateResults(List<T> items);
        public abstract DateTime ExtractDateTime(T item);
        // figure out change of type for map
        public virtual K Map(K input) 
        {
            return input;
        }

        public virtual bool Filter(K input)
        {
            return true;
        }
    }
}
